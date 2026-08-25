using AjamaGhouligan.AjamaGhouliganCode.CardPiles;
using AjamaGhouligan.AjamaGhouliganCode.Cards.Basic;
using AjamaGhouligan.AjamaGhouliganCode.Cards.Token;
using AjamaGhouligan.AjamaGhouliganCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;

namespace AjamaGhouligan.AjamaGhouliganCode.Powers;

public class JesterPower : AjamaGhouliganPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            CardModel strike = ModelDb.Card<Strike>().ToMutable();
            
            strike.EnergyCost.SetThisCombat(0);
            strike.AddKeyword(MyEnums.Bury);
            
            return
            [
                HoverTipFactory.FromPower<GoofPower>(),
                HoverTipFactory.FromKeyword(MyEnums.Bury),
                HoverTipFactory.FromCard(strike),
                HoverTipFactory.FromCard<Cavort>()
            ];
        }
    }

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier,
        CardModel? cardSource)
    {
        if (power.Owner != Owner) return;
        if (power is not GoofPower) return;
        if (amount <= 0) return;
        
        Flash();

        Player player = Owner.Player!;
        
        await MyActions.CreateCards(ModelDb.Card<Strike>(),
            Amount, player, Owner.CombatState!, SepulchrePile.PileType, modifyCardsBeforePreview: list =>
            {
                foreach (var card in list)
                {
                    card.EnergyCost.SetThisCombat(0);
                }
                
                MyActions.GainsBury(list, false);
                
                return list;
            });
    }
}