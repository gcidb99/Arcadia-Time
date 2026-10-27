using System;
using System.Collections.Generic;
using UnityEngine;

namespace ArcadiaTime.Core
{
    /// <summary>Gestiona la lista de usuarios y cuál es el usuario activo.</summary>
    public class ProfileService
    {
        public const int MinNameLength = 2;
        public const int MaxNameLength = 12;

        private const string LastProfileKey = "arcadia.lastProfile";

        private readonly IRecordsRepository _repository;

        public ProfileService(IRecordsRepository repository)
        {
            _repository = repository;
        }

        /// <summary>Usuario activo, o null si todavía no se ha elegido ninguno.</summary>
        public string CurrentProfile { get; private set; }

        public bool HasProfile => !string.IsNullOrEmpty(CurrentProfile);

        /// <summary>Se lanza al cambiar de usuario (null si se cierra la sesión).</summary>
        public event Action<string> ProfileChanged;

        /// <summary>Último usuario que se usó, si todavía existe; si no, null.</summary>
        public string LastUsedProfile
        {
            get
            {
                string last = PlayerPrefs.GetString(LastProfileKey, string.Empty);
                return _repository.ProfileExists(last) ? last : null;
            }
        }

        public IReadOnlyList<string> GetProfiles()
        {
            return _repository.GetProfileNames();
        }

        public static bool IsValidName(string name, out string error)
        {
            string trimmed = name == null ? string.Empty : name.Trim();

            if (trimmed.Length < MinNameLength)
            {
                error = $"El nombre debe tener al menos {MinNameLength} caracteres.";
                return false;
            }

            if (trimmed.Length > MaxNameLength)
            {
                error = $"El nombre no puede superar los {MaxNameLength} caracteres.";
                return false;
            }

            foreach (char c in trimmed)
            {
                bool allowed = char.IsLetterOrDigit(c) || c == ' ' || c == '_' || c == '-';
                if (!allowed)
                {
                    error = "Solo se permiten letras, números, espacios, guiones y guiones bajos.";
                    return false;
                }
            }

            error = null;
            return true;
        }

        /// <summary>Crea un usuario nuevo y lo deja como usuario activo.</summary>
        public bool TryCreateAndSelect(string name, out string error)
        {
            if (!IsValidName(name, out error)) return false;

            string trimmed = name.Trim();
            if (_repository.ProfileExists(trimmed))
            {
                error = "Ya existe un usuario con ese nombre.";
                return false;
            }

            _repository.CreateProfile(trimmed);
            return Select(trimmed);
        }

        /// <summary>Elige un usuario existente como usuario activo.</summary>
        public bool Select(string name)
        {
            if (!_repository.ProfileExists(name)) return false;

            CurrentProfile = name;
            PlayerPrefs.SetString(LastProfileKey, name);
            PlayerPrefs.Save();
            ProfileChanged?.Invoke(name);
            return true;
        }

        /// <summary>Cierra la sesión del usuario activo (no borra sus datos).</summary>
        public void Logout()
        {
            CurrentProfile = null;
            ProfileChanged?.Invoke(null);
        }
    }
}
