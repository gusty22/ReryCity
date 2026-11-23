using System;
using System.Collections.Generic;
using UnityEngine;
using ProjetoGrau2.Core;
using ProjetoGrau2.Traffic;

namespace ProjetoGrau2.Police
{
    /// <summary>
    /// Coordenador tático central de perseguições policiais (Police Pursuit Coordinator).
    /// Centraliza o rádio COPOM, compartilhamento em tempo real da Última Posição Conhecida (LKP),
    /// distribuição dinâmica de papéis táticos (Lead, Flanker, Interceptor, PerimeterSearch)
    /// e integração com o nível de procurado (WantedLevelManager).
    /// </summary>
    [AddComponentMenu("Projeto Grau 2/Police/Police Pursuit Coordinator")]
    public class PolicePursuitCoordinator : MonoBehaviour
    {
        public static PolicePursuitCoordinator Instance { get; private set; }

        [Header("Configurações Táticas e Escala")]
        [Tooltip("Número máximo de viaturas ativas na perseguição simultânea (Otimização Android).")]
        [Range(1, 6)] public int maxPursuitUnits = 4;

        [Tooltip("Raio de busca perimetral ao redor da LKP (metros).")]
        [Range(15f, 60f)] public float searchRadius = 35f;

        [Tooltip("Tempo máximo de busca após perda total de contato visual (segundos).")]
        [Range(5f, 30f)] public float searchTimeout = 14f;

        [Tooltip("Offset lateral para posicionamento de flanqueadores (metros).")]
        [Range(2f, 8f)] public float flankLateralOffset = 4.0f;

        [Tooltip("Distância de recuo dos flanqueadores em relação ao alvo (metros).")]
        [Range(3f, 15f)] public float flankFollowOffset = 6.0f;

        [Tooltip("Tempo de predição linear da rota do suspeito para corte de rota pelo interceptador (segundos).")]
        [Range(1f, 4f)] public float interceptLookAheadTime = 2.5f;

        [Header("Estado Global da Perseguição")]
        [SerializeField] private bool isPursuitActive = false;
        [SerializeField] private bool hasAnyVisualContact = false;
        [SerializeField] private Vector3 globalLastKnownPosition = Vector3.zero;
        [SerializeField] private float timeSinceLastVisualContact = 0f;
        [SerializeField] private MotorcycleController targetPlayer;

        // Guarnições policiais registradas e participantes
        private readonly List<PolicePursuitController> _registeredUnits = new List<PolicePursuitController>();
        private readonly List<PolicePursuitController> _activePursuitUnits = new List<PolicePursuitController>();
        private readonly Dictionary<PolicePursuitController, PoliceTacticalRole> _unitRoles = new Dictionary<PolicePursuitController, PoliceTacticalRole>();
        private readonly Dictionary<PolicePursuitController, Vector3> _searchOffsets = new Dictionary<PolicePursuitController, Vector3>();

        // Eventos públicos
        public event Action<bool> OnPursuitStateChanged;
        public event Action<PolicePursuitController, PoliceTacticalRole> OnRoleAssigned;
        public event Action<Vector3> OnSuspectSighted;
        public event Action OnPursuitEvaded;
        public event Action OnPursuitBusted;

        public bool IsPursuitActive => isPursuitActive;
        public bool HasAnyVisualContact => hasAnyVisualContact;
        public Vector3 GlobalLastKnownPosition => globalLastKnownPosition;
        public float TimeSinceLastVisualContact => timeSinceLastVisualContact;
        public MotorcycleController TargetPlayer => targetPlayer;
        public IReadOnlyList<PolicePursuitController> ActivePursuitUnits => _activePursuitUnits;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            FindTargetPlayer();
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void FindTargetPlayer()
        {
            if (targetPlayer == null)
            {
                targetPlayer = FindFirstObjectByType<MotorcycleController>();
            }
        }

        public void RegisterUnit(PolicePursuitController unit)
        {
            if (unit == null || _registeredUnits.Contains(unit)) return;
            _registeredUnits.Add(unit);
        }

