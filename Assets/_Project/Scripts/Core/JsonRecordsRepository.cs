using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace ArcadiaTime.Core
{
    /// <summary>Guarda usuarios y récords en un archivo JSON dentro de Application.persistentDataPath.</summary>
    public class JsonRecordsRepository : IRecordsRepository
    {
        private const string FileName = "arcadia-time-save.json";

        private readonly string _path;
        private readonly SaveData _data;

        public JsonRecordsRepository() : this(Path.Combine(Application.persistentDataPath, FileName))
        {
        }

        public JsonRecordsRepository(string path)
        {
            _path = path;
            _data = Load();
        }

        public IReadOnlyList<string> GetProfileNames()
        {
            return _data.profiles
                .Select(p => p.name)
                .OrderBy(n => n, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        public bool ProfileExists(string profileName)
        {
            return FindProfile(profileName) != null;
        }

        public void CreateProfile(string profileName)
        {
            if (ProfileExists(profileName)) return;
            _data.profiles.Add(new ProfileData { name = profileName });
            Save();
        }

        public int GetBestScore(string profileName, string gameId)
        {
            ScoreEntry entry = FindEntry(FindProfile(profileName), gameId);
            return entry != null ? entry.bestScore : 0;
        }

        public bool SubmitScore(string profileName, string gameId, int score)
        {
            ProfileData profile = FindProfile(profileName);
            if (profile == null)
            {
                Debug.LogWarning($"[Records] El usuario '{profileName}' no existe.");
                return false;
            }

            if (score <= 0) return false;

            ScoreEntry entry = FindEntry(profile, gameId);
            if (entry != null && score <= entry.bestScore) return false;

            if (entry == null)
            {
                entry = new ScoreEntry { gameId = gameId };
                profile.scores.Add(entry);
            }

            entry.bestScore = score;
            entry.achievedAtUtc = DateTime.UtcNow.ToString("o");
            Save();
            return true;
        }

        public IReadOnlyList<RankingEntry> GetRanking(string gameId, int maxEntries)
        {
            return _data.profiles
                .Select(p => new { Profile = p, Entry = FindEntry(p, gameId) })
                .Where(x => x.Entry != null)
                .OrderByDescending(x => x.Entry.bestScore)
                .Take(maxEntries)
                .Select(x => new RankingEntry(x.Profile.name, x.Entry.bestScore))
                .ToList();
        }

        // ---------- Búsquedas ----------

        private ProfileData FindProfile(string profileName)
        {
            return _data.profiles.FirstOrDefault(
                p => string.Equals(p.name, profileName, StringComparison.OrdinalIgnoreCase));
        }

        private static ScoreEntry FindEntry(ProfileData profile, string gameId)
        {
            if (profile == null) return null;
            return profile.scores.FirstOrDefault(s => s.gameId == gameId);
        }

        // ---------- Lectura y escritura ----------

        private SaveData Load()
        {
            if (!File.Exists(_path)) return new SaveData();

            try
            {
                string json = File.ReadAllText(_path);
                SaveData data = JsonUtility.FromJson<SaveData>(json);
                if (data == null) return new SaveData();

                if (data.profiles == null) data.profiles = new List<ProfileData>();
                foreach (ProfileData profile in data.profiles)
                {
                    if (profile.scores == null) profile.scores = new List<ScoreEntry>();
                }
                return data;
            }
            catch (Exception e)
            {
                Debug.LogError($"[Records] No se pudo leer el guardado: {e.Message}");
                BackupCorruptFile();
                return new SaveData();
            }
        }

        private void Save()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_path));
                string tempPath = _path + ".tmp";
                File.WriteAllText(tempPath, JsonUtility.ToJson(_data, true));

                if (File.Exists(_path)) File.Replace(tempPath, _path, null);
                else File.Move(tempPath, _path);
            }
            catch (Exception e)
            {
                Debug.LogError($"[Records] No se pudo guardar: {e.Message}");
            }
        }

        private void BackupCorruptFile()
        {
            try
            {
                string backup = _path + ".corrupto-" + DateTime.UtcNow.ToString("yyyyMMddHHmmss");
                File.Copy(_path, backup);
                Debug.LogWarning($"[Records] Copia del archivo dañado en: {backup}");
            }
            catch (Exception e)
            {
                Debug.LogError($"[Records] No se pudo hacer copia del archivo dañado: {e.Message}");
            }
        }
    }
}
