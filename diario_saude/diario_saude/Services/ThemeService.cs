using System;
using ReactiveUI;
using System.Reactive;
using System.Runtime.CompilerServices;
using System.ComponentModel;
namespace diario_saude.Services
{
    public static class ThemeService
    {
        private static string _preference = "Light";
        public static string Preference
        {
            get => _preference;
            set
            {
                if (_preference == value) return;
                _preference = value;
                PreferenceChanged?.Invoke(_preference);
            }
        }

        public static event Action<string>? PreferenceChanged;

        public static void Toggle() =>
            Preference = Preference == "Light" ? "Dark" : "Light";
    }
}
