namespace RoundMaster.Extension {
    using EFT.InventoryLogic;
    using System.Linq;
    using PresetItem = MagazineBuildPresetClass.GClass2109;

    public static class Extension {
        public static bool CanApplyTo(this MagazineBuildPresetClass instance, MagazineClass magazine) {
            foreach (PresetItem item in instance.Items) {
                if (item == null) {
                    continue;
                }
                if (!magazine.Cartridges.Filters.CheckFilter(item.TemplateId.ToString())) {
                    return false;
                }
            }
            return true;
        }
        public static bool CheckFilter(this ItemFilter[] instance, string templateId) {
            foreach (ItemFilter filter in instance) {
                if (filter.ExcludedFilter?.Contains(templateId) ?? false) {
                    return false;
                }
                if (filter.Filter?.Contains(templateId) ?? false) {
                    return true;
                }
            }
            return false;
        }
        public static bool IsMagazine(this IContainer container) {
            return container is StackSlot stackSlot && stackSlot.ParentItem is MagazineClass;
        }
        public static bool IsWeaponAttachmentSlot(this IContainer container) {
            return container is Slot slot && slot.ParentItem is Weapon;
        }
        public static string GetNextBullet(this MagazineClass magazine, MagazineBuildPresetClass preset) {
            throw new System.NotImplementedException();
        }
    }
}