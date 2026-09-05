using AjamaGhouligan.AjamaGhouliganCode.BundledHoverTips;
using AjamaGhouligan.AjamaGhouliganCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace AjamaGhouligan.AjamaGhouliganCode.Powers;

public class FemurFeverPower : AjamaGhouliganPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        ..new HauntBundle().HoverTips,
        HoverTipFactory.Static(MyEnums.Bury)
    ];

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Player != Owner.Player) return;
        if (!cardPlay.Card.Tags.Contains(CardTag.OstyAttack)) return;
        
        await CardPileCmd.ShuffleIfNecessary(choiceContext, Owner.Player);

        List<CardModel> topCards = Owner.Player.PlayerCombatState!.DrawPile.Cards.Take(Amount).ToList();

        await MyActions.HauntAndBurySpecific(topCards);
    }
}