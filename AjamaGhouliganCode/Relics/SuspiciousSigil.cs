using AjamaGhouligan.AjamaGhouliganCode.Enchantments;
using AjamaGhouligan.AjamaGhouliganCode.Relics;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace AjamaGhouligan.AjamaGhouliganCode.Relics;

public class SuspiciousSigil() : AjamaGhouliganRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Shop;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(3)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        ..HoverTipFactory.FromEnchantment<Occult>()
    ];

    public override async Task AfterObtained()
    {
        CardSelectorPrefs prefs =
            new (CardSelectorPrefs.EnchantSelectionPrompt, 
                0, DynamicVars.Cards.IntValue)
            {
                Cancelable = false,
                RequireManualConfirmation = true
            };

        foreach (CardModel card in await CardSelectCmd.FromDeckForEnchantment(Owner, ModelDb.Enchantment<Occult>(), DynamicVars.Cards.IntValue, prefs))
        {
            CardCmd.Enchant<Occult>(card, 1);
            CardCmd.Preview(card);
        }
    }
}