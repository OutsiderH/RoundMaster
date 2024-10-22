namespace RoundMaster {
    using global::RoundMaster.CustomInteraction;
    using global::RoundMaster.Localization;
    using global::RoundMaster.Patches;
    using BepInEx;
    using BepInEx.Logging;
    using HarmonyLib;
    using IcyClawz.CustomInteractions;

    [BepInPlugin(ModInfo.id, ModInfo.name, ModInfo.version)]
    public sealed class RoundMaster : BaseUnityPlugin {
        private static RoundMaster _instance;
        private ManualLogSource _logger;
        private Harmony harmonyInstance;
        private LocalizationManager localization;
        private BasePatch[] patches;
        internal static RoundMaster Instance {
            get => _instance;
            private set => _instance = value;
        }
        internal new ManualLogSource Logger {
            get => _logger;
            private set => _logger = value;
        }
        internal LocalizationManager Localization {
            get => localization;
            private set => localization = value;
        }
        private void Awake() {
            Instance = this;
            Logger = base.Logger;
            harmonyInstance = new(ModInfo.id);
            Localization = new();
            patches = [
                new VanillaMagazinePresetFix(),
                new SettingInitCapture()
            ];
            foreach (BasePatch patch in patches) {
                patch.Enable(harmonyInstance);
            }
            CustomInteractionsManager.Register(new MagazineInteractionsProvider());
        }

        private void OnDestroy() {
            foreach (BasePatch patch in patches) {
                patch.Disable(harmonyInstance);
            }
            Logger.Dispose();
        }
    }
}
