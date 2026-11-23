using System;
using UnityEngine;
using ProjetoGrau2.Data;
using ProjetoGrau2.Core;

namespace ProjetoGrau2.Rider
{
    /// <summary>
    /// Controlador de Cinemática Inversa (IK) para o piloto humanoide.
    /// Utiliza Unity OnAnimatorIK nativo para fixar com precisão mãos no guidão,
    /// pés nas pedaleiras e olhar na trajetória, compatível com qualquer avatar Humanoid.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class RiderIKController : MonoBehaviour
    {
        [Header("Configuração de Dados")]
        [SerializeField] private RiderData data;

        [Header("Pontos de Ancoragem na Motocicleta")]
        [SerializeField] private Transform leftHandAnchor;
        [SerializeField] private Transform rightHandAnchor;
        [SerializeField] private Transform leftFootAnchor;
        [SerializeField] private Transform rightFootAnchor;
        [SerializeField] private Transform seatPelvisAnchor;
        [SerializeField] private Transform lookTargetAnchor;

        [Header("Pesos de Atuação IK")]
        [Range(0f, 1f)] [SerializeField] private float leftHandWeight = 1.0f;
        [Range(0f, 1f)] [SerializeField] private float rightHandWeight = 1.0f;
        [Range(0f, 1f)] [SerializeField] private float leftFootWeight = 1.0f;
        [Range(0f, 1f)] [SerializeField] private float rightFootWeight = 1.0f;
        [Range(0f, 1f)] [SerializeField] private float lookAtWeight = 0.85f;

        private Animator animator;
        private MotorcycleController motorcycle;

        public void Initialize(RiderData riderData, MotorcycleController targetMotorcycle)
        {
            data = riderData;
            motorcycle = targetMotorcycle;
            animator = GetComponent<Animator>();
        }

        private void Awake()
        {
            animator = GetComponent<Animator>();
            if (motorcycle == null)
            {
                motorcycle = GetComponentInParent<MotorcycleController>();
            }
        }

        public void SetAnchors(Transform lHand, Transform rHand, Transform lFoot, Transform rFoot, Transform seat, Transform look)
        {
            leftHandAnchor = lHand;
            rightHandAnchor = rHand;
            leftFootAnchor = lFoot;
            rightFootAnchor = rFoot;
            seatPelvisAnchor = seat;
            lookTargetAnchor = look;
        }

        private void OnAnimatorIK(int layerIndex)
        {
            if (animator == null) return;

            // 1. IK da Mão Esquerda (Guidão Esquerdo)
            if (leftHandAnchor != null)
            {
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, leftHandWeight);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, leftHandWeight);
                animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandAnchor.position);
                animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandAnchor.rotation);
            }

            // 2. IK da Mão Direita (Manopla do Acelerador / Freio Dianteiro)
            if (rightHandAnchor != null)
            {
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, rightHandWeight);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, rightHandWeight);
                animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandAnchor.position);
                animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandAnchor.rotation);
            }

            // 3. IK do Pé Esquerdo (Pedaleira de Marcha)
            if (leftFootAnchor != null)
            {
                animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, leftFootWeight);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, leftFootWeight);
                animator.SetIKPosition(AvatarIKGoal.LeftFoot, leftFootAnchor.position);
                animator.SetIKRotation(AvatarIKGoal.LeftFoot, leftFootAnchor.rotation);
            }

            // 4. IK do Pé Direito (Pedaleira / Pedal de Freio Traseiro)
            if (rightFootAnchor != null)
            {
                animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, rightFootWeight);
                animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, rightFootWeight);
                animator.SetIKPosition(AvatarIKGoal.RightFoot, rightFootAnchor.position);
                animator.SetIKRotation(AvatarIKGoal.RightFoot, rightFootAnchor.rotation);
            }

            // 5. LookAt (Direção da Trajetória / Curva)
            if (lookTargetAnchor != null)
            {
                animator.SetLookAtWeight(lookAtWeight, 0.4f, 0.8f, 0.5f, 0.5f);
                animator.SetLookAtPosition(lookTargetAnchor.position);
            }
            else if (motorcycle != null)
            {
                Vector3 defaultLookPos = motorcycle.transform.position + motorcycle.transform.forward * 12f + Vector3.up * 1.5f;
                animator.SetLookAtWeight(lookAtWeight, 0.3f, 0.7f, 0.5f, 0.5f);
                animator.SetLookAtPosition(defaultLookPos);
            }
        }
    }
}
