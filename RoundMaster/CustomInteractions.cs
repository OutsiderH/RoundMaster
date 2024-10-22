#pragma warning disable CS0618
namespace RoundMaster.CustomInteraction {
    using EFT.InventoryLogic;
    using EFT.UI;
    using IcyClawz.CustomInteractions;
    using System.Collections.Generic;
    using InGameStatus = GClass1864;

    internal sealed class MagazineInteractionsProvider : IItemCustomInteractionsProvider {
        public IEnumerable<CustomInteraction> GetCustomInteractions(ItemUiContext uiContext, EItemViewType viewType, Item item) {
            if (item is not MagazineClass magazine) {
                yield break;
            }
            //if (!InGameStatus.InRaid) {
            //    yield break;
            //}
            yield return new() {
                Caption = () => new Localization.LocalizedText("load_magazine"),
                Action = () => { },
            };
        }
    }
}