        public void UnregisterUnit(PolicePursuitController unit)
        {
            if (unit == null) return;
            _registeredUnits.Remove(unit);
            _activePursuitUnits.Remove(unit);
            _unitRoles.Remove(unit);
            _searchOffsets.Remove(unit);
        }

        private void Update()
        {
            float dt = Time.deltaTime;

            if (targetPlayer == null)
            {
                FindTargetPlayer();
            }

            if (isPursuitActive)
            {
                UpdatePursuitStatus(dt);
                EvaluateUnitCoordination();
            }
            else
            {
                CheckForNewPursuitInitiation();
            }
        }

        private void CheckForNewPursuitInitiation()
        {
            int wanted = (WantedLevelManager.Instance != null) ? WantedLevelManager.Instance.CurrentWantedLevel : 0;
            if (wanted >= 1)
            {
                // Se o jogador está procurado e qualquer viatura o avistar, inicia perseguição
                for (int i = 0; i < _registeredUnits.Count; i++)
                {
                    var unit = _registeredUnits[i];
                    if (unit != null && unit.HasVisualContact)
                    {
                        StartCoordinatedPursuit(unit.LastKnownPosition);
                        break;
                    }
                }
            }
        }

        public void StartCoordinatedPursuit(Vector3 initialPosition)
        {
            if (isPursuitActive) return;

            isPursuitActive = true;
            hasAnyVisualContact = true;
            globalLastKnownPosition = initialPosition;
            timeSinceLastVisualContact = 0f;

            OnPursuitStateChanged?.Invoke(true);
            OnSuspectSighted?.Invoke(initialPosition);

            // Mobiliza viaturas disponíveis de acordo com o nível de procurado
            MobilizeUnitsForPursuit();
        }

        private void MobilizeUnitsForPursuit()
        {
            _activePursuitUnits.Clear();
            _unitRoles.Clear();

            int wantedLevel = (WantedLevelManager.Instance != null) ? WantedLevelManager.Instance.CurrentWantedLevel : 1;
            int desiredUnits = Mathf.Clamp(wantedLevel, 1, maxPursuitUnits);

            // Ordena viaturas disponíveis por proximidade da LKP
            var availableUnits = new List<PolicePursuitController>();
            for (int i = 0; i < _registeredUnits.Count; i++)
            {
                var unit = _registeredUnits[i];
                if (unit != null && unit.gameObject.activeInHierarchy)
                {
                    availableUnits.Add(unit);
                }
            }

            availableUnits.Sort((a, b) =>
            {
                float distA = (a.transform.position - globalLastKnownPosition).sqrMagnitude;
                float distB = (b.transform.position - globalLastKnownPosition).sqrMagnitude;
                return distA.CompareTo(distB);
            });

            int added = 0;
            for (int i = 0; i < availableUnits.Count && added < desiredUnits; i++)
            {
                var unit = availableUnits[i];
                _activePursuitUnits.Add(unit);
                added++;
            }

            AssignTacticalRoles();
        }

