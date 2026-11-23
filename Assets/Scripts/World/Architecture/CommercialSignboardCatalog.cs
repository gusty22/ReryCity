using System.Collections.Generic;
using UnityEngine;

namespace ProjetoGrau2.World.Architecture
{
    [System.Serializable]
    public struct CommercialSignboard
    {
        public string establishmentId;
        public string tradeName;          // Nome de fantasia (ex: "Oficina do Grau")
        public string slogan;             // Slogan fictício (ex: "Escape Esportivo & Preparação")
        public BuildingStyleCategory category;
        public Color signboardColor;
        public Color textColor;
    }

    /// <summary>
    /// Catálogo central de marcas fictícias e letreiros comerciais exclusivos de RERY CITY.
    /// Garante ambientação autêntica com estabelecimentos brasileiros típicos sem uso de marcas registradas.
    /// </summary>
    [CreateAssetMenu(fileName = "CommercialSignboards_Default", menuName = "Rery City/Architecture/Commercial Signboards Catalog")]
    public class CommercialSignboardCatalog : ScriptableObject
    {
        [Header("Letreiros Comerciais Fictícios")]
        [SerializeField] private List<CommercialSignboard> signboards = new List<CommercialSignboard>();

        public IReadOnlyList<CommercialSignboard> Signboards => signboards;

        public CommercialSignboard GetSignboardForCategory(BuildingStyleCategory cat, int index = 0)
        {
            List<CommercialSignboard> matching = new List<CommercialSignboard>();
            for (int i = 0; i < signboards.Count; i++)
            {
                if (signboards[i].category == cat)
                {
                    matching.Add(signboards[i]);
                }
            }

            if (matching.Count > 0)
            {
                int safeIdx = Mathf.Abs(index) % matching.Count;
                return matching[safeIdx];
            }

            return new CommercialSignboard
            {
                establishmentId = "Comm_Default",
                tradeName = "Comércio Rery City",
                slogan = "Artigos em Geral",
                category = cat,
                signboardColor = Color.blue,
                textColor = Color.white
            };
        }
    }
}
