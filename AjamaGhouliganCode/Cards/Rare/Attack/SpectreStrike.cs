using AjamaGhouligan.AjamaGhouliganCode.Cards;
using AjamaGhouligan.AjamaGhouliganCode.DynamicVars;
using AjamaGhouligan.AjamaGhouliganCode.Powers;
using AjamaGhouligan.AjamaGhouliganCode.Utils;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.ValueProps;

namespace AjamaGhouligan.AjamaGhouliganCode.Cards.Rare.Attack;

public class SpectreStrike() : AjamaGhouliganCard(1,
    CardType.Attack, CardRarity.Rare,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(11, ValueProp.Move),
        new HauntVar(2)
    ];

    public override HashSet<CardTag> MyCanonicalTags =>
    [
        CardTag.Strike
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.CardAttack(this, play,
                1,
                "vfx/vfx_attack_slash")
            .Execute(choiceContext);

        MyActions.HauntRandomInPile(PileType.Discard, this);

        List<CardModel> hauntedCardsInDiscard = CardPile.GetCards(Owner, PileType.Discard)
            .Where(c => c.Keywords.Contains(MyEnums.Haunted))
            .ToList();

        await CardPileCmd.Add(hauntedCardsInDiscard, PileType.Draw, CardPilePosition.Top);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Haunt.UpgradeValueBy(1);
    }
}