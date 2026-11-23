using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjetoGrau2.AI
{
    /// <summary>
    /// Gerenciador de Object Pooling para veículos civis NPC.
    /// Pré-aloca instâncias reutilizáveis de cada categoria de veículo para garantir Zero GC
    /// e taxa de quadros de 60 FPS estáveis em dispositivos móveis Android.
    /// Cria representações proxy procedurais (BoxCollider + MeshFilter/Renderer primitivos)
    /// na ausência de malhas 3D proprietárias.
    /// </summary>
    [AddComponentMenu("Projeto Grau 2/AI/AI Vehicle Pool")]
    public class AIVehiclePool : MonoBehaviour
    {
        public static AIVehiclePool Instance { get; private set; }

        [Header("Configuração de Pré-Alocação")]
        [Tooltip("Quantidade de instâncias pré-alocadas por tipo de veículo.")]
        [Range(1, 10)] public int poolSizePerType = 3;

        [Header("Dados das Categorias Disponíveis")]
        [SerializeField] private List<AIVehicleData> vehicleDataCatalog = new List<AIVehicleData>();
        [SerializeField] private List<AIDriverProfile> driverProfileCatalog = new List<AIDriverProfile>();

        // Filas de pooling indexadas por tipo de veículo
        private readonly Dictionary<AIVehicleType, Queue<AIVehicleController>> _inactivePool = new Dictionary<AIVehicleType, Queue<AIVehicleController>>();
        private readonly List<AIVehicleController> _activeVehicles = new List<AIVehicleController>();

        public IReadOnlyList<AIVehicleController> ActiveVehicles => _activeVehicles;
        public int ActiveCount => _activeVehicles.Count;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            InitializePools();
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void InitializePools()
        {
            foreach (AIVehicleType vType in Enum.GetValues(typeof(AIVehicleType)))
            {
                _inactivePool[vType] = new Queue<AIVehicleController>();
                AIVehicleData data = GetVehicleDataForType(vType);

                for (int i = 0; i < poolSizePerType; i++)
                {
                    AIVehicleController vehicle = CreateProxyVehicleInstance(vType, data);
                    vehicle.gameObject.SetActive(false);
                    vehicle.transform.SetParent(transform);
                    _inactivePool[vType].Enqueue(vehicle);
                }
            }
        }

        /// <summary>
        /// Obtém um veículo do pool por tipo. Se o pool estiver vazio, cria uma nova instância proxy.
        /// </summary>
        public AIVehicleController GetVehicle(AIVehicleType vType)
        {
            Queue<AIVehicleController> queue;
            if (!_inactivePool.TryGetValue(vType, out queue))
            {
                queue = new Queue<AIVehicleController>();
                _inactivePool[vType] = queue;
            }

            AIVehicleController vehicle;
            if (queue.Count > 0)
            {
                vehicle = queue.Dequeue();
            }
            else
            {
                vehicle = CreateProxyVehicleInstance(vType, GetVehicleDataForType(vType));
            }

            vehicle.gameObject.SetActive(true);
            _activeVehicles.Add(vehicle);
            return vehicle;
        }

        /// <summary>
        /// Devolve um veículo ao pool, desativando-o e limpando suas forças.
        /// </summary>
        public void ReturnVehicle(AIVehicleController vehicle)
        {
            if (vehicle == null) return;

            _activeVehicles.Remove(vehicle);
            vehicle.gameObject.SetActive(false);
            vehicle.transform.SetParent(transform);

            AIVehicleType vType = (vehicle.vehicleData != null) ? vehicle.vehicleData.vehicleType : AIVehicleType.EconomyCar;
            if (_inactivePool.ContainsKey(vType))
            {
                _inactivePool[vType].Enqueue(vehicle);
            }
        }

        /// <summary>
        /// Constrói um GameObject proxy completo com Rigidbody, BoxCollider e componentes de IA.
        /// </summary>
        private AIVehicleController CreateProxyVehicleInstance(AIVehicleType vType, AIVehicleData data)
        {
            GameObject go = new GameObject($"NPC_Vehicle_{vType}");
            go.layer = LayerMask.NameToLayer("Default");

            // 1. Rigidbody
            Rigidbody rb = go.AddComponent<Rigidbody>();
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.Discrete;

            // 2. BoxCollider Proxy
            BoxCollider col = go.AddComponent<BoxCollider>();
            if (data != null)
            {
                rb.mass = data.mass;
                rb.linearDamping = data.airDrag;
                rb.angularDamping = data.angularDrag;
                col.size = data.colliderSize;
                col.center = data.colliderCenter;
            }
            else
            {
                rb.mass = 1200f;
                col.size = new Vector3(1.7f, 1.4f, 4.0f);
                col.center = new Vector3(0f, 0.7f, 0f);
            }

            // 3. Representação Visual Primitiva (Cubo Proxy)
            GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Cube);
            visual.name = "Visual_Proxy_Chassis";
            visual.transform.SetParent(go.transform);
            visual.transform.localPosition = col.center;
            visual.transform.localRotation = Quaternion.identity;
            visual.transform.localScale = col.size;
            // Remove o colisor do cubo filho para não duplicar colisores
            Collider childCol = visual.GetComponent<Collider>();
            if (childCol != null) Destroy(childCol);

            // 4. Componentes de IA
            var sm = go.AddComponent<AIVehicleStateMachine>();
            var sensor = go.AddComponent<AIVehicleSensor>();
            var motor = go.AddComponent<AIVehicleMotorController>();
            var steering = go.AddComponent<AIVehicleSteeringController>();
            var brake = go.AddComponent<AIVehicleBrakeController>();
            var ctrl = go.AddComponent<AIVehicleController>();

            // Vínculos
            ctrl.vehicleData = data;
            ctrl.driverProfile = GetRandomDriverProfile();
            steering.Initialize(data, visual.transform);
            ctrl.InitializeComponents();

            return ctrl;
        }

        private AIVehicleData GetVehicleDataForType(AIVehicleType vType)
        {
            if (vehicleDataCatalog != null)
            {
                foreach (var d in vehicleDataCatalog)
                {
                    if (d != null && d.vehicleType == vType) return d;
                }
            }
            return null;
        }

        private AIDriverProfile GetRandomDriverProfile()
        {
            if (driverProfileCatalog != null && driverProfileCatalog.Count > 0)
            {
                int idx = UnityEngine.Random.Range(0, driverProfileCatalog.Count);
                return driverProfileCatalog[idx];
            }
            return null;
        }
    }
}
