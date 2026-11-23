using System;
using UnityEngine;
using ProjetoGrau2.Core;
using ProjetoGrau2.Traffic;
using ProjetoGrau2.AI;

namespace ProjetoGrau2.Police
{
    /// <summary>
    /// Controlador central de inteligência artificial policial e perseguição.
    /// Governa a percepção sensorial (visão/audição), condução física tática, cálculo de rotas,
    /// interceptação à frente do infrator, busca por perda de contato e consumação da abordagem/prisão.
    /// </summary>
    [AddComponentMenu("Projeto Grau 2/Police/Police Pursuit Controller")]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(PoliceAIStateMachine))]
    [RequireComponent(typeof(AIVehicleMotorController))]
    [RequireComponent(typeof(AIVehicleSteeringController))]
    [RequireComponent(typeof(AIVehicleBrakeController))]
    [RequireComponent(typeof(AIVehicleSensor))]
    public class PolicePursuitController : MonoBehaviour
    {
        [Header("Configurações de Dados Policiais")]
        public PoliceVehicleData vehicleData;
        public PoliceAIData aiData;

        [Header("Alvo de Perseguição")]
        [SerializeField] private MotorcycleController targetPlayer;
        [SerializeField] private Vector3 lastKnownPosition;
        [SerializeField] private bool hasVisualContact = false;

        [Header("Patrulhamento Padrão")]
        [SerializeField] private TrafficLane patrolLane;
        [SerializeField] private TrafficWaypoint patrolWaypoint;
        [SerializeField] private int patrolWaypointIndex = 0;

        [Header("Estado de Abordagem / Prisão")]
        [SerializeField] private float currentArrestTimer = 0f;
        [SerializeField] private float lostTargetTimer = 0f;

        [Header("Sinalização Visual (Giroflex / Sirene)")]
        [SerializeField] private Light sirenRedLight;
        [SerializeField] private Light sirenBlueLight;
        public bool isSirenActive = false;

        // Subcomponentes
        private Rigidbody _rb;
        private PoliceAIStateMachine _stateMachine;
        private AIVehicleMotorController _motor;
        private AIVehicleSteeringController _steering;
        private AIVehicleBrakeController _brake;
        private AIVehicleSensor _sensor;

        private static readonly RaycastHit[] _visionBuffer = new RaycastHit[4];
        private Collider[] _ownColliders;

        // Eventos
        public event Action<PolicePursuitController> OnPursuitInitiated;
        public event Action<PolicePursuitController> OnPlayerBusted;
        public event Action<PolicePursuitController, bool> OnVisualContactChanged;

        public PoliceAIState CurrentState => _stateMachine != null ? _stateMachine.CurrentState : PoliceAIState.Patrol;
        public bool HasVisualContact => hasVisualContact;
        public Vector3 LastKnownPosition => lastKnownPosition;
        public float ArrestProgress => (aiData != null && aiData.arrestDurationRequired > 0f) ? Mathf.Clamp01(currentArrestTimer / aiData.arrestDurationRequired) : 0f;
        public PoliceTacticalRole CurrentTacticalRole => (PolicePursuitCoordinator.Instance != null) ? PolicePursuitCoordinator.Instance.GetUnitRole(this) : PoliceTacticalRole.None;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _stateMachine = GetComponent<PoliceAIStateMachine>();
            _motor = GetComponent<AIVehicleMotorController>();
            _steering = GetComponent<AIVehicleSteeringController>();
            _brake = GetComponent<AIVehicleBrakeController>();
            _sensor = GetComponent<AIVehicleSensor>();
            _ownColliders = GetComponentsInChildren<Collider>(true);

            FindTargetPlayer();
            InitializeVehiclePhysics();
        }

        private void Start()
        {
            if (PolicePursuitCoordinator.Instance != null)
            {
                PolicePursuitCoordinator.Instance.RegisterUnit(this);
            }
        }

        private void OnDestroy()
        {
            if (PolicePursuitCoordinator.Instance != null)
            {
                PolicePursuitCoordinator.Instance.UnregisterUnit(this);
            }
        }

        private void InitializeVehiclePhysics()
        {
            if (vehicleData == null) return;

            _rb.mass = vehicleData.mass;
            _rb.linearDamping = vehicleData.airDrag;
            _rb.angularDamping = vehicleData.angularDrag;

            // Cria AIVehicleData adaptado em tempo de execução para os controladores físicos
            var adaptedData = ScriptableObject.CreateInstance<AIVehicleData>();
            adaptedData.mass = vehicleData.mass;
            adaptedData.airDrag = vehicleData.airDrag;
            adaptedData.angularDrag = vehicleData.angularDrag;
            adaptedData.colliderSize = vehicleData.colliderSize;
            adaptedData.colliderCenter = vehicleData.colliderCenter;
            adaptedData.wheelbase = vehicleData.wheelbase;
            adaptedData.topSpeedKmh = vehicleData.topSpeedKmh;
            adaptedData.accelerationForce = vehicleData.accelerationForce;
            adaptedData.brakeForce = vehicleData.brakeForce;
            adaptedData.reverseForce = vehicleData.accelerationForce * 0.4f;
            adaptedData.maxSteerAngle = vehicleData.maxSteerAngle;
            adaptedData.steerSpeed = vehicleData.steerSpeed;
            adaptedData.highSpeedSteerScale = vehicleData.highSpeedSteerScale;
            adaptedData.isTwoWheeled = vehicleData.isTwoWheeled;
            adaptedData.maxLeanAngle = vehicleData.maxLeanAngle;
            adaptedData.leanSpeed = vehicleData.leanSpeed;
            adaptedData.baseSafetyDistance = 5.0f;
            adaptedData.sensorForwardRange = (aiData != null) ? aiData.visionRange : 50f;

            _motor.Initialize(adaptedData, null);
            _steering.Initialize(adaptedData);
            _brake.Initialize(adaptedData, null);
        }

        private void FindTargetPlayer()
        {
            if (targetPlayer == null)
            {
                targetPlayer = FindFirstObjectByType<MotorcycleController>();
            }
        }

        public void SetPatrolRoute(TrafficLane lane, int startWaypointIndex = 0)
        {
            patrolLane = lane;
            patrolWaypointIndex = startWaypointIndex;
            if (patrolLane != null && patrolLane.Waypoints != null && patrolLane.Waypoints.Count > 0)
            {
                patrolWaypointIndex = Mathf.Clamp(startWaypointIndex, 0, patrolLane.Waypoints.Count - 1);
                patrolWaypoint = patrolLane.Waypoints[patrolWaypointIndex];
                if (patrolWaypoint != null)
                {
                    _steering.SetTargetWaypoint(patrolWaypoint.transform);
                }
            }
        }

        private void Update()
        {
            float dt = Time.deltaTime;
            _stateMachine.Tick(dt);
            _sensor.TickSensors(dt);

            UpdatePerception();
            EvaluatePoliceTactics(dt);
            ExecutePoliceControls();
            UpdateSirenLights(dt);
        }

        /// <summary>
        /// Varredura sensorial de visão e audição em relação ao jogador.
        /// </summary>
        private void UpdatePerception()
        {
            if (targetPlayer == null || aiData == null) return;

            Vector3 toPlayer = targetPlayer.transform.position - transform.position;
            float dist = toPlayer.magnitude;

            bool wasInContact = hasVisualContact;
            hasVisualContact = false;

            if (dist <= aiData.visionRange)
            {
                float angle = Vector3.Angle(transform.forward, toPlayer.normalized);
                if (angle <= aiData.visionFieldOfView * 0.5f)
                {
                    // Checagem de linha de visada desobstruída (Raycast)
                    Vector3 rayOrigin = transform.position + Vector3.up * 1.0f;
                    Vector3 targetHead = targetPlayer.transform.position + Vector3.up * 1.0f;
                    Vector3 dir = (targetHead - rayOrigin).normalized;

                    int hits = Physics.RaycastNonAlloc(rayOrigin, dir, _visionBuffer, dist, ~0, QueryTriggerInteraction.Ignore);
                    bool obstructed = false;

                    for (int i = 0; i < hits; i++)
                    {
                        var hitCol = _visionBuffer[i].collider;
                        if (IsOwnCollider(hitCol)) continue;

                        // Se o colisor atingido for do jogador, a linha de visada está livre
                        if (hitCol.transform.root == targetPlayer.transform.root)
                        {
                            break;
                        }
                        else
                        {
                            obstructed = true;
                            break;
                        }
                    }

                    if (!obstructed)
                    {
                        hasVisualContact = true;
                        lastKnownPosition = targetPlayer.transform.position;
                    }
                }
            }

            if (wasInContact != hasVisualContact)
            {
                OnVisualContactChanged?.Invoke(this, hasVisualContact);
                if (WantedLevelManager.Instance != null && hasVisualContact)
                {
                    WantedLevelManager.Instance.SetPlayerSpotted(true);
                }

                if (PolicePursuitCoordinator.Instance != null)
                {
                    if (hasVisualContact)
                    {
                        PolicePursuitCoordinator.Instance.ReportVisualContact(this, lastKnownPosition);
                    }
                    else
                    {
                        PolicePursuitCoordinator.Instance.ReportVisualLost(this);
                    }
                }
            }
            else if (!hasVisualContact && PolicePursuitCoordinator.Instance != null && PolicePursuitCoordinator.Instance.HasAnyVisualContact)
            {
                lastKnownPosition = PolicePursuitCoordinator.Instance.GlobalLastKnownPosition;
            }
        }

        private bool IsOwnCollider(Collider col)
        {
            if (col == null || _ownColliders == null) return false;
            for (int i = 0; i < _ownColliders.Length; i++)
            {
                if (_ownColliders[i] == col) return true;
            }
            return false;
        }

        /// <summary>
        /// Avalia táticas de perseguição, transições da máquina de estados e abordagem.
        /// </summary>
        private void EvaluatePoliceTactics(float dt)
        {
            int wantedLevel = (WantedLevelManager.Instance != null) ? WantedLevelManager.Instance.CurrentWantedLevel : 0;
            float distToPlayer = (targetPlayer != null) ? Vector3.Distance(transform.position, targetPlayer.transform.position) : float.MaxValue;

            switch (_stateMachine.CurrentState)
            {
                case PoliceAIState.Patrol:
                    isSirenActive = false;
                    currentArrestTimer = 0f;
                    lostTargetTimer = 0f;

                    // Se o jogador estiver procurado e no campo de visão, inicia perseguição
                    if (wantedLevel >= 1 && hasVisualContact)
                    {
                        _stateMachine.RequestState(wantedLevel >= 3 ? PoliceAIState.Pursuit : PoliceAIState.Investigating);
                        OnPursuitInitiated?.Invoke(this);
                    }
                    else if (PolicePursuitCoordinator.Instance != null && PolicePursuitCoordinator.Instance.IsPursuitActive)
                    {
                        var role = PolicePursuitCoordinator.Instance.GetUnitRole(this);
                        if (role != PoliceTacticalRole.None)
                        {
                            _stateMachine.RequestState(PoliceAIState.Pursuit);
                            OnPursuitInitiated?.Invoke(this);
                        }
                    }
                    break;

                case PoliceAIState.Suspicious:
                case PoliceAIState.Investigating:
                    isSirenActive = false;
                    if (hasVisualContact)
                    {
                        if (wantedLevel >= 2)
                        {
                            _stateMachine.RequestState(PoliceAIState.Pursuit);
                        }
                        else if (distToPlayer <= aiData.arrestDistance)
                        {
                            _stateMachine.RequestState(PoliceAIState.Arresting);
                        }
                    }
                    else if (_stateMachine.TimeInCurrentState > 6.0f)
                    {
                        _stateMachine.RequestState(PoliceAIState.ReturningToPatrol);
                    }
                    break;

                case PoliceAIState.Pursuit:
                    isSirenActive = true;
                    lostTargetTimer = 0f;

                    if (hasVisualContact)
                    {
                        // Se estiver próximo e em alta velocidade na mesma faixa, pode tentar interceptar
                        if (distToPlayer < 20f && distToPlayer > aiData.arrestDistance)
                        {
                            _stateMachine.RequestState(PoliceAIState.Intercepting);
                        }
                        else if (distToPlayer <= aiData.arrestDistance && targetPlayer.CurrentSpeedKmh <= aiData.playerMaxArrestSpeedKmh)
                        {
                            _stateMachine.RequestState(PoliceAIState.Arresting);
                        }
                    }
                    else
                    {
                        // Perdeu contato visual: transita para LostTarget
                        _stateMachine.RequestState(PoliceAIState.LostTarget);
                    }
                    break;

                case PoliceAIState.Intercepting:
                    isSirenActive = true;
                    if (!hasVisualContact)
                    {
                        _stateMachine.RequestState(PoliceAIState.LostTarget);
                    }
                    else if (distToPlayer <= aiData.arrestDistance && targetPlayer.CurrentSpeedKmh <= aiData.playerMaxArrestSpeedKmh)
                    {
                        _stateMachine.RequestState(PoliceAIState.Arresting);
                    }
                    break;

                case PoliceAIState.Arresting:
                    isSirenActive = true;
                    if (targetPlayer != null && distToPlayer <= aiData.arrestDistance + 2f)
                    {
                        if (targetPlayer.CurrentSpeedKmh <= aiData.playerMaxArrestSpeedKmh)
                        {
                            currentArrestTimer += dt;
                            if (currentArrestTimer >= aiData.arrestDurationRequired)
                            {
                                // Prisão consumada (Busted)
                                OnPlayerBusted?.Invoke(this);
                                if (WantedLevelManager.Instance != null)
                                {
                                    WantedLevelManager.Instance.ClearWantedLevel();
                                }
                                if (PolicePursuitCoordinator.Instance != null)
                                {
                                    PolicePursuitCoordinator.Instance.ReportBusted(this);
                                }
                                _stateMachine.RequestState(PoliceAIState.Patrol, true);
                            }
                        }
                        else
                        {
                            // Jogador acelerou para fugir da abordagem
                            currentArrestTimer = 0f;
                            if (TrafficViolationDetector.Instance != null)
                            {
                                TrafficViolationDetector.Instance.ReportEvadingOrder();
                            }
                            _stateMachine.RequestState(PoliceAIState.Pursuit, true);
                        }
                    }
                    else
                    {
                        currentArrestTimer = 0f;
                        _stateMachine.RequestState(PoliceAIState.Pursuit, true);
                    }
                    break;

                case PoliceAIState.LostTarget:
                    isSirenActive = true;
                    lostTargetTimer += dt;

                    // Se reencontrar o jogador durante a busca
                    if (hasVisualContact)
                    {
                        _stateMachine.RequestState(PoliceAIState.Pursuit, true);
                        return;
                    }

                    if (lostTargetTimer >= aiData.lostTargetTimeout)
                    {
                        // Desiste da busca e volta à patrulha normal
                        if (WantedLevelManager.Instance != null)
                        {
                            WantedLevelManager.Instance.SetPlayerSpotted(false);
                        }
                        _stateMachine.RequestState(PoliceAIState.ReturningToPatrol);
                    }
                    break;

                case PoliceAIState.ReturningToPatrol:
                    isSirenActive = false;
                    currentArrestTimer = 0f;
                    lostTargetTimer = 0f;
                    _stateMachine.RequestState(PoliceAIState.Patrol);
                    break;
            }
        }

        /// <summary>
        /// Condução física do veículo (acelerador, freio, esterçamento) baseada no estado.
        /// </summary>
        private void ExecutePoliceControls()
        {
            switch (_stateMachine.CurrentState)
            {
                case PoliceAIState.Patrol:
                case PoliceAIState.ReturningToPatrol:
                    _brake.ReleaseBrakes();
                    float limit = (patrolLane != null) ? patrolLane.speedLimitKmh : 50f;
                    _motor.targetSpeedKmh = limit;
                    _motor.SetThrottle(0.85f);
                    UpdatePatrolWaypointFollowing();
                    break;

                case PoliceAIState.Investigating:
                    _brake.ReleaseBrakes();
                    _motor.targetSpeedKmh = 70f;
                    _motor.SetThrottle(0.9f);
                    if (targetPlayer != null)
                    {
                        _steering.SetTargetPosition(targetPlayer.transform.position);
                    }
                    break;

                case PoliceAIState.Pursuit:
                    _brake.ReleaseBrakes();
                    float topPursuit = (vehicleData != null) ? vehicleData.topSpeedKmh : 180f;
                    _motor.targetSpeedKmh = topPursuit;
                    _motor.SetThrottle(1.0f * (vehicleData != null ? vehicleData.pursuitBoostMultiplier : 1.2f));

                    if (PolicePursuitCoordinator.Instance != null && PolicePursuitCoordinator.Instance.IsPursuitActive)
                    {
                        _steering.SetTargetPosition(PolicePursuitCoordinator.Instance.CalculateTacticalTargetPosition(this));
                    }
                    else if (targetPlayer != null)
                    {
                        _steering.SetTargetPosition(targetPlayer.transform.position);
                    }
                    break;

                case PoliceAIState.Intercepting:
                    _brake.ReleaseBrakes();
                    _motor.targetSpeedKmh = (vehicleData != null) ? vehicleData.topSpeedKmh : 180f;
                    _motor.SetThrottle(1.1f);

                    if (PolicePursuitCoordinator.Instance != null && PolicePursuitCoordinator.Instance.IsPursuitActive)
                    {
                        _steering.SetTargetPosition(PolicePursuitCoordinator.Instance.CalculateTacticalTargetPosition(this));
                    }
                    else if (targetPlayer != null)
                    {
                        // Mira à frente do infrator para cortar caminho
                        Vector3 predictedPos = targetPlayer.transform.position + targetPlayer.transform.forward * (targetPlayer.CurrentSpeedKmh / 3.6f * aiData.interceptLookAheadTime);
                        _steering.SetTargetPosition(predictedPos);
                    }
                    break;

                case PoliceAIState.Arresting:
                    _motor.StopThrottle();
                    _brake.SetBrake(1.0f);
                    break;

                case PoliceAIState.LostTarget:
                    _brake.ReleaseBrakes();
                    _motor.targetSpeedKmh = 45f;
                    _motor.SetThrottle(0.7f);
                    if (PolicePursuitCoordinator.Instance != null && PolicePursuitCoordinator.Instance.IsPursuitActive)
                    {
                        _steering.SetTargetPosition(PolicePursuitCoordinator.Instance.GetSearchTargetForUnit(this));
                    }
                    else
                    {
                        // Navega até a última posição conhecida
                        _steering.SetTargetPosition(lastKnownPosition);
                    }
                    break;
            }
        }

        private void UpdatePatrolWaypointFollowing()
        {
            if (patrolWaypoint == null || patrolLane == null) return;

            float dist = Vector3.Distance(transform.position, patrolWaypoint.Position);
            if (dist <= 5.0f)
            {
                patrolWaypointIndex++;
                if (patrolLane.Waypoints != null && patrolWaypointIndex < patrolLane.Waypoints.Count)
                {
                    patrolWaypoint = patrolLane.Waypoints[patrolWaypointIndex];
                }
                else
                {
                    patrolWaypointIndex = 0;
                    patrolWaypoint = (patrolLane.Waypoints != null && patrolLane.Waypoints.Count > 0) ? patrolLane.Waypoints[0] : null;
                }

                if (patrolWaypoint != null)
                {
                    _steering.SetTargetWaypoint(patrolWaypoint.transform);
                }
            }
        }

        private void UpdateSirenLights(float dt)
        {
            if (!isSirenActive)
            {
                if (sirenRedLight != null) sirenRedLight.enabled = false;
                if (sirenBlueLight != null) sirenBlueLight.enabled = false;
                return;
            }

            // Alternância estroboscópica rápida (6 Hz)
            bool toggle = ((int)(Time.time * 6.0f) % 2) == 0;
            if (sirenRedLight != null) sirenRedLight.enabled = toggle;
            if (sirenBlueLight != null) sirenBlueLight.enabled = !toggle;
        }

        private void OnDrawGizmosSelected()
        {
            if (aiData == null) return;

            // Cone de Visão
            Gizmos.color = hasVisualContact ? Color.red : new Color(0f, 0.5f, 1f, 0.4f);
            Vector3 eyePos = transform.position + Vector3.up * 1.0f;
            Gizmos.DrawWireSphere(eyePos, aiData.visionRange);

            // Última Posição Conhecida (LKP)
            if (_stateMachine != null && _stateMachine.CurrentState == PoliceAIState.LostTarget)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(lastKnownPosition, aiData.searchRadius);
                Gizmos.DrawLine(eyePos, lastKnownPosition);
            }
        }
    }
}