        /// <summary>
        /// Distribui papéis táticos especializados entre viaturas convencionais e ROCAM.
        /// Motos ROCAM têm prioridade absoluta como LeadPursuer devido à agilidade.
        /// </summary>
        public void AssignTacticalRoles()
        {
            if (_activePursuitUnits.Count == 0) return;

            PolicePursuitController leadCandidate = null;

            // 1. Prioriza ROCAM (veículo de duas rodas) como LeadPursuer
            for (int i = 0; i < _activePursuitUnits.Count; i++)
            {
                var unit = _activePursuitUnits[i];
                if (unit != null && unit.vehicleData != null && unit.vehicleData.isTwoWheeled)
                {
                    leadCandidate = unit;
                    break;
                }
            }

            // Se não houver ROCAM, pega a viatura mais próxima
            if (leadCandidate == null)
            {
                float minSqrDist = float.MaxValue;
                for (int i = 0; i < _activePursuitUnits.Count; i++)
                {
                    var unit = _activePursuitUnits[i];
                    if (unit == null) continue;
                    float sqrDist = (unit.transform.position - globalLastKnownPosition).sqrMagnitude;
                    if (sqrDist < minSqrDist)
                    {
                        minSqrDist = sqrDist;
                        leadCandidate = unit;
                    }
                }
            }

            // Atribui papéis na ordem: Lead -> FlankerLeft -> FlankerRight -> Interceptor
            int roleIndex = 0;
            if (leadCandidate != null)
            {
                SetUnitRole(leadCandidate, PoliceTacticalRole.LeadPursuer);
                roleIndex++;
            }

            for (int i = 0; i < _activePursuitUnits.Count; i++)
            {
                var unit = _activePursuitUnits[i];
                if (unit == null || unit == leadCandidate) continue;

                if (roleIndex == 1)
                {
                    SetUnitRole(unit, PoliceTacticalRole.FlankerLeft);
                }
                else if (roleIndex == 2)
                {
                    SetUnitRole(unit, PoliceTacticalRole.FlankerRight);
                }
                else
                {
                    SetUnitRole(unit, PoliceTacticalRole.Interceptor);
                }
                roleIndex++;
            }
        }

        private void SetUnitRole(PolicePursuitController unit, PoliceTacticalRole role)
        {
            _unitRoles[unit] = role;
            OnRoleAssigned?.Invoke(unit, role);
        }

        public PoliceTacticalRole GetUnitRole(PolicePursuitController unit)
        {
            if (unit != null && _unitRoles.TryGetValue(unit, out var role))
            {
                return role;
            }
            return PoliceTacticalRole.None;
        }

        /// <summary>
        /// Chamado por qualquer guarnição que avistar o suspeito (comunicação de rádio COPOM).
        /// </summary>
        public void ReportVisualContact(PolicePursuitController reportingUnit, Vector3 targetPos)
        {
            hasAnyVisualContact = true;
            globalLastKnownPosition = targetPos;
            timeSinceLastVisualContact = 0f;

            if (!isPursuitActive)
            {
                StartCoordinatedPursuit(targetPos);
            }
            else
            {
                OnSuspectSighted?.Invoke(targetPos);
            }

            if (WantedLevelManager.Instance != null)
            {
                WantedLevelManager.Instance.SetPlayerSpotted(true);
            }
        }

        /// <summary>
        /// Chamado quando uma viatura perde contato visual.
        /// </summary>
        public void ReportVisualLost(PolicePursuitController reportingUnit)
        {
            // Checa se alguma outra unidade ainda mantém contato visual
            bool anyContact = false;
            for (int i = 0; i < _activePursuitUnits.Count; i++)
            {
                var unit = _activePursuitUnits[i];
                if (unit != null && unit != reportingUnit && unit.HasVisualContact)
                {
                    anyContact = true;
                    break;
                }
            }

            hasAnyVisualContact = anyContact;

            if (!hasAnyVisualContact)
            {
                // Inicia busca perimetral coordenada com quadrantes separados
                DistributeSearchQuadrants();
                if (WantedLevelManager.Instance != null)
                {
                    WantedLevelManager.Instance.SetPlayerSpotted(false);
                }
            }
        }

        private void DistributeSearchQuadrants()
        {
            _searchOffsets.Clear();
            Vector3[] directions = new Vector3[]
            {
                Vector3.forward,
                Vector3.back,
                Vector3.right,
                Vector3.left
            };

            for (int i = 0; i < _activePursuitUnits.Count; i++)
            {
                var unit = _activePursuitUnits[i];
                if (unit == null) continue;

                Vector3 dir = directions[i % directions.Length];
                Vector3 targetSearchPos = globalLastKnownPosition + dir * (searchRadius * 0.75f);
                _searchOffsets[unit] = targetSearchPos;
                SetUnitRole(unit, PoliceTacticalRole.PerimeterSearch);
            }
        }

