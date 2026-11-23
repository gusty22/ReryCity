using UnityEngine;

namespace ProjetoGrau2.Core
{
    /// <summary>
    /// Câmera de perseguição em terceira pessoa otimizada para testes físicos.
    /// Segue a motocicleta suavemente sem sofrer com trepidações angulares extremas do Rigidbody.
    /// </summary>
    public class CameraFollowPhysics : MonoBehaviour
    {
        [Header("Alvo")]
        [SerializeField] private Transform target;

        [Header("Distância e Altura")]
        [SerializeField] private float distance = 4.2f;
        [SerializeField] private float height = 1.7f;
        [SerializeField] private float lookAtHeight = 0.9f;

        [Header("Suavização")]
        [SerializeField] private float positionDamping = 8f;
        [SerializeField] private float rotationDamping = 6f;

        private void LateUpdate()
        {
            if (target == null) return;

            // Calcula a posição desejada com base no yaw do alvo (ignora o pitch do grau para a posição da câmera não girar descontroladamente)
            Vector3 targetEuler = target.eulerAngles;
            Quaternion currentRotation = Quaternion.Euler(0f, targetEuler.y, 0f);

            Vector3 desiredPosition = target.position - (currentRotation * Vector3.forward * distance) + Vector3.up * height;
            transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * positionDamping);

            // Olha para o ponto focal na moto
            Vector3 lookTarget = target.position + Vector3.up * lookAtHeight;
            Vector3 lookDirection = (lookTarget - transform.position).normalized;

            if (lookDirection != Vector3.zero)
            {
                Quaternion targetLookRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetLookRotation, Time.deltaTime * rotationDamping);
            }
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }
    }
}
