using System;
using System.Collections.Generic;
using UnityEngine;
using ProjetoGrau2.World;

namespace ProjetoGrau2.Police
{
    /// <summary>
    /// Gerenciador de despacho e atendimento de ocorrências policiais (Police Dispatch Manager).
    /// Conecta os eventos do mundo (acidentes, bloqueios e sinistros) à frota policial ativa.
    /// Localiza viaturas em patrulha disponíveis e comanda deslocamento tático e atendimento no local.
    /// </summary>
    [AddComponentMenu("Projeto Grau 2/Police/Police Dispatch Manager")]
    public class PoliceDispatchManager : MonoBehaviour
    {
        public static PoliceDispatchManager Instance { get; private set; }

        [Header("Fila de Ocorrências Pendentes")]
        private readonly Queue<WorldEventInstance> _pendingIncidents = new Queue<WorldEventInstance>();

        [Header("Viaturas em Atendimento Ativo")]
        private readonly Dictionary<PolicePursuitController, WorldEventInstance> _assignedUnits = new Dictionary<PolicePursuitController, WorldEventInstance>();

        private PolicePursuitController[] _cachedPoliceUnits;

        // Eventos públicos
        public event Action<PolicePursuitController, WorldEventInstance> OnUnitDispatched;
        public event Action<PolicePursuitController, WorldEventInstance> OnIncidentArrived;
        public event Action<PolicePursuitController, WorldEventInstance> OnIncidentResolved;

        public int PendingIncidentCount => _pendingIncidents.Count;
        public int AssignedUnitCount => _assignedUnits.Count;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            RefreshPoliceUnitsCache();
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        public void RefreshPoliceUnitsCache()
        {
            _cachedPoliceUnits = FindObjectsByType<PolicePursuitController>(FindObjectsSortMode.None);
        }

        private void Update()
        {
            ProcessPendingDispatches();
            MonitorAssignedUnits(Time.deltaTime);
        }

        /// <summary>
        /// Registra uma nova ocorrência que requer presença policial.
        /// </summary>
        public void DispatchIncident(WorldEventInstance incident)
        {
            if (incident == null || !incident.IsActive) return;

            if (!_pendingIncidents.Contains(incident))
            {
                _pendingIncidents.Enqueue(incident);
                ProcessPendingDispatches();
            }
        }

        private void ProcessPendingDispatches()
        {
            if (_pendingIncidents.Count == 0) return;
            if (_cachedPoliceUnits == null || _cachedPoliceUnits.Length == 0)
            {
                RefreshPoliceUnitsCache();
                if (_cachedPoliceUnits == null || _cachedPoliceUnits.Length == 0) return;
            }

            // Tenta atender o topo da fila
            WorldEventInstance nextIncident = _pendingIncidents.Peek();
            if (nextIncident == null || !nextIncident.IsActive)
            {
                _pendingIncidents.Dequeue();
                return;
            }

            PolicePursuitController availableUnit = FindNearestAvailableUnit(nextIncident.Position);
            if (availableUnit != null)
            {
                _pendingIncidents.Dequeue();
                AssignUnitToIncident(availableUnit, nextIncident);
            }
        }

        private PolicePursuitController FindNearestAvailableUnit(Vector3 targetPos)
        {
            PolicePursuitController nearest = null;
            float minSqrDist = float.MaxValue;

            foreach (var unit in _cachedPoliceUnits)
            {
                if (unit == null || !unit.gameObject.activeInHierarchy) continue;

                // Só despacha viaturas que estão em Patrulha ou Retornando à Patrulha (não em perseguição ativa)
                if (unit.CurrentState == PoliceAIState.Patrol || unit.CurrentState == PoliceAIState.ReturningToPatrol)
                {
                    if (!_assignedUnits.ContainsKey(unit))
                    {
                        float sqrDist = (unit.transform.position - targetPos).sqrMagnitude;
                        if (sqrDist < minSqrDist)
                        {
                            minSqrDist = sqrDist;
                            nearest = unit;
                        }
                    }
                }
            }

            return nearest;
        }

        private void AssignUnitToIncident(PolicePursuitController unit, WorldEventInstance incident)
        {
            _assignedUnits[unit] = incident;

            // Altera estado da viatura para Investigating e liga sirene
            var sm = unit.GetComponent<PoliceAIStateMachine>();
            if (sm != null)
            {
                sm.RequestState(PoliceAIState.Investigating, true);
            }

            var steering = unit.GetComponent<AIVehicleSteeringController>();
            if (steering != null)
            {
                steering.SetTargetPosition(incident.Position);
            }

            unit.isSirenActive = true;
            OnUnitDispatched?.Invoke(unit, incident);
        }

        private void MonitorAssignedUnits(float dt)
        {
            var units = new List<PolicePursuitController>(_assignedUnits.Keys);
            foreach (var unit in units)
            {
                if (unit == null)
                {
                    _assignedUnits.Remove(unit);
                    continue;
                }

                WorldEventInstance incident = _assignedUnits[unit];
                if (incident == null || !incident.IsActive)
                {
                    ReleaseUnit(unit, incident);
                    continue;
                }

                // Checa chegada ao local da ocorrência (raio <= 12m)
                float dist = Vector3.Distance(unit.transform.position, incident.Position);
                if (dist <= 12f)
                {
                    if (incident.currentState == WorldEventState.Active)
                    {
                        incident.StartResolving();
                        OnIncidentArrived?.Invoke(unit, incident);
                    }
                }

                // Se a ocorrência foi concluída
                if (incident.currentState == WorldEventState.Completed)
                {
                    ReleaseUnit(unit, incident);
                }
            }
        }

        private void ReleaseUnit(PolicePursuitController unit, WorldEventInstance incident)
        {
            _assignedUnits.Remove(unit);
            unit.isSirenActive = false;

            var sm = unit.GetComponent<PoliceAIStateMachine>();
            if (sm != null)
            {
                sm.RequestState(PoliceAIState.ReturningToPatrol, true);
            }

            OnIncidentResolved?.Invoke(unit, incident);
        }
    }
}
