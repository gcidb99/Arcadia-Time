using System.Collections.Generic;
using UnityEngine;

namespace ArcadiaTime.Core
{
    /// <summary>Punto de acceso de los juegos y del menú a los récords del usuario activo.</summary>
    public class RecordsService
    {
        private const string EditorTestProfile = "Editor";

        private readonly IRecordsRepository _repository;
        private readonly ProfileService _profiles;

        public RecordsService(IRecordsRepository repository, ProfileService profiles)
        {
            _repository = repository;
            _profiles = profiles;
        }

        /// <summary>Mejor puntuación del usuario activo en un juego (0 si no hay usuario o récord).</summary>
        public int GetBestScore(string gameId)
        {
            return _profiles.HasProfile ? _repository.GetBestScore(_profiles.CurrentProfile, gameId) : 0;
        }

        /// <summary>Registra la puntuación final de una partida. Devuelve true si es un nuevo récord.</summary>
        public bool SubmitScore(string gameId, int score)
        {
#if UNITY_EDITOR
            // Permite probar una escena de juego directamente, sin pasar antes por el menú.
            if (!_profiles.HasProfile && !_profiles.Select(EditorTestProfile))
            {
                _profiles.TryCreateAndSelect(EditorTestProfile, out _);
            }
#endif
            if (!_profiles.HasProfile)
            {
                Debug.LogWarning("[Records] No hay usuario activo; la puntuación no se guarda.");
                return false;
            }

            return _repository.SubmitScore(_profiles.CurrentProfile, gameId, score);
        }

        /// <summary>Ranking de todos los usuarios en un juego, de mayor a menor puntuación.</summary>
        public IReadOnlyList<RankingEntry> GetRanking(string gameId, int maxEntries = 10)
        {
            return _repository.GetRanking(gameId, maxEntries);
        }
    }
}
