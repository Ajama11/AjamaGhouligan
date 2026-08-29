using AjamaGhouligan.AjamaGhouliganCode.DynamicVars;
using AjamaGhouligan.AjamaGhouliganCode.Relics;
using AjamaGhouligan.AjamaGhouliganCode.Utils;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
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
        RelicRarity.Rare;

    private bool ShouldSummonAfterCardPlay { get; set; }
    private int AmountToSummonAfterCardPlay { get; set; }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        ..HalfSummon.MakeVars(1, 1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HalfSummon.DynamicTip(DynamicVars)
    ];

    public override Task AfterAttack(PlayerChoiceContext choiceContext, AttackCommand command)
    {
        if (command.Attacker?.Monster is not Osty || 
            command.Attacker?.PetOwner != Owner) return Task.CompletedTask;
        
        Flash();

        int numberOfHits = command.Results.Sum(list => list.Count);

        ShouldSummonAfterCardPlay = true;
        AmountToSummonAfterCardPlay = DynamicVars.HalfSummonFilled.IntValue * numberOfHits;
        
        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (ShouldSummonAfterCardPlay)
        {
            ShouldSummonAfterCardPlay = false;

            await MyActions.HalfSummon(this, Owner, 
                AmountToSummonAfterCardPlay, AmountToSummonAfterCardPlay,
                choiceContext);
            
            AmountToSummonAfterCardPlay = 0;
        }
    }
}