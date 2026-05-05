using AjamaGhouligan.AjamaGhouliganCode.CardPiles;
using AjamaGhouligan.AjamaGhouliganCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace AjamaGhouligan.AjamaGhouliganCode.Cards;

public class Test() : AjamaGhouliganCard(0,
    CardType.Skill, CardRarity.Basic,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var snapshottedSepulchre = CardPile.Get(SepulchrePile.PileType, Owner)!.Cards.ToList();
        
        foreach (CardModel card in CardPile.Get(PileType.Hand, Owner)!.Cards.ToList())
        {
            await CardPileCmd.Add(card, SepulchrePile.PileType);
        }
        
        foreach (CardModel card in snapshottedSepulchre)
        {
            await CardPileCmd.Add(card, PileType.Hand);
        }
    }

    protected override void OnUpgrade()
    {

    }
}