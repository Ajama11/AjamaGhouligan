using AjamaGhouligan.AjamaGhouliganCode.Cards;
using AjamaGhouligan.AjamaGhouliganCode.DynamicVars;
using AjamaGhouligan.AjamaGhouliganCode.Powers;
using AjamaGhouligan.AjamaGhouliganCode.Utils;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace AjamaGhouligan.AjamaGhouliganCode.Cards.Rare.Attack;

public class Leashed() : AjamaGhouliganCard(1,
    CardType.Attack, CardRarity.Rare,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<DoomPower>(3),
        new CalculationBaseVar(4),
        new ExtraDamageVar(1).FromOsty(),
        new CalculatedDamageVar(ValueProp.Move).FromOsty()
            .WithMultiplier((card, _) => 
                card.Owner.Osty is not { IsAlive: true } ? 0 : card.Owner.Osty.CurrentHp)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await MyActions.SelfDoom(choiceContext, this);
        
        if (!Osty.CheckMissingWithAnim(Owner))
        {
            await DamageCmd.Attack(DynamicVars.CalculatedDamage)
                .FromOsty(Owner.Osty!, this, play)
                .Targeting(play.Target!)
                .WithHitFx("vfx/vfx_attack_blunt", tmpSfx: "blunt_attack.mp3")
                .Execute(choiceContext);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Power<DoomPower>().UpgradeValueBy(-1);
    }
}