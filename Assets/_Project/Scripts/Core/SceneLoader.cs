using UnityEngine;
using UnityEngine.SceneManagement;

namespace ArcadiaTime.Core
{
    /// <summary>Nombres de las escenas fijas del programa. Deben coincidir con los archivos .unity.</summary>
    public static class SceneNames
    {
        public const string Menu = "Menu";
        public const string Records = "Records";
    }

    /// <summary>Carga de escenas. Todo cambio de escena pasa por aquí.</summary>
    public static class SceneLoader
    {
        public static void Load(string sceneName)
        {
            // Si se sale desde una pausa, el tiempo vuelve a su velocidad normal.
            Time.timeScale = 1f;
            SceneManager.LoadScene(sceneName);
        }

        public static void LoadMenu()
        {
            Load(SceneNames.Menu);
        }
    }
}
