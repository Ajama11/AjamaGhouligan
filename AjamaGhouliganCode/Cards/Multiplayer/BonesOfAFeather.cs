using AjamaGhouligan.AjamaGhouliganCode.BundledHoverTips.Core;
using AjamaGhouligan.AjamaGhouliganCode.Cards;
using AjamaGhouligan.AjamaGhouliganCode.DynamicVars;
using AjamaGhouligan.AjamaGhouliganCode.Powers;
using AjamaGhouligan.AjamaGhouliganCode.Utils;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.ValueProps;

namespace AjamaGhouligan.AjamaGhouliganCode.Cards.Multiplayer;

public class BonesOfAFeather() : AjamaGhouliganCard(1,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.AllAllies),
    IOnDisinter
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new SummonVar(3)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        MyEnums.Bury
    ];

    public override BundledHoverTipManager MyBundles =>
    [
        BundledHoverTipFactory.Static(MyEnums.Disinter),
        BundledHoverTipFactory.FromKeyword(MyEnums.Haunted)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        foreach (Creature creature in CombatState!
                     .GetTeammatesOf(Owner.Creature)
                     .Where(c => c is { IsAlive: true, IsPlayer: true })
                     .ToList())
        {
            await MyActions.Summon(this, creature.Player!, DynamicVars.Summon.IntValue, choiceContext);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Summon.UpgradeValueBy(2);
    }

    public async Task OnDisinter(CardModel card)
    {
        if (card != this) return;
        
        card.AddKeyword(MyEnums.Haunted);
    }
}