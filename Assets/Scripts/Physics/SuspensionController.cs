using UnityEngine;
using ProjetoGrau2.Data;

namespace ProjetoGrau2.Physics
{
    /// <summary>
    /// Controlador físico da suspensão e molejo dianteiro e traseiro.
    /// Implementa equações clássicas massa-mola-amortecedor em FixedUpdate (60 Hz),
    /// gerando transferência de peso, absorção de aterrissagem do grau e mergulho de frenagem.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class SuspensionController : MonoBehaviour
    {
        [Header("Configuração de Dados")]
        [SerializeField] private SuspensionPreset preset;

        [Header("Pontos de Fixação no Chassi")]
        [SerializeField] private Transform frontForkMount;
        [SerializeField] private Transform rearSwingarmMount;

        [Header("Transformações Visuais das Rodas")]
        [SerializeField] private Transform frontWheelVisual;
        [SerializeField] private Transform rearWheelVisual;
        [SerializeField] private float tireRadius = 0.32f;
        [SerializeField] private LayerMask groundLayer = ~0;

        private Rigidbody rb;
        private float frontPrevCompression = 0f;
        private float rearPrevCompression = 0f;

        private bool isFrontGrounded = false;
        private bool isRearGrounded = false;

        public SuspensionPreset Preset => preset;
        public bool IsFrontGrounded => isFrontGrounded;
        public bool IsRearGrounded => isRearGrounded;

        public void Initialize(SuspensionPreset newPreset)
        {
            preset = newPreset;
        }

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            if (preset == null) return;

            SimulateWheelSuspension(
                frontForkMount,
                frontWheelVisual,
                preset.frontRestLength + preset.rideHeightOffset,
                preset.frontSpringStiffness,
                preset.frontDamping,
                ref frontPrevCompression,
                out isFrontGrounded
            );

            SimulateWheelSuspension(
                rearSwingarmMount,
                rearWheelVisual,
                preset.rearRestLength + preset.rideHeightOffset,
                preset.rearSpringStiffness,
                preset.rearDamping,
                ref rearPrevCompression,
                out isRearGrounded
            );
        }

        private void SimulateWheelSuspension(
            Transform mountPoint,
            Transform wheelVisual,
            float restLength,
            float stiffness,
            float damping,
            ref float prevCompression,
            out bool isGrounded)
        {
            isGrounded = false;
            if (mountPoint == null) return;

            Vector3 origin = mountPoint.position;
            float maxRayDistance = restLength + tireRadius;

            // Raio descendente em direção ao solo
            if (UnityEngine.Physics.Raycast(origin, -transform.up, out RaycastHit hit, maxRayDistance, groundLayer))
            {
                isGrounded = true;
                float currentLength = hit.distance - tireRadius;
                float compression = restLength - currentLength;

                if (compression > 0f)
                {
                    // Velocidade de compressão/retorno (haste do amortecedor)
                    float compressionVelocity = (compression - prevCompression) / Time.fixedDeltaTime;
                    prevCompression = compression;

                    // Equação clássica massa-mola-amortecedor: F = k*x + c*v
                    float springForce = (compression * stiffness) + (compressionVelocity * damping);
                    springForce = Mathf.Max(0f, springForce);

                    // Aplica força de sustentação no ponto de ancoragem do chassi
                    Vector3 forceVector = transform.up * springForce;
                    rb.AddForceAtPosition(forceVector, origin, ForceMode.Force);
                }

                // Posicionamento visual da roda na altura da superfície de contato
                if (wheelVisual != null)
                {
                    Vector3 targetPos = origin - transform.up * (hit.distance - tireRadius);
                    wheelVisual.position = Vector3.Lerp(wheelVisual.position, targetPos, Time.fixedDeltaTime * 25f);
                }
            }
            else
            {
                prevCompression = 0f;
                // Roda totalmente distendida no ar
                if (wheelVisual != null)
                {
                    Vector3 extendedPos = origin - transform.up * restLength;
                    wheelVisual.position = Vector3.Lerp(wheelVisual.position, extendedPos, Time.fixedDeltaTime * 15f);
                }
            }
        }
    }
}
