using AjamaGhouligan.AjamaGhouliganCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace AjamaGhouligan.AjamaGhouliganCode.Powers;

public class PranksterMisfortunePower : BasePranksterPower
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<MisfortunePower>(),
    ];
    
    protected override async Task DoTheThing(PlayerChoiceContext choiceContext, int energySpent, int powerAmountOverride = -1)
    {
        int amount = powerAmountOverride == -1 ? Amount : powerAmountOverride;
        
        Flash();
        
        await PowerCmd.Apply<MisfortunePower>(choiceContext,
            CombatState.HittableEnemies, 
            amount * energySpent,
            Owner, null);
    }
}