using System.Collections.Generic;

namespace ArcadiaTime.Core
{
    /// <summary>Una fila del ranking de un juego.</summary>
    public readonly struct RankingEntry
    {
        public string ProfileName { get; }
        public int Score { get; }

        public RankingEntry(string profileName, int score)
        {
            ProfileName = profileName;
            Score = score;
        }
    }

    /// <summary>
    /// Almacenamiento de usuarios y récords. El resto del programa solo conoce esta interfaz,
    /// de modo que se puede sustituir el JSON local por otra base de datos sin tocar los juegos.
    /// </summary>
    public interface IRecordsRepository
    {
        IReadOnlyList<string> GetProfileNames();
        bool ProfileExists(string profileName);
        void CreateProfile(string profileName);

        /// <summary>Mejor puntuación del usuario en el juego (0 si no tiene ninguna).</summary>
        int GetBestScore(string profileName, string gameId);

        /// <summary>Registra una puntuación. Devuelve true solo si supera el récord anterior.</summary>
        bool SubmitScore(string profileName, string gameId, int score);

        /// <summary>Mejores puntuaciones de todos los usuarios en un juego, de mayor a menor.</summary>
        IReadOnlyList<RankingEntry> GetRanking(string gameId, int maxEntries);
    }
}
