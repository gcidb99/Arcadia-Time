using UnityEngine;

namespace ArcadiaTime.Core
{
    /// <summary>Datos de un minijuego: lo que necesita el menú para mostrarlo y lanzarlo.</summary>
    [CreateAssetMenu(fileName = "NewGameDefinition", menuName = "Arcadia Time/Game Definition")]
    public class GameDefinition : ScriptableObject
    {
        [Tooltip("Identificador único y estable usado en los récords, por ejemplo: cutie-breaker")]
        [SerializeField] private string id;

        [SerializeField] private string displayName;
        [SerializeField] private Sprite thumbnail;

        [Tooltip("Nombre exacto de la escena del juego (debe estar en la lista de escenas del build)")]
        [SerializeField] private string sceneName;

        public string Id => id;
        public string DisplayName => displayName;
        public Sprite Thumbnail => thumbnail;
        public string SceneName => sceneName;
    }
}
