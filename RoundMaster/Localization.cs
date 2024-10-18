namespace RoundMaster.Localization {
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.IO;
    using Bsg.GameSettings;
    using Comfort.Common;

    internal class LocalizationManager {
        private readonly Dictionary<string, Dictionary<string, string>> table;
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
                    table[file.Name] = subTable;
                }
            }
        }
        internal bool TryLocalize(string language, string key, out string value) {
            value = null;
            return table.TryGetValue(language, out Dictionary<string, string> subTable) && subTable.TryGetValue(key, out value);
        }
    }

    internal class Text {
        private string str;
        internal static Text Localized(string key) {
            return new() {
                str = key
            };
        }
        public static implicit operator string(Text instance) {
            return instance.str;
        }
    }
}