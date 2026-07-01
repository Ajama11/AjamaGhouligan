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
using MegaCrit.Sts2.Core.ValueProps;

namespace AjamaGhouligan.AjamaGhouliganCode.Cards.Uncommon.Attack;

public class PianoDrop() : AjamaGhouliganCard(2,
    CardType.Attack, CardRarity.Uncommon,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(14, ValueProp.Move),
        new PowerVar<MisfortunePower>(6)
    ]; 
    
    public override IEnumerable<IHoverTip> MyHoverTips =>
    [

    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);

        await CommonActions.CardAttack(this, play,
                1,
                "vfx/vfx_heavy_blunt",
                null,
                "blunt_attack.mp3")
            .WithHitVfxSpawnedAtBase()
            .Execute(choiceContext);

        await MyActions.Misfortune(choiceContext, CombatState!.HittableEnemies, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Power<MisfortunePower>().UpgradeValueBy(1);
    }
}