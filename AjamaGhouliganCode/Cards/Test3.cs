using AjamaGhouligan.AjamaGhouliganCode.CardPiles;
using AjamaGhouligan.AjamaGhouliganCode.Cards;
using AjamaGhouligan.AjamaGhouliganCode.Utils;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace AjamaGhouligan.AjamaGhouliganCode.Cards;

public class Test3() : AjamaGhouliganCard(0,
    CardType.Skill, CardRarity.Basic,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    public override IEnumerable<CardKeyword> CanonicalKeywords => 
    [
        MyEnums.Haunted
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.Heal(Owner.Creature, 2);

        CardModel? card = await CommonActions.SelectSingleCard(this, SepulchrePile.SelectionPrompt.HauntAndBury, choiceContext, PileType.Draw);

        if (card != null)
        {
            CardCmd.ApplyKeyword(card, MyEnums.Haunted);
            CardCmd.ApplyKeyword(card, MyEnums.Bury);
        }
    }

    protected override void OnUpgrade()
    {

    }
}