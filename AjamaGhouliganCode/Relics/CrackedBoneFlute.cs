using AjamaGhouligan.AjamaGhouliganCode.Relics;
using AjamaGhouligan.AjamaGhouliganCode.Utils;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Monsters;

namespace AjamaGhouligan.AjamaGhouliganCode.Relics;

public class CrackedBoneFlute() : AjamaGhouliganRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Common;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new SummonVar(1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.Static(StaticHoverTip.SummonDynamic, DynamicVars.Summon)
    ];

    public override async Task AfterAttack(PlayerChoiceContext choiceContext, AttackCommand command)
    {
        if (command.Attacker?.Monster is not Osty || 
            command.Attacker?.PetOwner != Owner) return;
        
        Flash();

        int numberOfHits = command.Results.Sum(list => list.Count);

        await MyActions.Summon(this, Owner, DynamicVars.Summon.IntValue * numberOfHits, choiceContext);
    }
}