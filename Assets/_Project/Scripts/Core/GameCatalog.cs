using System.Collections.Generic;
using UnityEngine;

namespace ArcadiaTime.Core
{
    /// <summary>Lista de juegos disponibles. El menú y la pantalla de récords se construyen a partir de ella.</summary>
    [CreateAssetMenu(fileName = "GameCatalog", menuName = "Arcadia Time/Game Catalog")]
    public class GameCatalog : ScriptableObject
    {
        [SerializeField] private List<GameDefinition> games = new List<GameDefinition>();

        public IReadOnlyList<GameDefinition> Games => games;
    }
}
