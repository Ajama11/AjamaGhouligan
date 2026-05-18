using AjamaGhouligan.AjamaGhouliganCode.Cards;
using AjamaGhouligan.AjamaGhouliganCode.DynamicVars;
using AjamaGhouligan.AjamaGhouliganCode.Powers;
using AjamaGhouligan.AjamaGhouliganCode.Utils;
using BaseLib.Extensions;
using BaseLib.Patches.Features;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace AjamaGhouligan.AjamaGhouliganCode.Cards.Uncommon.Attack;

public class Spit() : AjamaGhouliganCard(0,
    CardType.Attack, CardRarity.Uncommon,
    CustomTargetType.AllLowestHpEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(4, ValueProp.Move),
        new RepeatVar(2)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        MyEnums.Unfortunate
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.CardAttack(this, play,
                WasDoomAppliedThisTurn ? DynamicVars.Repeat.IntValue : 1,
                "vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }
    
    public bool WasDoomAppliedThisTurn =>
        CombatManager.Instance.History.Entries
            .OfType<PowerReceivedEntry>()
            .Any((Func<PowerReceivedEntry, bool>) (e => 
                e.HappenedThisTurn(CombatState) && 
                e.Power is DoomPower && 
                e.Applier == Owner.Creature));

    protected override bool ShouldGlowGoldInternal => WasDoomAppliedThisTurn;

    protected override void OnUpgrade()
    {
        DynamicVars.Repeat.UpgradeValueBy(1);
    }
}