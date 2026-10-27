using UnityEngine;

namespace ArcadiaTime.Core
{
    /// <summary>
    /// Punto de acceso a los servicios compartidos. Se inicializa solo al arrancar el programa,
    /// antes de cargar cualquier escena, así que funciona igual si empiezas desde el menú
    /// o si pulsas Play directamente en la escena de un juego.
    /// </summary>
    public static class GameServices
    {
        public static IRecordsRepository Repository { get; private set; }
        public static ProfileService Profiles { get; private set; }
        public static RecordsService Records { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            Repository = new JsonRecordsRepository();
            Profiles = new ProfileService(Repository);
            Records = new RecordsService(Repository, Profiles);
        }
    }
}
