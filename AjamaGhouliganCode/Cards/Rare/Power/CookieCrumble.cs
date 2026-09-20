using AjamaGhouligan.AjamaGhouliganCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace AjamaGhouligan.AjamaGhouliganCode.Cards.Rare.Power;

public class CookieCrumble() : AjamaGhouliganCard(1,
    CardType.Power, CardRarity.Rare,
    TargetType.Self)
{
    public override bool CanBeGeneratedInCombat => false;
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(8),
        new MaxHpVar(3)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "PowerUp", Owner.Character.PowerUpAnimDelay);

        CookieCrumblePower power = (CookieCrumblePower) ModelDb.Power<CookieCrumblePower>()
            .ToMutable();
        
        power.InitializeValues((int) DynamicVars.Cards.BaseValue, DynamicVars.MaxHp.BaseValue);

        await PowerCmd.Apply(choiceContext, power,
            Owner.Creature, DynamicVars.Cards.BaseValue,
            Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.MaxHp.UpgradeValueBy(1);
    }
}