using System;
using UnityEngine;
using ProjetoGrau2.Core;
using ProjetoGrau2.Police;
using ProjetoGrau2.World;

namespace ProjetoGrau2.Audio
{
    /// <summary>
    /// Gerenciador central de integração e roteamento de áudio do jogo (SYS_AUDIO).
    /// Coordena canais de som do motor, trocas de marcha, corte de giro, sirenes policiais espaciais,
    /// impactos físicos e alertas da interface do usuário (HUD).
    /// Opera com checagem nula defensiva caso clipes sonoros estejam pendentes de importação.
    /// </summary>
    [AddComponentMenu("Projeto Grau 2/Audio/Audio Integration Manager")]
    public class AudioIntegrationManager : MonoBehaviour
    {
        public static AudioIntegrationManager Instance { get; private set; }

        [Header("Volumes Gerais")]
        [Range(0f, 1f)] public float masterVolume = 1.0f;
        [Range(0f, 1f)] public float engineVolume = 0.9f;
        [Range(0f, 1f)] public float sirenVolume = 0.85f;
        [Range(0f, 1f)] public float uiVolume = 0.75f;
        [Range(0f, 1f)] public float collisionVolume = 0.9f;

        [Header("Fontes de Áudio de Interface")]
        [SerializeField] private AudioSource uiAudioSource;
        [SerializeField] private AudioClip uiClickClip;
        [SerializeField] private AudioClip violationBuzzerClip;
        [SerializeField] private AudioClip worldEventChimeClip;

        [Header("Fontes de Áudio de Colisão e Impacto")]
        [SerializeField] private AudioSource collisionAudioSource;
        [SerializeField] private AudioClip crashImpactClip;

        [Header("Módulos de Áudio Observados")]
        [SerializeField] private MotorAudioController motorcycleAudio;

        public bool HasMotorcycleAudioAttached => motorcycleAudio != null;
        public bool HasUIAudioSourceAttached => uiAudioSource != null;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            EnsureAudioSourcesExist();
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void EnsureAudioSourcesExist()
        {
            if (uiAudioSource == null)
            {
                uiAudioSource = gameObject.AddComponent<AudioSource>();
                uiAudioSource.playOnAwake = false;
                uiAudioSource.spatialBlend = 0f; // Áudio 2D estéreo para UI
            }

            if (collisionAudioSource == null)
            {
                collisionAudioSource = gameObject.AddComponent<AudioSource>();
                collisionAudioSource.playOnAwake = false;
                collisionAudioSource.spatialBlend = 0.7f; // Áudio espacial 3D para colisões
                collisionAudioSource.minDistance = 3f;
                collisionAudioSource.maxDistance = 60f;
            }

            if (motorcycleAudio == null)
            {
                motorcycleAudio = FindFirstObjectByType<MotorAudioController>();
            }
        }

        public void PlayUIClick()
        {
            if (uiAudioSource != null && uiClickClip != null)
            {
                uiAudioSource.PlayOneShot(uiClickClip, masterVolume * uiVolume);
            }
        }

        public void PlayViolationAlert()
        {
            if (uiAudioSource != null && violationBuzzerClip != null)
            {
                uiAudioSource.PlayOneShot(violationBuzzerClip, masterVolume * uiVolume);
            }
        }

        public void PlayWorldEventChime()
        {
            if (uiAudioSource != null && worldEventChimeClip != null)
            {
                uiAudioSource.PlayOneShot(worldEventChimeClip, masterVolume * uiVolume);
            }
        }

        public void PlayCollisionImpact(float intensity, Vector3 worldPos)
        {
            if (collisionAudioSource != null)
            {
                collisionAudioSource.transform.position = worldPos;
                if (crashImpactClip != null)
                {
                    float vol = Mathf.Clamp01(intensity) * masterVolume * collisionVolume;
                    collisionAudioSource.PlayOneShot(crashImpactClip, vol);
                }
            }
        }

        /// <summary>
        /// Configura espacialmente e aciona a fonte de áudio da sirene da viatura ou moto ROCAM.
        /// </summary>
        public void ConfigureSpatialSirenSource(AudioSource sirenSource)
        {
            if (sirenSource == null) return;
            sirenSource.spatialBlend = 1.0f; // Totalmente 3D
            sirenSource.rolloffMode = AudioRolloffMode.Logarithmic;
            sirenSource.minDistance = 5f;
            sirenSource.maxDistance = 80f;
            sirenSource.dopplerLevel = 1.2f; // Efeito Doppler realista para perseguições
            sirenSource.volume = masterVolume * sirenVolume;
        }
    }
}
