using UnityEngine;
using ProjetoGrau2.Data;
using ProjetoGrau2.Propulsion;

namespace ProjetoGrau2.Audio
{
    /// <summary>
    /// Controlador dinâmico de áudio do motor e escapamento.
    /// Sincronizado por observação aos estados do EngineController, TransmissionController e AthlonECUController.
    /// Totalmente desacoplado de cálculos físicos (respeitando a regra ADR-002).
    /// </summary>
    public class MotorAudioController : MonoBehaviour
    {
        [Header("Perfil de Áudio")]
        [SerializeField] private MotorAudioProfile audioProfile;

        [Header("Fontes de Áudio (AudioSources)")]
        [SerializeField] private AudioSource idleAudioSource;
        [SerializeField] private AudioSource runningAudioSource;
        [SerializeField] private AudioSource backfireAudioSource;
        [SerializeField] private AudioSource shiftAudioSource;

        [Header("Módulos Observados")]
        [SerializeField] private EngineController engineController;
        [SerializeField] private TransmissionController transmissionController;
        [SerializeField] private AthlonECUController ecuController;

        private void Awake()
        {
            if (engineController == null) engineController = GetComponentInParent<EngineController>();
            if (transmissionController == null) transmissionController = GetComponentInParent<TransmissionController>();
            if (ecuController == null) ecuController = GetComponentInParent<AthlonECUController>();

            SetupAudioSources();
        }

        private void OnEnable()
        {
            if (ecuController != null)
            {
                ecuController.OnBackfireTriggered += HandleBackfire;
            }

            if (transmissionController != null)
            {
                transmissionController.OnShiftStarted += HandleShift;
            }

            if (engineController != null)
            {
                engineController.OnEngineStarted += HandleEngineStarted;
                engineController.OnEngineStopped += HandleEngineStopped;
            }
        }

        private void OnDisable()
        {
            if (ecuController != null)
            {
                ecuController.OnBackfireTriggered -= HandleBackfire;
            }

            if (transmissionController != null)
            {
                transmissionController.OnShiftStarted -= HandleShift;
            }

            if (engineController != null)
            {
                engineController.OnEngineStarted -= HandleEngineStarted;
                engineController.OnEngineStopped -= HandleEngineStopped;
            }
        }

        private void SetupAudioSources()
        {
            if (idleAudioSource == null) idleAudioSource = gameObject.AddComponent<AudioSource>();
            if (runningAudioSource == null) runningAudioSource = gameObject.AddComponent<AudioSource>();
            if (backfireAudioSource == null) backfireAudioSource = gameObject.AddComponent<AudioSource>();
            if (shiftAudioSource == null) shiftAudioSource = gameObject.AddComponent<AudioSource>();

            idleAudioSource.loop = true;
            runningAudioSource.loop = true;
            backfireAudioSource.loop = false;
            shiftAudioSource.loop = false;

            idleAudioSource.playOnAwake = false;
            runningAudioSource.playOnAwake = false;

            if (audioProfile != null)
            {
                idleAudioSource.clip = audioProfile.idleClip;
                runningAudioSource.clip = audioProfile.highRpmClip ?? audioProfile.lowRpmClip;
            }
        }

        private void Start()
        {
            if (engineController != null && engineController.State == EngineState.Running)
            {
                HandleEngineStarted();
            }
        }

        private void Update()
        {
            if (audioProfile == null || engineController == null || engineController.State != EngineState.Running)
            {
                if (idleAudioSource != null && idleAudioSource.isPlaying) idleAudioSource.Stop();
                if (runningAudioSource != null && runningAudioSource.isPlaying) runningAudioSource.Stop();
                return;
            }

            float currentRpm = engineController.CurrentRpm;
            float idleRpm = engineController.Data != null ? engineController.Data.idleRpm : 1300f;
            float maxRpm = engineController.Data != null ? engineController.Data.maxRpm : 11000f;
            float throttle = engineController.EffectiveThrottle;

            // 1. Modulação de Pitch proporcional ao RPM
            float targetPitch = audioProfile.CalculatePitch(currentRpm, idleRpm, maxRpm);
            if (idleAudioSource != null) idleAudioSource.pitch = targetPitch;
            if (runningAudioSource != null) runningAudioSource.pitch = targetPitch;

            // 2. Modulação de Volume conforme Carga e Rotação
            float rpmNormalized = Mathf.Clamp01((currentRpm - idleRpm) / (maxRpm - idleRpm));
            float idleVol = Mathf.Lerp(audioProfile.idleVolume, 0.1f, rpmNormalized);
            float runningVol = Mathf.Lerp(0f, audioProfile.throttleVolumeBoost, rpmNormalized);

            // Redução de volume se o acelerador estiver solto (freio-motor)
            if (throttle < 0.05f)
            {
                runningVol *= audioProfile.decelerationVolumeFactor;
            }

            if (idleAudioSource != null) idleAudioSource.volume = idleVol;
            if (runningAudioSource != null) runningAudioSource.volume = runningVol;
        }

        private void HandleBackfire(float volume, Vector3 position)
        {
            if (audioProfile != null && audioProfile.backfirePops != null && audioProfile.backfirePops.Length > 0)
            {
                int idx = UnityEngine.Random.Range(0, audioProfile.backfirePops.Length);
                AudioClip clip = audioProfile.backfirePops[idx];
                if (clip != null && backfireAudioSource != null)
                {
                    backfireAudioSource.pitch = UnityEngine.Random.Range(0.85f, 1.25f);
                    backfireAudioSource.PlayOneShot(clip, volume);
                }
            }
        }

        private void HandleShift()
        {
            if (audioProfile != null && audioProfile.gearShiftClip != null && shiftAudioSource != null)
            {
                shiftAudioSource.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
                shiftAudioSource.PlayOneShot(audioProfile.gearShiftClip, 0.8f);
            }
        }

        private void HandleEngineStarted()
        {
            if (idleAudioSource != null && !idleAudioSource.isPlaying && idleAudioSource.clip != null)
            {
                idleAudioSource.Play();
            }
            if (runningAudioSource != null && !runningAudioSource.isPlaying && runningAudioSource.clip != null)
            {
                runningAudioSource.Play();
            }
        }

        private void HandleEngineStopped()
        {
            if (idleAudioSource != null) idleAudioSource.Stop();
            if (runningAudioSource != null) runningAudioSource.Stop();
        }
    }
}
