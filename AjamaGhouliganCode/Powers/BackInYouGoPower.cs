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

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.Static(MyEnums.Haunt),
        HoverTipFactory.FromKeyword(MyEnums.Haunted),
        HoverTipFactory.Static(MyEnums.BuryOther)
    ];

    public async Task AfterSepulchreAutoplayOnTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player) return;
        
        List<CardModel> cards = MyActions.GetRandomCards(Owner.Player, PileType.Discard, _ => true, Amount);
        
        if (cards.Count == 0) return;

        await MyActions.HauntAndBurySpecific(cards);
    }
}