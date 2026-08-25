using AjamaGhouligan.AjamaGhouliganCode.Cards;
using AjamaGhouligan.AjamaGhouliganCode.DynamicVars;
using AjamaGhouligan.AjamaGhouliganCode.Powers;
using AjamaGhouligan.AjamaGhouliganCode.Utils;
using BaseLib.Cards.Variables;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace AjamaGhouligan.AjamaGhouliganCode.Cards.Token;

[Pool(typeof(TokenCardPool))]
public class Cavort() : AjamaGhouliganCard(0,
    CardType.Skill, CardRarity.Token,
    TargetType.None)
{
    public string FormattedCardName = "nothing";
    
    public CardModel? Card
    {
        get;
        set
        {
            field = value;
            FormattedCardName = field != null ?
                $"[gold]{field.Title}[/gold]" :
                "nothing";
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DisplayVar<Cavort>("FormattedCardName", cavort => cavort.FormattedCardName)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    public override IEnumerable<IHoverTip> MyHoverTips =>
        Card != null ?
            [HoverTipFactory.FromCard(Card)] :
            [];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        if (Card != null)
        {
            await CardCmd.AutoPlay(choiceContext, Card.CreateDupe(Owner), null);
        }
    }

    public override Task AfterCardEnteredCombat(CardModel card)
    {
        if (card != this) return Task.CompletedTask;
        
        SetLastPlayedCard();
        
        return Task.CompletedTask;
    }

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (IsExhausted) return Task.CompletedTask;
        
        SetLastPlayedCard();
        
        return Task.CompletedTask;
    }

    public override Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? clonedBy)
    {
        if (card != this) return Task.CompletedTask;
        if (oldPileType != PileType.Exhaust) return Task.CompletedTask;
        
        SetLastPlayedCard();
        
        return Task.CompletedTask;
    }

    private void SetLastPlayedCard()
    {
        Card = CombatManager.Instance.History.CardPlaysFinished.LastOrDefault(e =>
                e.CardPlay.Player == Owner &&
                e.CardPlay.Card.Type is CardType.Attack or CardType.Skill &&
                !e.CardPlay.Card.IsDupe &&
                e.CardPlay.Card is not Cavort)?
            .CardPlay.Card;
    }

    private bool IsExhausted => Pile is { Type: PileType.Exhaust };

    protected override void AddExtraArgsToDescription(LocString description)
    {
        description.Add("IsExhausted", IsExhausted);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}