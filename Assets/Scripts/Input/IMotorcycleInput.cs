using UnityEngine;

namespace ProjetoGrau2.Inputs
{
    /// <summary>
    /// Contrato universal de entrada para o controle das motocicletas e câmeras.
    /// Permite desacoplar a física e a perspectiva visual da origem dos comandos.
    /// </summary>
    public interface IMotorcycleInput
    {
        /// <summary>Acelerador analógico graduado de 0.0 a 1.0.</summary>
        float Throttle { get; }

        /// <summary>Freio dianteiro na manete graduado de 0.0 a 1.0.</summary>
        float FrontBrake { get; }

        /// <summary>Freio traseiro no pedal graduado de 0.0 a 1.0 (fundamental para o grau).</summary>
        float RearBrake { get; }

        /// <summary>Esterçamento direcional do guidão de -1.0 (esquerda) a +1.0 (direita).</summary>
        float Steering { get; }

        /// <summary>Comando direto de puxar o grau (erguer a roda dianteira).</summary>
        bool WheelieHold { get; }

        /// <summary>Comando de reinicialização da cena de teste físico.</summary>
        bool ResetTriggered { get; }

        /// <summary>Comando de subir marcha (Shift Up).</summary>
        bool ShiftUpTriggered { get; }

        /// <summary>Comando de descer marcha (Shift Down).</summary>
        bool ShiftDownTriggered { get; }

        /// <summary>Acionamento manual da manete de embreagem de 0.0 a 1.0.</summary>
        float Clutch { get; }

        /// <summary>Comando para alternar o modo de câmera ativa (1ª pessoa, 3ª pessoa, Stunt).</summary>
        bool CameraCycleTriggered { get; }

        /// <summary>Delta de rotação da câmera (Mouse X/Y no PC ou arrasto tátil no Mobile).</summary>
        Vector2 LookDelta { get; }
    }
}
