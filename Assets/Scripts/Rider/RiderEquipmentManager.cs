using System;
using System.Collections.Generic;
using UnityEngine;
using ProjetoGrau2.Data;

namespace ProjetoGrau2.Rider
{
    /// <summary>
    /// Gerenciador de equipamentos, vestimentas e acessórios do piloto.
    /// Permite equipar e substituir capacetes, camisas, bermudas, tênis e mochilas
    /// dinamicamente em tempo de execução sem corromper o esqueleto ou o Animator.
    /// </summary>
    public class RiderEquipmentManager : MonoBehaviour
    {
        [Header("Pontos de Fixação de Ossos (Bones)")]
        [SerializeField] private Transform headAnchor;
        [SerializeField] private Transform spineAnchor;
        [SerializeField] private Transform hipsAnchor;
        [SerializeField] private Transform leftFootAnchor;
        [SerializeField] private Transform rightFootAnchor;
        [SerializeField] private Transform leftHandAnchor;
        [SerializeField] private Transform rightHandAnchor;

        // Dicionário de equipamentos ativos
        private readonly Dictionary<RiderEquipmentCategory, RiderEquipmentData> equippedItems = new Dictionary<RiderEquipmentCategory, RiderEquipmentData>();
        private readonly Dictionary<RiderEquipmentCategory, GameObject> spawnedVisualInstances = new Dictionary<RiderEquipmentCategory, GameObject>();

        public event Action<RiderEquipmentData> OnEquipmentChanged;

        private void Awake()
        {
            ResolveDefaultBones();
        }

        private void ResolveDefaultBones()
        {
            Animator anim = GetComponent<Animator>();
            if (anim != null && anim.isHuman)
            {
                if (headAnchor == null) headAnchor = anim.GetBoneTransform(HumanBodyBones.Head);
                if (spineAnchor == null) spineAnchor = anim.GetBoneTransform(HumanBodyBones.Chest) ?? anim.GetBoneTransform(HumanBodyBones.Spine);
                if (hipsAnchor == null) hipsAnchor = anim.GetBoneTransform(HumanBodyBones.Hips);
                if (leftFootAnchor == null) leftFootAnchor = anim.GetBoneTransform(HumanBodyBones.LeftFoot);
                if (rightFootAnchor == null) rightFootAnchor = anim.GetBoneTransform(HumanBodyBones.RightFoot);
                if (leftHandAnchor == null) leftHandAnchor = anim.GetBoneTransform(HumanBodyBones.LeftHand);
                if (rightHandAnchor == null) rightHandAnchor = anim.GetBoneTransform(HumanBodyBones.RightHand);
            }
        }

        public Transform GetAnchorForCategory(RiderEquipmentCategory category)
        {
            switch (category)
            {
                case RiderEquipmentCategory.Helmet: return headAnchor;
                case RiderEquipmentCategory.Shirt:
                case RiderEquipmentCategory.Accessories: return spineAnchor;
                case RiderEquipmentCategory.Pants: return hipsAnchor;
                case RiderEquipmentCategory.Footwear: return leftFootAnchor;
                case RiderEquipmentCategory.Gloves: return leftHandAnchor;
                default: return headAnchor;
            }
        }

        /// <summary>
        /// Equipa um novo item no piloto substituindo o anterior da mesma categoria.
        /// </summary>
        public bool Equip(RiderEquipmentData item)
        {
            if (item == null) return false;

            // Remove o visual anterior da categoria se existir
            Unequip(item.category);

            equippedItems[item.category] = item;

            Transform anchor = GetAnchorForCategory(item.category);
            if (anchor != null && item.equipmentPrefab != null)
            {
                GameObject instance = Instantiate(item.equipmentPrefab, anchor);
                instance.transform.localPosition = item.localOffset;
                instance.transform.localEulerAngles = item.localEulerRotation;
                spawnedVisualInstances[item.category] = instance;

                // Aplica substituição de material se configurada
                if (item.materialOverride != null)
                {
                    Renderer[] rends = instance.GetComponentsInChildren<Renderer>();
                    foreach (var r in rends) r.material = item.materialOverride;
                }
            }

            OnEquipmentChanged?.Invoke(item);
            return true;
        }

        /// <summary>
        /// Remove o equipamento da categoria especificada.
        /// </summary>
        public void Unequip(RiderEquipmentCategory category)
        {
            if (spawnedVisualInstances.TryGetValue(category, out GameObject inst) && inst != null)
            {
                Destroy(inst);
                spawnedVisualInstances.Remove(category);
            }

            if (equippedItems.ContainsKey(category))
            {
                equippedItems.Remove(category);
                OnEquipmentChanged?.Invoke(null);
            }
        }
    }
}
