using AjamaGhouligan.AjamaGhouliganCode.CardPiles;
using AjamaGhouligan.AjamaGhouliganCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace AjamaGhouligan.AjamaGhouliganCode.Powers;

public class JesterPower : AjamaGhouliganPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier,
        CardModel? cardSource)
    {
        if (power.Owner != Owner) return;
        if (power is not GoofPower) return;
        
        Flash();

        Player player = Owner.Player!;
        
        List<CardPoolModel> pools = player.UnlockState.CharacterCardPools.ToList();

        if (pools.Count > 1) pools.Remove(player.Character.CardPool);
        
        List<CardModel> cards = CardFactory.GetDistinctForCombat(player, pools
                    .SelectMany(
                        c => c.GetUnlockedCards(
                            player.UnlockState, 
                            player.RunState.CardMultiplayerConstraint)
                        ),
                Amount, player.RunState.Rng.CombatCardGeneration)
            .ToList();
        
        if (cards.Count == 0) return;
        
        MyActions.GainsBury(cards, false);
        
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardsToCombat(cards, SepulchrePile.PileType, player));
    }
}