using System;
using UnityEngine;
using ProjetoGrau2.Data;
using ProjetoGrau2.Core;
using ProjetoGrau2.Physics;

namespace ProjetoGrau2.Rider
{
    /// <summary>
    /// Controlador postural dinâmico do piloto.
    /// Modula os ossos da coluna vertebral, tórax e cabeça do esqueleto humanoide
    /// em tempo real em LateUpdate, reagindo a forças de inércia, inclinação em curvas,
    /// aceleração, frenagem e posturas extremas de raspão durante o grau.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class RiderPoseController : MonoBehaviour
    {
        [Header("Configuração de Dados")]
        [SerializeField] private RiderData data;

        [Header("Referências Físicas")]
        [SerializeField] private MotorcycleController motorcycle;
        [SerializeField] private WheeliePhysicsController wheelieController;

        private Animator animator;
        private Transform spineBone;
        private Transform chestBone;
        private Transform headBone;

        // Ângulos posturais interpolados
        private float currentSpinePitch = 0f;
        private float currentSpineRoll = 0f;
        private float currentHeadYaw = 0f;

        public void Initialize(RiderData riderData, MotorcycleController targetMotorcycle, WheeliePhysicsController wheelie)
        {
            data = riderData;
            motorcycle = targetMotorcycle;
            wheelieController = wheelie;
            ResolveBones();
        }

        private void Awake()
        {
            animator = GetComponent<Animator>();
            if (motorcycle == null) motorcycle = GetComponentInParent<MotorcycleController>();
            if (wheelieController == null && motorcycle != null) wheelieController = motorcycle.WheelieController;

            ResolveBones();
        }

        private void ResolveBones()
        {
            if (animator != null && animator.isHuman)
            {
                spineBone = animator.GetBoneTransform(HumanBodyBones.Spine);
                chestBone = animator.GetBoneTransform(HumanBodyBones.Chest);
                headBone = animator.GetBoneTransform(HumanBodyBones.Head);
            }
        }

        private void LateUpdate()
        {
            if (data == null || motorcycle == null || animator == null) return;
            if (!animator.enabled) return; // Se estiver em modo Ragdoll, não sobrescreve ossos

            CalculateTargetPose(out float targetPitch, out float targetRoll, out float targetHeadYaw);

            // Interpolação suave amortecida contra vibrações de alta frequência
            float dt = Time.deltaTime;
            currentSpinePitch = Mathf.Lerp(currentSpinePitch, targetPitch, dt * data.postureDamping);
            currentSpineRoll = Mathf.Lerp(currentSpineRoll, targetRoll, dt * data.postureDamping);
            currentHeadYaw = Mathf.Lerp(currentHeadYaw, targetHeadYaw, dt * data.headLookSpeed);

            ApplyBoneOffsets();
        }

        private void CalculateTargetPose(out float targetPitch, out float targetRoll, out float targetHeadYaw)
        {
            float speedKmh = motorcycle.CurrentSpeedKmh;
            float wheeliePitch = wheelieController != null ? wheelieController.CurrentPitchAngle : 0f;
            float rollAngle = motorcycle.transform.eulerAngles.z;
            if (rollAngle > 180f) rollAngle -= 360f;

            // 1. Postura no Eixo de Arfagem (Pitch)
            if (wheeliePitch > 20f)
            {
                // Durante o grau: projeta o tronco para trás proporcionalmente ao ângulo
                float wheelieFactor = Mathf.Clamp01((wheeliePitch - 20f) / 50f);
                targetPitch = -Mathf.Lerp(data.spineForwardLeanAngle * 0.5f, data.wheelieSpineBackLeanAngle, wheelieFactor);

                // Postura de Raspão extremo (> 70°): tronco quase alinhado com a rabeta
                if (wheeliePitch >= 70f)
                {
                    targetPitch -= 15f;
                }
            }
            else
            {
                // Em solo: inclinação padrão para frente atenuada/aumentada por aceleração ou frenagem
                targetPitch = data.spineForwardLeanAngle;

                // Sob alta velocidade ou aceleração, agacha contra o tanque
                if (speedKmh > 60f)
                {
                    float speedFactor = Mathf.Clamp01((speedKmh - 60f) / 80f);
                    targetPitch += data.accelerationTuckAngle * speedFactor;
                }
            }

            // 2. Postura de Curva (Roll): tronco pendula para dentro da curva
            targetRoll = -rollAngle * data.leanInCurveFactor;

            // 3. Rotação da Cabeça (Olhar para o ponto de fuga da curva)
            targetHeadYaw = -rollAngle * 0.4f;
        }

        private void ApplyBoneOffsets()
        {
            // Aplica rotações adicionais aditivas nos ossos humanoides da coluna
            if (spineBone != null)
            {
                spineBone.localRotation *= Quaternion.Euler(currentSpinePitch * 0.5f, 0f, currentSpineRoll * 0.5f);
            }

            if (chestBone != null)
            {
                chestBone.localRotation *= Quaternion.Euler(currentSpinePitch * 0.5f, 0f, currentSpineRoll * 0.5f);
            }

            if (headBone != null)
            {
                headBone.localRotation *= Quaternion.Euler(-currentSpinePitch * 0.3f, currentHeadYaw, 0f);
            }
        }
    }
}
