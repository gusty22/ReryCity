using System;
using UnityEngine;

namespace ProjetoGrau2.World
{
    public enum PedestrianState
    {
        Idle,
        Walking,
        CrossingStreet,
        Fleeing
    }

    /// <summary>
    /// Controlador autônomo leve de pedestres NPC (World Population System).
    /// Controla circulação em calçadas, travessias e reação de fuga ao tráfego rápido,
    /// totalmente desacoplado e reutilizável via Object Pooling sem alocações no heap.
    /// </summary>
    [AddComponentMenu("Projeto Grau 2/World/Pedestrian Controller")]
    public class PedestrianController : MonoBehaviour
    {
        [Header("Estados e Movimentação")]
        [SerializeField] private PedestrianState currentState = PedestrianState.Walking;
        [Range(0.8f, 2.5f)] public float walkSpeed = 1.35f;
        [Range(2.5f, 5.0f)] public float fleeSpeed = 3.6f;

        [Header("Navegação em Calçadas")]
        [SerializeField] private Vector3 targetWaypoint;
        [SerializeField] private float arrivalDistance = 1.0f;

        [Header("Reação a Perigo")]
        [SerializeField] private float dangerDetectionRadius = 3.5f;

        private Vector3 _startPosition;
        private float _fleeTimer = 0f;

        public PedestrianState CurrentState => currentState;
        public bool IsActive => gameObject.activeInHierarchy;

        public void Initialize(Vector3 spawnPosition, Vector3 initialTarget)
        {
            transform.position = spawnPosition;
            _startPosition = spawnPosition;
            targetWaypoint = initialTarget;
            currentState = PedestrianState.Walking;
            _fleeTimer = 0f;
            gameObject.SetActive(true);
        }

        public void ResetPedestrian()
        {
            currentState = PedestrianState.Idle;
            _fleeTimer = 0f;
            gameObject.SetActive(false);
        }

        public void SetTarget(Vector3 target)
        {
            targetWaypoint = target;
            if (currentState != PedestrianState.Fleeing)
            {
                currentState = PedestrianState.Walking;
            }
        }

        public void TriggerFlee(Vector3 dangerSource)
        {
            currentState = PedestrianState.Fleeing;
            _fleeTimer = 2.5f;

            // Foge na direção oposta ao perigo
            Vector3 fleeDir = (transform.position - dangerSource).normalized;
            fleeDir.y = 0f;
            targetWaypoint = transform.position + fleeDir * 6f;
        }

        private void Update()
        {
            float dt = Time.deltaTime;

            if (currentState == PedestrianState.Fleeing)
            {
                _fleeTimer -= dt;
                if (_fleeTimer <= 0f)
                {
                    currentState = PedestrianState.Walking;
                }
            }

            MoveTowardsTarget(dt);
        }

        private void MoveTowardsTarget(float dt)
        {
            if (currentState == PedestrianState.Idle) return;

            Vector3 toTarget = targetWaypoint - transform.position;
            toTarget.y = 0f;
            float dist = toTarget.magnitude;

            if (dist <= arrivalDistance)
            {
                if (currentState == PedestrianState.Fleeing)
                {
                    currentState = PedestrianState.Walking;
                }
                else
                {
                    // Inverte ou encerra rota simples
                    targetWaypoint = _startPosition;
                    _startPosition = transform.position;
                }
                return;
            }

            Vector3 moveDir = toTarget / dist;
            float speed = (currentState == PedestrianState.Fleeing) ? fleeSpeed : walkSpeed;
            transform.position += moveDir * (speed * dt);

            // Orientação de rotação suave
            if (moveDir.sqrMagnitude > 0.001f)
            {
                transform.rotation = Quaternion.LookRotation(moveDir, Vector3.up);
            }
        }
    }
}
