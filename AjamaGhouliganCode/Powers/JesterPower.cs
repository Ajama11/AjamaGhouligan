using AjamaGhouligan.AjamaGhouliganCode.CardPiles;
using AjamaGhouligan.AjamaGhouliganCode.Cards.Basic;
using AjamaGhouligan.AjamaGhouliganCode.Cards.Token;
using AjamaGhouligan.AjamaGhouliganCode.Utils;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
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
                HoverTipFactory.FromCard<Cavort>(),
                HoverTipFactory.FromCard(strike)
            ];
        }
    }
    
    public override async Task AfterCurrentHpChanged(Creature creature, decimal delta)
    {
        if (delta >= 0 || creature.Monster is not Osty || creature.PetOwner != Owner.Player || Owner.Player == null) return;
       
        Flash();

        await PowerCmd.Apply<GoofPower>(new ThrowingPlayerChoiceContext(),
            Owner, Amount,
            Owner, null);
        
        await MyActions.CreateCards(ModelDb.Card<Strike>(),
            Amount, Owner.Player!, Owner.CombatState!, SepulchrePile.PileType, modifyCardsBeforePreview: list =>
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