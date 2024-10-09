namespace RoundMaster {
    using BepInEx;
    using BepInEx.Logging;
    using HarmonyLib;
    using System.Collections.Generic;

    [BepInPlugin(ModInfo.id, ModInfo.name, ModInfo.version)]
    public class RoundMaster : BaseUnityPlugin {
        private static ManualLogSource logger;
        private Harmony harmonyInstance;
        private List<Patches.BasePatch> patches;
        internal static new ManualLogSource Logger {
            get {
                return logger;
            }
        }
        private void Awake() {
            logger = new(ModInfo.name);
            harmonyInstance = new(ModInfo.id);
            patches = [new Patches.VanillaMagazinePresetFix()];
            foreach (Patches.BasePatch patch in patches) {
                patch.Enable(harmonyInstance);
            }
        }

        private void OnDestroy() {
            foreach (Patches.BasePatch patch in patches) {
                patch.Disable(harmonyInstance);
            }
        }
    }
}
