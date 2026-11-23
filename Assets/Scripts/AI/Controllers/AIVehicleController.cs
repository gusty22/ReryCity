using System;
using UnityEngine;
using ProjetoGrau2.Traffic;

namespace ProjetoGrau2.AI
{
    /// <summary>
    /// Orquestrador mestre do veículo NPC.
    /// Integra sensores, máquina de estados, motor, direção, frenagem e a malha viária vetorial.
    /// Consome diretamente os dados de TrafficNavigationManager, TrafficLane, TrafficSignal e TrafficIntersection.
    /// </summary>
    [AddComponentMenu("Projeto Grau 2/AI/AI Vehicle Controller")]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(AIVehicleStateMachine))]
    [RequireComponent(typeof(AIVehicleSensor))]
    [RequireComponent(typeof(AIVehicleMotorController))]
    [RequireComponent(typeof(AIVehicleSteeringController))]
    [RequireComponent(typeof(AIVehicleBrakeController))]
    public class AIVehicleController : MonoBehaviour
    {
        [Header("Dados e Perfil")]
        public AIVehicleData vehicleData;
        public AIDriverProfile driverProfile;

        [Header("Navegação na Malha")]
        [SerializeField] private TrafficLane currentLane;
        [SerializeField] private TrafficWaypoint currentWaypoint;
        [SerializeField] private int currentWaypointIndex = 0;
        public float waypointArrivalDistance = 4.5f;

        [Header("Estado de Mudança de Faixa")]
        [SerializeField] private TrafficLane targetAdjacentLane;
        private float laneChangeProgress = 0f;

        // Subcomponentes
        private Rigidbody _rb;
        private AIVehicleStateMachine _stateMachine;
        private AIVehicleSensor _sensor;
        private AIVehicleMotorController _motor;
        private AIVehicleSteeringController _steering;
        private AIVehicleBrakeController _brake;

        public TrafficLane CurrentLane => currentLane;
        public TrafficWaypoint CurrentWaypoint => currentWaypoint;
        public AIVehicleState CurrentState => _stateMachine != null ? _stateMachine.CurrentState : AIVehicleState.Idle;
        public float CurrentSpeedKmh => _motor != null ? _motor.CurrentSpeedKmh : 0f;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _stateMachine = GetComponent<AIVehicleStateMachine>();
            _sensor = GetComponent<AIVehicleSensor>();
            _motor = GetComponent<AIVehicleMotorController>();
            _steering = GetComponent<AIVehicleSteeringController>();
            _brake = GetComponent<AIVehicleBrakeController>();

            if (vehicleData != null)
            {
                InitializeComponents();
            }
        }

        public void InitializeComponents()
        {
            if (vehicleData == null) return;

            _motor.Initialize(vehicleData, driverProfile);
            _steering.Initialize(vehicleData);
            _brake.Initialize(vehicleData, driverProfile);

            if (_sensor != null)
            {
                _sensor.forwardRange = vehicleData.sensorForwardRange;
            }
        }

        /// <summary>
        /// Configura a rota inicial do veículo em uma determinada faixa.
        /// </summary>
        public void SetRoute(TrafficLane lane, int startWaypointIndex = 0)
        {
            currentLane = lane;
            currentWaypointIndex = startWaypointIndex;
            targetAdjacentLane = null;

            if (currentLane != null && currentLane.Waypoints != null && currentLane.Waypoints.Count > 0)
            {
                currentWaypointIndex = Mathf.Clamp(startWaypointIndex, 0, currentLane.Waypoints.Count - 1);
                currentWaypoint = currentLane.Waypoints[currentWaypointIndex];
                if (currentWaypoint != null)
                {
                    _steering.SetTargetWaypoint(currentWaypoint.transform);
                }
            }
        }

        private void Update()
        {
            float dt = Time.deltaTime;
            _stateMachine.Tick(dt);
            _sensor.TickSensors(dt);

            EvaluateStateDecisions();
            ExecuteStateControls();
            CheckWaypointProgression();
        }

        /// <summary>
        /// Avalia percepção e toma decisões na máquina de estados.
        /// </summary>
        private void EvaluateStateDecisions()
        {
            float safeDistance = (vehicleData != null ? vehicleData.baseSafetyDistance : 6f);
            if (driverProfile != null)
            {
                safeDistance *= driverProfile.safeDistanceMultiplier;
            }

            // 1. Verificação de Obstáculo Frontal
            if (_sensor.FrontHit.hasHit)
            {
                float dist = _sensor.FrontHit.distance;

                // Parada de Emergência Crítica
                if (dist < 3.5f || (dist < safeDistance * 0.6f && _sensor.FrontHit.relativeSpeedKmh > 20f))
                {
                    _stateMachine.RequestState(AIVehicleState.EmergencyStop);
                    return;
                }

                // Oportunidade de ultrapassagem (se permitido pelo perfil e se a esquerda estiver livre)
                if (dist < safeDistance * 1.5f && _stateMachine.CanInitiateLaneChange && currentLane != null && currentLane.LeftAdjacentLane != null)
                {
                    float roll = UnityEngine.Random.value;
                    float propensity = (driverProfile != null) ? driverProfile.laneChangePropensity : 0.3f;

                    if (roll <= propensity && _sensor.IsLeftClear)
                    {
                        InitiateLaneChange(currentLane.LeftAdjacentLane, true);
                        return;
                    }
                }

                // Seguir veículo à frente ou frear
                if (dist < safeDistance)
                {
                    _stateMachine.RequestState(AIVehicleState.FollowVehicle);
                    return;
                }
                else
                {
                    _stateMachine.RequestState(AIVehicleState.Brake);
                    return;
                }
            }

            // 2. Verificação de Semáforos e Linhas de Retenção
            if (currentWaypoint != null && currentWaypoint.isStopLine)
            {
                TrafficSignal signal = currentWaypoint.AssociatedSignal;
                if (signal != null && signal.IsRed)
                {
                    float distToStop = Vector3.Distance(transform.position, currentWaypoint.Position);
                    float stopMargin = (driverProfile != null) ? driverProfile.stopLineMargin : 2.5f;

                    if (distToStop <= safeDistance + stopMargin)
                    {
                        _stateMachine.RequestState(AIVehicleState.StopAtSignal);
                        return;
                    }
                }
            }

            // 3. Verificação de Cruzamento à Frente
            if (TrafficNavigationManager.Instance != null)
            {
                TrafficIntersection intersectionAhead = TrafficNavigationManager.Instance.GetIntersectionAhead(transform.position, transform.forward, 20f);
                if (intersectionAhead != null)
                {
                    if (!intersectionAhead.CanEnterIntersection(transform, currentLane))
                    {
                        _stateMachine.RequestState(AIVehicleState.IntersectionWaiting);
                        return;
                    }
                }
            }

            // 4. Verificação de Faixa Interditada / Bloqueio Viário
            if (currentLane != null && currentLane.isBlocked && _stateMachine.CanInitiateLaneChange)
            {
                if (currentLane.LeftAdjacentLane != null && _sensor.IsLeftClear)
                {
                    InitiateLaneChange(currentLane.LeftAdjacentLane, false);
                    return;
                }
                else if (currentLane.RightAdjacentLane != null && _sensor.IsRightClear)
                {
                    InitiateLaneChange(currentLane.RightAdjacentLane, false);
                    return;
                }
                else
                {
                    _stateMachine.RequestState(AIVehicleState.Brake);
                    return;
                }
            }

            // 5. Manobra de Mudança de Faixa em Andamento
            if (_stateMachine.CurrentState == AIVehicleState.LaneChanging || _stateMachine.CurrentState == AIVehicleState.Overtaking)
            {
                return;
            }

            // 5. Retorno ao Cruzeiro Padrão
            _stateMachine.RequestState(AIVehicleState.Cruise);
        }

        /// <summary>
        /// Aplica controles de acelerador, freio e direção com base no estado ativo.
        /// </summary>
        private void ExecuteStateControls()
        {
            float targetSpeed = (currentLane != null) ? currentLane.speedLimitKmh : 60f;
            if (currentWaypoint != null && currentWaypoint.speedLimitKmh > 0f)
            {
                targetSpeed = Mathf.Min(targetSpeed, currentWaypoint.speedLimitKmh);
            }
            _motor.targetSpeedKmh = targetSpeed;

            switch (_stateMachine.CurrentState)
            {
                case AIVehicleState.Cruise:
                    _brake.ReleaseBrakes();
                    _motor.SetThrottle(1.0f);
                    break;

                case AIVehicleState.FollowVehicle:
                    _brake.ReleaseBrakes();
                    // Ajusta acelerador proporcional à velocidade do veículo à frente
                    float speedDiff = Mathf.Clamp01(_sensor.FrontHit.distance / (vehicleData != null ? vehicleData.baseSafetyDistance : 6f));
                    _motor.SetThrottle(speedDiff * 0.7f);
                    if (_sensor.FrontHit.distance < 4.0f)
                    {
                        _brake.SetBrake(0.4f);
                    }
                    break;

                case AIVehicleState.Brake:
                    _motor.StopThrottle();
                    _brake.SetBrake(0.5f);
                    break;

                case AIVehicleState.StopAtSignal:
                case AIVehicleState.IntersectionWaiting:
                    _motor.StopThrottle();
                    _brake.SetBrake(0.9f);
                    break;

                case AIVehicleState.EmergencyStop:
                    _motor.StopThrottle();
                    _brake.SetEmergencyStop();
                    break;

                case AIVehicleState.LaneChanging:
                case AIVehicleState.Overtaking:
                    _brake.ReleaseBrakes();
                    _motor.SetThrottle(1.1f); // Ganho leve de velocidade na manobra
                    UpdateLaneChangeProgress();
                    break;

                case AIVehicleState.Idle:
                default:
                    _motor.StopThrottle();
                    _brake.SetBrake(0.5f);
                    break;
            }
        }

        /// <summary>
        /// Inicia a transição lateral para uma faixa adjacente.
        /// </summary>
        public void InitiateLaneChange(TrafficLane targetLane, bool isOvertaking)
        {
            if (targetLane == null) return;

            targetAdjacentLane = targetLane;
            laneChangeProgress = 0f;
            _stateMachine.RequestState(isOvertaking ? AIVehicleState.Overtaking : AIVehicleState.LaneChanging, true);

            // Seleciona waypoint equivalente mais próximo na nova faixa
            var nextWp = targetAdjacentLane.GetNearestWaypoint(transform.position + transform.forward * 15f);
            if (nextWp != null)
            {
                _steering.SetTargetWaypoint(nextWp.transform);
            }
        }

        private void UpdateLaneChangeProgress()
        {
            if (targetAdjacentLane == null) return;

            laneChangeProgress += Time.deltaTime;
            var targetWp = targetAdjacentLane.GetNearestWaypoint(transform.position);

            if (targetWp != null)
            {
                float distToTargetLane = Vector3.Distance(transform.position, targetWp.Position);
                if (distToTargetLane < 2.0f || laneChangeProgress >= 4.0f)
                {
                    // Conclui a mudança de faixa
                    currentLane = targetAdjacentLane;
                    currentWaypoint = targetWp;
                    targetAdjacentLane = null;
                    _stateMachine.RequestState(AIVehicleState.Cruise, true);
                    _steering.SetTargetWaypoint(currentWaypoint.transform);
                }
            }
        }

        /// <summary>
        /// Avança o índice do waypoint ao alcançar o ponto atual.
        /// </summary>
        private void CheckWaypointProgression()
        {
            if (currentWaypoint == null || currentLane == null) return;

            float dist = Vector3.Distance(transform.position, currentWaypoint.Position);
            if (dist <= waypointArrivalDistance)
            {
                AdvanceToNextWaypoint();
            }
        }

        private void AdvanceToNextWaypoint()
        {
            if (currentLane == null || currentLane.Waypoints == null) return;

            currentWaypointIndex++;
            if (currentWaypointIndex < currentLane.Waypoints.Count)
            {
                currentWaypoint = currentLane.Waypoints[currentWaypointIndex];
            }
            else
            {
                // Fim da faixa: se houver continuidade topológica
                if (currentWaypoint != null && currentWaypoint.NextWaypoints != null && currentWaypoint.NextWaypoints.Count > 0)
                {
                    // Seleciona próximo waypoint (ou ramificação)
                    currentWaypoint = currentWaypoint.NextWaypoints[0];
                    if (currentWaypoint.ParentLane != null)
                    {
                        currentLane = currentWaypoint.ParentLane;
                        currentWaypointIndex = 0;
                    }
                }
                else
                {
                    // Loop de retorno ou reinício
                    currentWaypointIndex = 0;
                    currentWaypoint = currentLane.Waypoints[0];
                }
            }

            if (currentWaypoint != null)
            {
                _steering.SetTargetWaypoint(currentWaypoint.transform);
            }
        }

        /// <summary>
        /// Reinicia o veículo para reinserção na cena a partir de um pool.
        /// </summary>
        public void ResetVehicle(Vector3 spawnPosition, Quaternion spawnRotation, TrafficLane initialLane)
        {
            transform.position = spawnPosition;
            transform.rotation = spawnRotation;

            if (_rb != null)
            {
                _rb.linearVelocity = Vector3.zero;
                _rb.angularVelocity = Vector3.zero;
            }

            _stateMachine.ResetStateMachine(AIVehicleState.Cruise);
            _brake.ReleaseBrakes();
            _motor.StopThrottle();

            SetRoute(initialLane, 0);
        }
    }
}
