using AjamaGhouligan.AjamaGhouliganCode.Utils;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace AjamaGhouligan.AjamaGhouliganCode.CardMods;

public class SafeKeepingMod : CardModifier, ICustomModel, IOnDisinter
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar(1)
    ];

    public override void ModifyDescription(Creature? target, ref string description)
    {
        description = description + "\n" + GetLoc().GetFormattedText();
    }

    public override void AddTips(List<IHoverTip> tips)
    {
        tips.Add(HoverTipFactory.Static(MyEnums.Disinter));
    }

    public async Task OnDisinter(CardModel card)
    {
        if (card != Owner) return;
        
        card.EnergyCost.AddUntilPlayed(-1);

        RemoveModifier(Owner, this);
    }
}