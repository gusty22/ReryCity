using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjetoGrau2.Rider
{
    /// <summary>
    /// Gerenciador de Ragdoll e quedas físicas do piloto.
    /// Controla a transição suave entre o Animator/IK e a simulação de múltiplos corpos rígidos (Ragdoll)
    /// ao capotar a moto ou colidir violentamente, permitindo ejeção no asfalto e recuperação ao resetar.
    /// </summary>
    public class RiderRagdollController : MonoBehaviour
    {
        [Header("Ejeção e Impulso de Queda")]
        [SerializeField] private float ejectionUpwardImpulse = 2.5f;
        [SerializeField] private float ejectionTumbleTorque = 8.0f;

        private MotorcycleRiderController riderController;
        private Animator animator;
        private RiderIKController ikController;

        // Lista de corpos e colisores que compõem o esqueleto ragdoll
        private Rigidbody[] ragdollBodies;
        private Collider[] ragdollColliders;

        private Transform originalParent;
        private Vector3 initialLocalPosition;
        private Quaternion initialLocalRotation;

        private bool isRagdollActive = false;
        public bool IsRagdollActive => isRagdollActive;

        public void Initialize(MotorcycleRiderController controller)
        {
            riderController = controller;
            SetupComponents();
        }

        private void Awake()
        {
            if (riderController == null) riderController = GetComponent<MotorcycleRiderController>();
            SetupComponents();
        }

        private void SetupComponents()
        {
            animator = GetComponent<Animator>();
            ikController = GetComponent<RiderIKController>();

            originalParent = transform.parent;
            initialLocalPosition = transform.localPosition;
            initialLocalRotation = transform.localRotation;

            // Coleta todos os rigidbodies e colliders filhos (excluindo a raiz se for o chassi)
            List<Rigidbody> bodies = new List<Rigidbody>();
            foreach (var rb in GetComponentsInChildren<Rigidbody>())
            {
                if (rb.gameObject != gameObject)
                {
                    bodies.Add(rb);
                }
            }
            ragdollBodies = bodies.ToArray();

            List<Collider> colliders = new List<Collider>();
            foreach (var col in GetComponentsInChildren<Collider>())
            {
                if (col.gameObject != gameObject)
                {
                    colliders.Add(col);
                }
            }
            ragdollColliders = colliders.ToArray();

            // Estado inicial: Ragdoll desativado (controlado pelo Animator)
            SetRagdollState(false);
        }

        public void ActivateRagdoll()
        {
            if (isRagdollActive) return;
            isRagdollActive = true;

            // 1. Desliga sistemas de animação e IK
            if (animator != null) animator.enabled = false;
            if (ikController != null) ikController.enabled = false;

            // 2. Desacopla da hierarquia da moto para movimentação livre no mundo
            transform.SetParent(null, true);

            // 3. Obtém o vetor de velocidade e momento linear da motocicleta
            Vector3 inheritedVelocity = Vector3.zero;
            Vector3 inheritedAngular = Vector3.zero;
            if (riderController != null && riderController.Motorcycle != null && riderController.Motorcycle.Rigidbody != null)
            {
                inheritedVelocity = riderController.Motorcycle.Rigidbody.linearVelocity;
                inheritedAngular = riderController.Motorcycle.Rigidbody.angularVelocity;
            }

            // 4. Ativa física em todos os ossos do esqueleto
            SetRagdollState(true);

            // 5. Aplica momento linear + impulso de ejeção para cima e rotação caótica
            Vector3 ejectionForce = inheritedVelocity + Vector3.up * ejectionUpwardImpulse + (UnityEngine.Random.insideUnitSphere * 1.5f);
            foreach (var rb in ragdollBodies)
            {
                rb.linearVelocity = ejectionForce;
                rb.angularVelocity = inheritedAngular + (UnityEngine.Random.insideUnitSphere * ejectionTumbleTorque);
            }
        }

        public void DeactivateRagdoll()
        {
            if (!isRagdollActive) return;
            isRagdollActive = false;

            // 1. Desativa a física dos ossos
            SetRagdollState(false);

            // 2. Re-ancora o piloto na motocicleta
            transform.SetParent(originalParent, false);
            transform.localPosition = initialLocalPosition;
            transform.localRotation = initialLocalRotation;

            // 3. Reativa Animator e IK
            if (animator != null) animator.enabled = true;
            if (ikController != null) ikController.enabled = true;
        }

        private void SetRagdollState(bool active)
        {
            if (ragdollBodies != null)
            {
                foreach (var rb in ragdollBodies)
                {
                    rb.isKinematic = !active;
                    rb.detectCollisions = active;
                }
            }

            if (ragdollColliders != null)
            {
                foreach (var col in ragdollColliders)
                {
                    col.enabled = active;
                }
            }
        }
    }
}
