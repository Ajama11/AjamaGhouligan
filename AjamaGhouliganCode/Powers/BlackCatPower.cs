using AjamaGhouligan.AjamaGhouliganCode.Cards.Status;
using AjamaGhouligan.AjamaGhouliganCode.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace AjamaGhouligan.AjamaGhouliganCode.Powers;

public class BlackCatPower : AjamaGhouliganPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        if (player != Owner.Player) return;
        
        Flash();

        await MyActions.CreateCards(ModelDb.Card<Scorn>(), Amount, Owner.Player, CombatState, modifyCardsBeforePreview:
            list =>
            {
                foreach (var scorn in list)
                {
                    scorn.AddKeyword(CardKeyword.Ethereal);
                }
                return list;
            });
    }
}