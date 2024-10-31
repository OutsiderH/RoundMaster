#pragma warning disable CS0618
namespace RoundMaster.CustomInteraction {
    using global::RoundMaster.Extension;
    using EFT.InventoryLogic;
    using EFT.UI;
    using IcyClawz.CustomInteractions;
    using System.Collections.Generic;
    using Comfort.Common;
    using System.Linq;
    using InGameStatus = GClass1864;

    internal sealed class MagazineInteractionsProvider : IItemCustomInteractionsProvider {
        public IEnumerable<CustomInteraction> GetCustomInteractions(ItemUiContext uiContext, EItemViewType viewType, Item item) {
            if (item is not MagazineClass magazine) {
                yield break;
            }
            if (!InGameStatus.InRaid) {
                //yield break;
            }
            List<MagazineBuildPresetClass> availablePresets = [];
            foreach (MagazineBuildPresetClass preset in uiContext.Session.MagBuildsStorage.Presets) {
                if (preset.CanApplyTo(magazine)) {
                    availablePresets.Add(preset);
                }
            }
            yield return new() {
                Caption = () => new Localization.LocalizedText("load_preset"),
                Enabled = () => uiContext.Session.Profile.CheckedMagazines.ContainsKey(magazine.Id) || availablePresets.Count > 0,
                Error = () => new Localization.LocalizedText("no_available_preset"),
                SubMenu = () => new SelectPresetSubInteractions(uiContext, magazine, availablePresets),
            };
        }
    }
    internal sealed class SelectPresetSubInteractions : CustomSubInteractions {
        internal SelectPresetSubInteractions(ItemUiContext uiContext, MagazineClass magazine, IEnumerable<MagazineBuildPresetClass> availablePresets) : base(uiContext) {
            IEnumerable<Item> allBullets = uiContext.Session.Profile.Inventory.Equipment.GetNotMergedItems().Where(item => item is BulletClass && !item.Parent.Container.IsMagazine() && !item.Parent.Container.IsWeaponAttachmentSlot());
            foreach (MagazineBuildPresetClass preset in availablePresets) {
                Add(new() {
                    Caption = () => preset.Name,
                    Action = () => {
                        Singleton<GUISounds>.Instance.PlayUISound(EUISoundType.ButtonClick);
                        while (magazine.Cartridges.Count < magazine.Cartridges.MaxCount) {
                            magazine.GetNextBullet(preset);
                        }
                    }
                });
            }
        }
    }
}