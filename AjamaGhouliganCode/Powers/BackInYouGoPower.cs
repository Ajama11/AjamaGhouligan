using AjamaGhouligan.AjamaGhouliganCode.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace AjamaGhouligan.AjamaGhouliganCode.Powers;

public class BackInYouGoPower : AjamaGhouliganPower, IAfterSepulchreAutoplayOnTurnStart
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public async Task AfterSepulchreAutoplayOnTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player) return;

        List<CardModel> cards = [];
        
        cards =
        [
            ..cards,
            ..MyActions.GetRandomCards(Owner.Player, PileType.Discard,
                c => c.Keywords.Contains(MyEnums.Haunted), Amount)
        ];
        
        cards =
        [
            ..cards,
            ..MyActions.GetRandomCards(Owner.Player, PileType.Discard,
                c => !c.Keywords.Contains(MyEnums.Haunted), Amount)
        ];
        
        if (cards.Count == 0) return;

        await MyActions.BurySpecific(cards);
    }
}