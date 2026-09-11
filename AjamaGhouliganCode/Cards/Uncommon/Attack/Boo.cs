using AjamaGhouligan.AjamaGhouliganCode.BundledHoverTips;
using AjamaGhouligan.AjamaGhouliganCode.BundledHoverTips.Core;
using AjamaGhouligan.AjamaGhouliganCode.Cards;
using AjamaGhouligan.AjamaGhouliganCode.DynamicVars;
using AjamaGhouligan.AjamaGhouliganCode.Powers;
using AjamaGhouligan.AjamaGhouliganCode.Utils;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Audio.Debug;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.ValueProps;

namespace AjamaGhouligan.AjamaGhouliganCode.Cards.Uncommon.Attack;

public class Boo() : AjamaGhouliganCard(2,
    CardType.Attack, CardRarity.Uncommon,
    TargetType.AnyEnemy)
{
    private const string Threshold = "Threshold";
    private const string CalculatedTriggers = "CalculatedTriggers";
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6, DamageProps.card),
        new RepeatVar(2),
        new IntVar(Threshold, 10),
        ..MakeCalculatedVar(CalculatedTriggers, 0, (card, _) =>
            Math.Floor(
                CombatManager.Instance.History.Entries
                    .OfType<PowerReceivedEntry>()
                    .Where(e =>
                        e.Power is DoomPower &&
                        e.Applier == card.Owner.Creature &&
                        e.Amount > 0)
                    .Sum(e => e.Amount)
                / card.DynamicVars[Threshold].BaseValue
            ))
    ];

    public override BundledHoverTipManager MyBundles =>
    [
        new UnfortunateBundle()
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.CardAttack(this, play,
                DynamicVars.Repeat.IntValue,
                VfxCmd.dramaticStabPath,
                tmpSfx: TmpSfx.heavyAttack)
            .Execute(choiceContext);

        await UnfortunateSingleton.Trigger(
            CombatState!,
            (int) ((CalculatedVar) DynamicVars[CalculatedTriggers]).Calculate(null),
            choiceContext, play);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Repeat.UpgradeValueBy(1);
        DynamicVars[Threshold].UpgradeValueBy(-2);
    }
}