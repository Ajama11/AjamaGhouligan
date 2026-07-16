using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace AjamaGhouligan.AjamaGhouliganCode.Cards.Uncommon.Skill;

public class Switcheroo() : AjamaGhouliganCard(0,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self)
{
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        List<CardModel> snapshottedDrawPile = CardPile.GetCards(Owner, PileType.Draw).ToList();

        await CardPileCmd.Shuffle(choiceContext, Owner);

        await CardPileCmd.Add(snapshottedDrawPile, PileType.Discard, CardPilePosition.Random);

        if (!(Keywords.Contains(CardKeyword.Exhaust) || ExhaustOnNextPlay))
            await CardPileCmd.Add(this, PileType.Draw, CardPilePosition.Random);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}