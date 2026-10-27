using System;
using System.Collections.Generic;

namespace ArcadiaTime.Core
{
    /// <summary>Mejor puntuación de un usuario en un juego.</summary>
    [Serializable]
    public class ScoreEntry
    {
        public string gameId;
        public int bestScore;
        public string achievedAtUtc; // fecha ISO 8601
    }

    /// <summary>Un usuario y sus mejores puntuaciones.</summary>
    [Serializable]
    public class ProfileData
    {
        public string name;
        public List<ScoreEntry> scores = new List<ScoreEntry>();
    }

    /// <summary>Contenido completo del archivo de guardado.</summary>
    [Serializable]
    public class SaveData
    {
        public int version = 1;
        public List<ProfileData> profiles = new List<ProfileData>();
    }
}
