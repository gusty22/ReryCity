using System;
using UnityEngine;
using ProjetoGrau2.Data;
using ProjetoGrau2.Inputs;

namespace ProjetoGrau2.Physics
{
    /// <summary>
    /// Dados de telemetria emitidos a cada quadro físico (FixedUpdate) para depuração e HUD.
    /// </summary>
    public struct WheelieTelemetryData
    {
        public float PitchAngle;
        public float AngularVelocityX;
        public float LiftTorque;
        public float GravityTorque;
        public float BrakeTorque;
        public float NetTorque;
        public bool InBalanceZone;
        public bool IsCrashed;
        public float TimeInWheelie;
    }

    /// <summary>
    /// Controlador físico de empinar da motocicleta.
    /// Implementa equações diferenciais de torque angular em torno do eixo transversal local (Pitch).
    /// Baseado nas especificações técnicas comprovadas do BikeWheelie do Projeto Grau 2 original.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class WheeliePhysicsController : MonoBehaviour
    {
        [Header("Configuração Física (ScriptableObject)")]
        [SerializeField] private MotorcycleData _data;

        private Rigidbody _rb;
        private IMotorcycleInput _input;

        // Estados em tempo de execução
        private float _currentPitchAngle;
        private float _angularVelocityX;
        private bool _isCrashed;
        private float _wheelieTimer;
        private bool _isWheelieActive;

        // Eventos públicos de notificação (desacoplamento limpo)
        public event Action<WheelieTelemetryData> OnTelemetryUpdated;
        public event Action OnCrashTriggered;
        public event Action OnRecoveredToGround;

        public float CurrentPitchAngle => _currentPitchAngle;
        public bool IsCrashed => _isCrashed;
        public float TimeInWheelie => _wheelieTimer;

        public void Initialize(MotorcycleData data, IMotorcycleInput input)
        {
            _data = data;
            _input = input;
            _rb = GetComponent<Rigidbody>();
        }

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            if (_input == null)
            {
                _input = GetComponent<IMotorcycleInput>() ?? GetComponentInChildren<IMotorcycleInput>();
            }
        }

        private void FixedUpdate()
        {
            if (_data == null || _input == null) return;

            // 1. Cálculo do ângulo de arfagem (Pitch) em relação ao plano horizontal
            Vector3 forwardFlat = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
            if (forwardFlat.sqrMagnitude > 0.001f)
            {
                _currentPitchAngle = Vector3.Angle(transform.forward, forwardFlat);
                // Se a frente estiver apontando para cima, o ângulo é positivo
                if (transform.forward.y < 0) _currentPitchAngle = -_currentPitchAngle;
            }
            else
            {
                _currentPitchAngle = 90f * Mathf.Sign(transform.forward.y);
            }

            // Velocidade angular local no eixo transversal (X local), imune a rotações de Yaw
            Vector3 localAngularVelocity = transform.InverseTransformDirection(_rb.angularVelocity);
            _angularVelocityX = localAngularVelocity.x * Mathf.Rad2Deg;

            // 2. Verificação de Limite de Queda (Crash Angle)
            if (!_isCrashed && _currentPitchAngle >= _data.crashAngle)
            {
                TriggerCrash();
                return;
            }

            if (_isCrashed) return;

            // 3. Cálculo das componentes de torque
            float netTorque = CalculateNetTorque(localAngularVelocity.x);

            // 4. Aplicação física do torque angular de arfagem no Rigidbody com respeito ao tensor de inércia e massa
            // Vector3.right no espaço local é o eixo horizontal transversal da moto
            _rb.AddRelativeTorque(Vector3.right * netTorque, ForceMode.Force);

            // 5. Cronômetro de tempo de grau contínuo
            UpdateWheelieTimer();

            // 6. Notificação de Telemetria
            EmitTelemetry(netTorque);
        }

