using AjamaGhouligan.AjamaGhouliganCode.Utils;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace AjamaGhouligan.AjamaGhouliganCode.Powers;

public class GoofballFormPower : AjamaGhouliganPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust),
        HoverTipFactory.FromPower<GoofPower>()
    ];

    public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal)
    {
        if (card.Owner.Creature != Owner) return;

        if (causedByEthereal)
        {
            GetInternalData<Data>().EtherealCount++;
        }
        else
        {
            Flash();
            
            await PowerCmd.Apply<GoofPower>(choiceContext, 
                Owner, Amount,
                Owner, null);
        }
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner)) return;

        Data data = GetInternalData<Data>();
        
        if (data.EtherealCount > 0) Flash();
        
        await PowerCmd.Apply<GoofPower>(choiceContext, 
            Owner, Amount * data.EtherealCount,
            Owner, null);

        data.EtherealCount = 0;
    }

    protected override object InitInternalData() => new Data();

    /// <summary>
    /// Just like Dark Embrace, if Ethereal cards are Exhausted at the end of the turn, give the cards after the flush occurs. The energy gets wasted, but that's much better than both the energy and the cards being wasted.
    /// </summary>
    public class Data
    {
        public int EtherealCount;
    }
}