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
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.ValueProps;

namespace AjamaGhouligan.AjamaGhouliganCode.Cards.Uncommon.Attack;

public class PianoDrop() : AjamaGhouliganCard(2,
    CardType.Attack, CardRarity.Uncommon,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(15, ValueProp.Move),
        new PowerVar<MisfortunePower>(6),
        new CardsVar(2)
    ]; 
    
    public override IEnumerable<IHoverTip> MyHoverTips =>
    [
        HoverTipFactory.FromCard<Dazed>()
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

        await MyActions.CreateCards(ModelDb.Card<Dazed>(), DynamicVars.Cards.IntValue, this, PileType.Discard);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2);
        DynamicVars.Power<MisfortunePower>().UpgradeValueBy(1);
    }
}