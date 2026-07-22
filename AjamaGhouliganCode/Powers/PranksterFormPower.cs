using AjamaGhouligan.AjamaGhouliganCode.Cards.Token;
using AjamaGhouligan.AjamaGhouliganCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace AjamaGhouligan.AjamaGhouliganCode.Powers;

public class PranksterFormPower : AjamaGhouliganPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<GoofPower>(),
        HoverTipFactory.FromPower<MisfortunePower>()
    ];
    
    // I'm not using this hook for the main effect because I want the effects to apply after the card's been played and not before, since Goof hitting 10 causes a delay as it gives energy and card draw. And Surprise can be drawn and autoplayed when that happens, which would delay the actual card play even further.
    public override async Task AfterEnergySpent(CardModel card, int amount)
    {
        if (card.Owner.Creature != Owner) return;
        if (amount <= 0) return;
        
        // Spireverse's Captain lets you spend energy on an action that's not a card, so it needs special handling. Thankfully it calls this hook with a dummy card.
        if (card.Id.ToString() == "CARD.INTOTHESPIREVERSE-AMMO_VOLLEY")
        {
            await ApplyPowers(new ThrowingPlayerChoiceContext(), amount);
        }
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != Owner) return;
        if (cardPlay.Resources.EnergySpent <= 0) return;

        await ApplyPowers(choiceContext, cardPlay.Resources.EnergySpent);
    }

    private async Task ApplyPowers(PlayerChoiceContext choiceContext, int energySpent)
    {
        Flash();
        
        await PowerCmd.Apply<MisfortunePower>(choiceContext,
            CombatState.HittableEnemies, 
            Amount * energySpent,
            Owner, null);
            
        await PowerCmd.Apply<GoofPower>(choiceContext, 
            Owner, 
            Amount * energySpent, 
            Owner, null);
    }
}