using AjamaGhouligan.AjamaGhouliganCode.BundledHoverTips;
using AjamaGhouligan.AjamaGhouliganCode.BundledHoverTips.Core;
using AjamaGhouligan.AjamaGhouliganCode.Cards;
using AjamaGhouligan.AjamaGhouliganCode.Cards.Token;
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

namespace AjamaGhouligan.AjamaGhouliganCode.Cards.Rare.Power;

public class SpectreForm() : AjamaGhouliganCard(3,
    CardType.Power, CardRarity.Rare,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<SpectreFormPower>(10)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        
    ];

    public override BundledHoverTipManager MyBundles =>
    [
        BundledHoverTipFactory.FromKeyword(MyEnums.Haunted),
        BundledHoverTipFactory.FromKeyword(MyEnums.Bury)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "PowerUp", Owner.Character.PowerUpAnimDelay);

        await CommonActions.ApplySelf<SpectreFormPower>(choiceContext, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Power<SpectreFormPower>().UpgradeValueBy(-2);
    }
}