using AjamaGhouligan.AjamaGhouliganCode.Powers;
using AjamaGhouligan.AjamaGhouliganCode.Utils;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace AjamaGhouligan.AjamaGhouliganCode.Cards.Ancient;

public class Eclipse() : AjamaGhouliganCard(2,
    CardType.Power, CardRarity.Ancient,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<EclipsePower>(1)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        
    ];

    public override IEnumerable<IHoverTip> MyHoverTips =>
    [
        HoverTipFactory.FromKeyword(MyEnums.Haunted)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "PowerUp", Owner.Character.PowerUpAnimDelay);

        await CommonActions.ApplySelf<EclipsePower>(choiceContext, this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}