using AjamaGhouligan.AjamaGhouliganCode.Cards.Token;
using AjamaGhouligan.AjamaGhouliganCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace AjamaGhouligan.AjamaGhouliganCode.Powers;

public class PranksterSurprisePower : BasePranksterPower
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Surprise>()
    ];
    
    protected override async Task DoTheThing(PlayerChoiceContext choiceContext, int energySpent, int powerAmountOverride = -1)
    {
        int amount = powerAmountOverride == -1 ? Amount : powerAmountOverride;
        
        Flash();
        
        await MyActions.CreateSurprises(
            amount * energySpent, Owner.Player!, CombatState,
            PileType.Discard, CardPilePosition.Bottom, previewTime: 0.6f);
    }
}