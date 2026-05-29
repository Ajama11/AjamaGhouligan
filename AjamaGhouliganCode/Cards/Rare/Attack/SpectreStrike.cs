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
        new DamageVar(11, ValueProp.Move)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    public override HashSet<CardTag> MyCanonicalTags =>
    [
        CardTag.Strike
    ];

    public override IEnumerable<IHoverTip> MyHoverTips =>
    [
        HoverTipFactory.FromKeyword(CardKeyword.Ethereal),
        HoverTipFactory.Static(MyEnums.Haunt),
        HoverTipFactory.Static(MyEnums.BuryOther)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.CardAttack(this, play,
                1,
                "vfx/vfx_attack_slash")
            .Execute(choiceContext);

        CardModel? randomCard = MyActions.GetRandomCards(this, PileType.Draw,
                c => 
                    !c.Keywords.Contains(CardKeyword.Ethereal) && 
                    !c.Keywords.Contains(CardKeyword.Retain))
            .FirstOrDefault()
            ??
            MyActions.GetRandomCards(this, PileType.Draw,
                c => 
                    !c.Keywords.Contains(CardKeyword.Ethereal))
            .FirstOrDefault();

        randomCard?.AddKeyword(CardKeyword.Ethereal);

        List<CardModel> etherealCards = Owner.PlayerCombatState!.AllCards
            .Where(c => 
                c.Keywords.Contains(CardKeyword.Ethereal) &&
                c.Pile != null &&
                c.Pile.Type != PileType.Exhaust)
            .ToList();

        await MyActions.HauntAndBurySpecific(etherealCards);
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}