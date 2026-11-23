using System;
using UnityEngine;

namespace ProjetoGrau2.AI
{
    /// <summary>
    /// Dados consolidados de detecção de obstáculo frontal.
    /// </summary>
    public struct SensorHitInfo
    {
        public bool hasHit;
        public float distance;
        public Vector3 point;
        public Vector3 normal;
        public Transform hitTransform;
        public Rigidbody hitRigidbody;
        public bool isPlayer;
        public float relativeSpeedKmh;
    }

    /// <summary>
    /// Sistema de percepção sensorial para veículos NPC.
    /// Utiliza SphereCast e Raycasts não-alocativos (Zero GC) para varredura frontal,
    /// lateral (pontos cegos para ultrapassagem) e traseira com frequência adaptativa.
    /// </summary>
    [AddComponentMenu("Projeto Grau 2/AI/AI Vehicle Sensor")]
    public class AIVehicleSensor : MonoBehaviour
    {
        [Header("Configuração de Camadas")]
        [Tooltip("Camadas de colisão detectadas como veículos, jogadores ou obstáculos.")]
        public LayerMask detectionLayers = ~0;

        [Header("Parâmetros do Sensor Frontal")]
        [Range(10f, 80f)] public float forwardRange = 35f;
        [Range(0.2f, 2.0f)] public float sphereCastRadius = 0.8f;
        [Tooltip("Deslocamento vertical do ponto de emissão dos raios (altura do capô/farol).")]
        public float sensorHeight = 0.65f;

        [Header("Sensores Laterais (Mudança de Faixa / Ponto Cego)")]
        [Range(2f, 15f)] public float sideRayDistance = 4.5f;
        [Range(5f, 30f)] public float blindSpotRange = 12.0f;

        [Header("Frequência de Varredura Adaptativa")]
        [Tooltip("Intervalo entre leituras de sensores em segundos (modulado por LOD).")]
        [Range(0.02f, 1.0f)] public float updateInterval = 0.05f; // Padrão: 20 Hz (LOD 0)

        // Estado das leituras
        public SensorHitInfo FrontHit { get; private set; }
        public bool IsLeftClear { get; private set; } = true;
        public bool IsRightClear { get; private set; } = true;
        public bool IsRearClear { get; private set; } = true;

        private float _sensorTimer = 0f;
        private Rigidbody _ownRigidbody;
        private Collider[] _ownColliders;

        // Buffers estáticos para chamadas NonAlloc (Zero GC)
        private static readonly RaycastHit[] _raycastBuffer = new RaycastHit[8];

        private void Awake()
        {
            _ownRigidbody = GetComponent<Rigidbody>();
            _ownColliders = GetComponentsInChildren<Collider>(true);
            FrontHit = default;
        }

        public void SetUpdateInterval(float interval)
        {
            updateInterval = Mathf.Max(0.02f, interval);
        }

        public void TickSensors(float deltaTime)
        {
            _sensorTimer += deltaTime;
            if (_sensorTimer >= updateInterval)
            {
                _sensorTimer = 0f;
                PerformSensorScans();
            }
        }

        private void PerformSensorScans()
        {
            ScanFront();
            ScanSides();
        }

        private void ScanFront()
        {
            Vector3 origin = transform.position + Vector3.up * sensorHeight;
            Vector3 direction = transform.forward;

            int hitCount = Physics.SphereCastNonAlloc(
                origin,
                sphereCastRadius,
                direction,
                _raycastBuffer,
                forwardRange,
                detectionLayers,
                QueryTriggerInteraction.Ignore
            );

            SensorHitInfo closestHit = default;
            float minDistance = float.MaxValue;

            for (int i = 0; i < hitCount; i++)
            {
                RaycastHit hit = _raycastBuffer[i];
                if (IsOwnCollider(hit.collider)) continue;

                if (hit.distance < minDistance)
                {
                    minDistance = hit.distance;
                    closestHit.hasHit = true;
                    closestHit.distance = hit.distance;
                    closestHit.point = hit.point;
                    closestHit.normal = hit.normal;
                    closestHit.hitTransform = hit.transform;
                    closestHit.hitRigidbody = hit.rigidbody;

                    // Verifica se o alvo é o jogador (pela tag ou componente)
                    closestHit.isPlayer = hit.transform.CompareTag("Player") ||
                                          hit.transform.name.IndexOf("Titan", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                          hit.transform.name.IndexOf("Motorcycle", StringComparison.OrdinalIgnoreCase) >= 0;

                    // Cálculo da velocidade relativa longitudinal
                    if (hit.rigidbody != null && _ownRigidbody != null)
                    {
                        Vector3 relativeVel = _ownRigidbody.linearVelocity - hit.rigidbody.linearVelocity;
                        closestHit.relativeSpeedKmh = Vector3.Dot(relativeVel, direction) * 3.6f;
                    }
                    else
                    {
                        closestHit.relativeSpeedKmh = (_ownRigidbody != null) ? _ownRigidbody.linearVelocity.magnitude * 3.6f : 0f;
                    }
                }
            }

            FrontHit = closestHit;
        }

        private void ScanSides()
        {
            Vector3 origin = transform.position + Vector3.up * sensorHeight;

            // Lateral Esquerda (Raio lateral + Raio de Ponto Cego Diagonal Traseiro)
            bool leftHit = CheckDirection(origin, -transform.right, sideRayDistance) ||
                           CheckDirection(origin, (-transform.right - transform.forward * 0.7f).normalized, blindSpotRange);

            // Lateral Direita
            bool rightHit = CheckDirection(origin, transform.right, sideRayDistance) ||
                            CheckDirection(origin, (transform.right - transform.forward * 0.7f).normalized, blindSpotRange);

            // Traseira
            bool rearHit = CheckDirection(origin, -transform.forward, sideRayDistance * 1.5f);

            IsLeftClear = !leftHit;
            IsRightClear = !rightHit;
            IsRearClear = !rearHit;
        }

        private bool CheckDirection(Vector3 origin, Vector3 dir, float distance)
        {
            int hits = Physics.RaycastNonAlloc(origin, dir, _raycastBuffer, distance, detectionLayers, QueryTriggerInteraction.Ignore);
            for (int i = 0; i < hits; i++)
            {
                if (!IsOwnCollider(_raycastBuffer[i].collider))
                {
                    return true;
                }
            }
            return false;
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

        private void OnDrawGizmosSelected()
        {
            Vector3 origin = transform.position + Vector3.up * sensorHeight;

            // Raio Frontal
            if (FrontHit.hasHit)
            {
                Gizmos.color = FrontHit.isPlayer ? Color.magenta : Color.red;
                Gizmos.DrawLine(origin, origin + transform.forward * FrontHit.distance);
                Gizmos.DrawWireSphere(origin + transform.forward * FrontHit.distance, sphereCastRadius);
            }
            else
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(origin, origin + transform.forward * forwardRange);
                Gizmos.DrawWireSphere(origin + transform.forward * forwardRange, sphereCastRadius);
            }

            // Raios Laterais
            Gizmos.color = IsLeftClear ? Color.cyan : Color.red;
            Gizmos.DrawLine(origin, origin - transform.right * sideRayDistance);

            Gizmos.color = IsRightClear ? Color.cyan : Color.red;
            Gizmos.DrawLine(origin, origin + transform.right * sideRayDistance);
        }
    }
}