        public Vector3 GetSearchTargetForUnit(PolicePursuitController unit)
        {
            if (unit != null && _searchOffsets.TryGetValue(unit, out var pos))
            {
                return pos;
            }
            return globalLastKnownPosition;
        }

        /// <summary>
        /// Retorna a posição tática de destino calculada para o papel da viatura.
        /// </summary>
        public Vector3 CalculateTacticalTargetPosition(PolicePursuitController unit)
        {
            if (targetPlayer == null) return globalLastKnownPosition;

            Vector3 playerPos = targetPlayer.transform.position;
            Vector3 playerFwd = targetPlayer.transform.forward;
            Vector3 playerRight = targetPlayer.transform.right;
            float playerSpeedMs = targetPlayer.CurrentSpeedKmh / 3.6f;

            PoliceTacticalRole role = GetUnitRole(unit);

            switch (role)
            {
                case PoliceTacticalRole.LeadPursuer:
                    // Persegue diretamente atrás da moto (offset de segurança)
                    return playerPos;

                case PoliceTacticalRole.FlankerLeft:
                    // Posiciona-se à esquerda ligeiramente recuado para fechar o corredor
                    return playerPos - playerFwd * flankFollowOffset - playerRight * flankLateralOffset;

                case PoliceTacticalRole.FlankerRight:
                    // Posiciona-se à direita ligeiramente recuado para fechar o corredor
                    return playerPos - playerFwd * flankFollowOffset + playerRight * flankLateralOffset;

                case PoliceTacticalRole.Interceptor:
                    // Prevê a rota e busca ultrapassar/cortar a frente
                    return playerPos + playerFwd * (playerSpeedMs * interceptLookAheadTime);

                case PoliceTacticalRole.PerimeterSearch:
                    return GetSearchTargetForUnit(unit);

                default:
                    return playerPos;
            }
        }

        private void UpdatePursuitStatus(float dt)
        {
            if (!hasAnyVisualContact)
            {
                timeSinceLastVisualContact += dt;
                if (timeSinceLastVisualContact >= searchTimeout)
                {
                    // Suspeito despistou a polícia com sucesso (Evaded)
                    EndPursuitEvaded();
                }
            }
            else
            {
                timeSinceLastVisualContact = 0f;
            }
        }

        private void EvaluateUnitCoordination()
        {
            int wanted = (WantedLevelManager.Instance != null) ? WantedLevelManager.Instance.CurrentWantedLevel : 1;
            int targetUnitCount = Mathf.Clamp(wanted, 1, maxPursuitUnits);

            if (_activePursuitUnits.Count != targetUnitCount)
            {
                MobilizeUnitsForPursuit();
            }
        }

        public void ReportBusted(PolicePursuitController arrestingUnit)
        {
            if (!isPursuitActive) return;

            isPursuitActive = false;
            OnPursuitBusted?.Invoke();
            OnPursuitStateChanged?.Invoke(false);

            for (int i = 0; i < _activePursuitUnits.Count; i++)
            {
                var unit = _activePursuitUnits[i];
                if (unit != null)
                {
                    var sm = unit.GetComponent<PoliceAIStateMachine>();
                    if (sm != null)
                    {
                        sm.RequestState(PoliceAIState.Patrol, true);
                    }
                    unit.isSirenActive = false;
                }
            }

            _activePursuitUnits.Clear();
            _unitRoles.Clear();
        }

        private void EndPursuitEvaded()
        {
            isPursuitActive = false;
            OnPursuitEvaded?.Invoke();
            OnPursuitStateChanged?.Invoke(false);

            for (int i = 0; i < _activePursuitUnits.Count; i++)
            {
                var unit = _activePursuitUnits[i];
                if (unit != null)
                {
                    var sm = unit.GetComponent<PoliceAIStateMachine>();
                    if (sm != null)
                    {
                        sm.RequestState(PoliceAIState.ReturningToPatrol, true);
                    }
                    unit.isSirenActive = false;
                }
            }

            _activePursuitUnits.Clear();
            _unitRoles.Clear();
        }
    }
}
