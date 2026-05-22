using AjamaGhouligan.AjamaGhouliganCode.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace AjamaGhouligan.AjamaGhouliganCode.Powers;

public class BlehPower : AjamaGhouliganPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card.Owner.Creature != Owner || card.Type != CardType.Status) return;

        int statusesDrawnThisTurn = CombatManager.Instance.History.Entries.OfType<CardDrawnEntry>()
            .Count(e =>
            e.HappenedThisTurn(CombatState) && 
            e.Actor == Owner && 
            e.Card.Type == CardType.Status);

        if (statusesDrawnThisTurn <= 1)
        {
            Flash();

            CardModel? status = CombatManager.Instance.History.Entries
                .OfType<CardDrawnEntry>()
                .FirstOrDefault(e => 
                    e.HappenedThisTurn(CombatState) && 
                    e.Actor == Owner && 
                    e.Card.Type == CardType.Status)
                ?.Card;

            if (status != null) await MyActions.BurySpecific(status);
            
            await CardPileCmd.Draw(choiceContext, Amount, Owner.Player!);
        }
    }
}