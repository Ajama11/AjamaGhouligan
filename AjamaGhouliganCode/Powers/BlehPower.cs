using AjamaGhouligan.AjamaGhouliganCode.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace AjamaGhouligan.AjamaGhouliganCode.Powers;

public class BlehPower : AjamaGhouliganPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        ..MyEnums.TreatHovers()
    ];

    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card.Owner.Creature != Owner || card.Type is not (CardType.Status or CardType.Curse)) return;

        List<CardDrawnEntry> entries = CombatManager.Instance.History.Entries
            .OfType<CardDrawnEntry>()
            .ToList();
        
        int statusesOrCursesDrawnThisTurn = entries
            .Count(IsStatusOrCurse);

        if (statusesOrCursesDrawnThisTurn <= Amount)
        {
            Flash();

            CardModel? statusOrCurse = entries
                .Where(IsStatusOrCurse)
                .ElementAtOrDefault(statusesOrCursesDrawnThisTurn - 1)
                ?.Card;

            if (statusOrCurse != null)
            {
                await CardCmd.Transform(
                    statusOrCurse, 
                    MyActions.CreateRandomTreatWithoutAddingToPile(Owner.Player!, CombatState));
            }
        }
    }

    private bool IsStatusOrCurse(CardDrawnEntry e)
    {
        return e.HappenedThisTurn(CombatState) && 
            e.Actor == Owner &&
            e.Card.Type is CardType.Status or CardType.Curse;
    }
}