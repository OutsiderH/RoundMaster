namespace RoundMaster.Localization {
    using global::RoundMaster.Patches;
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.IO;
    using Bsg.GameSettings;
    using System;

    internal class LocalizationManager {
        private readonly Dictionary<string, Dictionary<string, string>> table;
        private GameSetting<string> languageSetting;
        internal event Action onLanguageChanged;
        internal LocalizationManager() {
            table = [];
            DirectoryInfo localeDir = new("./BepInEx/Plugins/dev/locale");
            if (!localeDir.Exists) {
                RoundMaster.Instance.Logger.LogError("locale directory not found");
                return;
            }
            foreach (FileInfo file in localeDir.EnumerateFiles()) {
                if (file.Extension != ".json") {
                    continue;
                }
                using (StreamReader reader = file.OpenText()) {
                    JObject json = JsonConvert.DeserializeObject<JObject>(reader.ReadToEnd());
                    if (json == null) {
                        continue;
                    }
                    Dictionary<string, string> subTable = [];
                    foreach (KeyValuePair<string, JToken> pair in json) {
                        if (pair.Value.Type != JTokenType.String) {
                            continue;
                        }
                        subTable[pair.Key] = (string)pair.Value;
                    }
                    table[file.Name[..2]] = subTable;
                }
            }
            SettingInitCapture.onSettingInit += (setting) => {
                languageSetting = setting.Game.Settings.Language;
                languageSetting.Subscribe(newLanguage => {
                    onLanguageChanged?.Invoke();
                });
            };
        }
        internal bool TryLocalize(string key, out string value) {
            value = null;
            return table.TryGetValue(languageSetting, out Dictionary<string, string> subTable) && subTable.TryGetValue(key, out value);
        }
    }

    internal class LocalizedText {
        private readonly string key;
        private string str;
        internal LocalizedText(string key) {
            this.key = key;
            Refresh();
            RoundMaster.Instance.Localization.onLanguageChanged += Refresh;
        }
        ~LocalizedText() {
            RoundMaster.Instance.Localization.onLanguageChanged -= Refresh;
        }
        private void Refresh() {
            str = RoundMaster.Instance.Localization.TryLocalize(key, out string value) ? value : key;
        }
        public static implicit operator string(LocalizedText instance) {
            return instance.str;
        }
    }
}