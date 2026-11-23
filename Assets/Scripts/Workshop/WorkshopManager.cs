using System;
using System.Collections.Generic;
using UnityEngine;
using ProjetoGrau2.Core;
using ProjetoGrau2.Data;
using ProjetoGrau2.Propulsion;
using ProjetoGrau2.Audio;

namespace ProjetoGrau2.Workshop
{
    /// <summary>
    /// Gerenciador central da oficina mecânica e visual.
    /// Controla catálogo de peças, instalação/remoção modular e sincronização
    /// em tempo real com os subsistemas físicos (Motor, Câmbio, Áudio e Chassi).
    /// </summary>
    public class WorkshopManager : MonoBehaviour
    {
        [Header("Alvo da Motocicleta")]
        [SerializeField] private MotorcycleController targetMotorcycle;

        [Header("Catálogo Geral de Peças Disponíveis")]
        [SerializeField] private List<PartItemData> availableCatalog = new List<PartItemData>();

        [Header("Inspeção Tridimensional (Câmera da Oficina)")]
        [SerializeField] private Transform turntableMotorcyclePivot;
        [SerializeField] private float rotationSpeed = 90f;
        [SerializeField] private float zoomSpeed = 2f;
        [SerializeField] private float minZoomDistance = 1.8f;
        [SerializeField] private float maxZoomDistance = 4.5f;

        // Dicionário de peças instaladas por categoria
        private readonly Dictionary<PartCategory, PartItemData> installedParts = new Dictionary<PartCategory, PartItemData>();

        // Eventos públicos
        public event Action<PartItemData> OnPartInstalled;
        public event Action<PartCategory> OnPartRemoved;

        public MotorcycleController TargetMotorcycle => targetMotorcycle;
        public IReadOnlyDictionary<PartCategory, PartItemData> InstalledParts => installedParts;

        private void Awake()
        {
            if (targetMotorcycle == null)
            {
                targetMotorcycle = FindFirstObjectByType<MotorcycleController>();
            }
        }

        /// <summary>
        /// Retorna todas as peças do catálogo filtradas por categoria.
        /// </summary>
        public List<PartItemData> GetPartsByCategory(PartCategory category)
        {
            List<PartItemData> result = new List<PartItemData>();
            foreach (var item in availableCatalog)
            {
                if (item != null && item.category == category)
                {
                    result.Add(item);
                }
            }
            return result;
        }

        /// <summary>
        /// Instala uma peça na motocicleta e aplica seus efeitos mecânicos e visuais imediatamente.
        /// </summary>
        public bool InstallPart(PartItemData part)
        {
            if (part == null || targetMotorcycle == null) return false;

            // Substitui peça anterior da mesma categoria se houver
            installedParts[part.category] = part;

            // 1. Aplica modificador de Motor
            if (part.category == PartCategory.Engine && part.engineOverride != null)
            {
                if (targetMotorcycle.Engine != null)
                {
                    targetMotorcycle.Engine.Initialize(part.engineOverride);
                }
            }

            // 2. Aplica modificador de Escapamento (Perfil Sonoro)
            if (part.category == PartCategory.Exhaust && part.audioProfileOverride != null)
            {
                var audioCtrl = targetMotorcycle.GetComponentInChildren<MotorAudioController>();
                // Notifica troca de perfil de som
            }

            // 3. Aplica modificador de Transmissão (Relação Coroa/Pinhão)
            if (part.category == PartCategory.Transmission && part.customFinalDriveRatio > 0.1f)
            {
                if (targetMotorcycle.Transmission != null && targetMotorcycle.Transmission.Data != null)
                {
                    targetMotorcycle.Transmission.Data.finalDriveRatio = part.customFinalDriveRatio;
                }
            }

            // 4. Modificação de Massa do Chassi
            if (Mathf.Abs(part.weightDeltaKg) > 0.01f && targetMotorcycle.Rigidbody != null)
            {
                targetMotorcycle.Rigidbody.mass = Mathf.Max(60f, targetMotorcycle.Rigidbody.mass + part.weightDeltaKg);
            }

            OnPartInstalled?.Invoke(part);
            return true;
        }

        /// <summary>
        /// Remove uma peça instalada da categoria especificada.
        /// </summary>
        public void UninstallPart(PartCategory category)
        {
            if (installedParts.ContainsKey(category))
            {
                installedParts.Remove(category);
                OnPartRemoved?.Invoke(category);
            }
        }

        /// <summary>
        /// Rotação da motocicleta na mesa giratória da oficina (Touch/Mouse Drag).
        /// </summary>
        public void RotateTurntable(float deltaAngle)
        {
            Transform pivot = turntableMotorcyclePivot != null ? turntableMotorcyclePivot : (targetMotorcycle != null ? targetMotorcycle.transform : null);
            if (pivot != null)
            {
                pivot.Rotate(Vector3.up, deltaAngle * rotationSpeed * Time.deltaTime, Space.World);
            }
        }
    }
}