        private float CalculateNetTorque(float localAngularVelX)
        {
            float throttle = _input.Throttle;
            float rearBrake = _input.RearBrake;
            bool wheelieCommand = _input.WheelieHold;

            // A) Torque de Elevação (Lift Torque)
            float lift = 0f;
            if (wheelieCommand || throttle > 0.1f)
            {
                float easeFactor = CalculateEaseFactor(_currentPitchAngle);
                float throttleInfluence = wheelieCommand ? 1.0f : (throttle * _data.throttleLiftFactor);
                lift = _data.liftTorque * throttleInfluence * easeFactor;
            }

            // B) Torque Restaurador da Gravidade (Gravity Torque)
            // Diminui conforme o ângulo se aproxima da vertical (cos(theta))
            float rad = _currentPitchAngle * Mathf.Deg2Rad;
            float gravity = Mathf.Cos(rad) * (_data.mass * 9.81f * 0.45f) * _data.gravityTorqueFactor;
            if (_currentPitchAngle < 0) gravity = -gravity; // se a frente estiver afundada

            // C) Torque do Freio Traseiro (Brake Torque)
            // Freio a pé puxa a frente de volta imediatamente para evitar capotar
            float brake = 0f;
            if (rearBrake > 0.01f && _currentPitchAngle > 5f)
            {
                brake = _data.brakeTorque * rearBrake;
            }

            // D) Amortecimento Angular (Damping Torque)
            // Opõe-se à velocidade angular local em X para estabilizar oscilações bruscas
            float damping = -(localAngularVelX * _data.damping * 10f);

            // E) Assistência de Equilíbrio (Balance Assist)
            // Na janela balanceAssistRange em torno de balanceAngle, aplica leve atração restauradora
            float assist = 0f;
            float deltaFromBalance = _currentPitchAngle - _data.balanceAngle;
            if (Mathf.Abs(deltaFromBalance) <= _data.balanceAssistRange && _currentPitchAngle > 20f)
            {
                assist = -deltaFromBalance * (_data.balanceAssist * 15f);
            }

            // F) Retorno ao Solo (Recovery) quando o comando de grau é aliviado
            float recovery = 0f;
            if (!wheelieCommand && throttle < 0.05f && _currentPitchAngle > 0f)
            {
                recovery = -(_data.recoverySpeed * 20f);
            }

            float totalTorque = lift - gravity - brake + damping + assist + recovery;
            return totalTorque;
        }

        private float CalculateEaseFactor(float pitch)
        {
            float easeStart = _data.balanceAngle - _data.liftEaseRange;
            if (pitch <= easeStart) return 1.0f;
            if (pitch >= _data.balanceAngle) return 0.05f; // resíduo suave de sustentação

            float t = (pitch - easeStart) / _data.liftEaseRange;
            return Mathf.Lerp(1.0f, 0.05f, t);
        }

        private void UpdateWheelieTimer()
        {
            if (_currentPitchAngle >= 25f && !_isCrashed)
            {
                _isWheelieActive = true;
                _wheelieTimer += Time.fixedDeltaTime;
            }
            else
            {
                if (_isWheelieActive && _currentPitchAngle <= 3f)
                {
                    _isWheelieActive = false;
                    OnRecoveredToGround?.Invoke();
                }
                if (_currentPitchAngle <= 5f)
                {
                    _wheelieTimer = 0f;
                }
            }
        }

        private void TriggerCrash()
        {
            _isCrashed = true;
            _isWheelieActive = false;
            OnCrashTriggered?.Invoke();
        }

        public void ResetPhysics(Vector3 position, Quaternion rotation)
        {
            _isCrashed = false;
            _wheelieTimer = 0f;
            _isWheelieActive = false;
            _currentPitchAngle = 0f;
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            transform.position = position;
            transform.rotation = rotation;
        }

        private void EmitTelemetry(float netTorque)
        {
            bool inBalance = Mathf.Abs(_currentPitchAngle - _data.balanceAngle) <= (_data.balanceAssistRange * 0.5f);
            WheelieTelemetryData telem = new WheelieTelemetryData
            {
                PitchAngle = _currentPitchAngle,
                AngularVelocityX = _angularVelocityX,
                LiftTorque = _data.liftTorque,
                GravityTorque = Mathf.Cos(_currentPitchAngle * Mathf.Deg2Rad) * _data.mass * 9.81f,
                BrakeTorque = _input.RearBrake * _data.brakeTorque,
                NetTorque = netTorque,
                InBalanceZone = inBalance,
                IsCrashed = _isCrashed,
                TimeInWheelie = _wheelieTimer
            };

            OnTelemetryUpdated?.Invoke(telem);
        }
    }
}
