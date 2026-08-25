using AjamaGhouligan.AjamaGhouliganCode.CardPiles;
using AjamaGhouligan.AjamaGhouliganCode.Cards;
using AjamaGhouligan.AjamaGhouliganCode.DynamicVars;
using AjamaGhouligan.AjamaGhouliganCode.Powers;
using AjamaGhouligan.AjamaGhouliganCode.Utils;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.ValueProps;

namespace AjamaGhouligan.AjamaGhouliganCode.Cards.Rare.Skill;

public class Tombstone() : AjamaGhouliganCard(2,
    CardType.Skill, CardRarity.Rare,
    TargetType.Self)
{
    private const string CalculatedSummon = "CalculatedSummon";
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        ..MakeCalculatedVar(CalculatedSummon, 10, (card, _) => SepulchrePile.PileType.GetPile(card.Owner).Cards.Count)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Retain,
        CardKeyword.Exhaust
    ];

    public override IEnumerable<IHoverTip> MyHoverTips
    {
        get
        {
            string str = StringHelper.Slugify(nameof(StaticHoverTip.SummonDynamic));
            
            LocString title = HoverTipFactory.L10NStatic(str + ".title");
            LocString description = HoverTipFactory.L10NStatic(str + ".description");
            
            title.AddObj("Summon", DynamicVars[CalculatedSummon]);
            description.AddObj("Summon", DynamicVars[CalculatedSummon]);
            
            return
            [
                new HoverTip(title, description)
            ];
        }
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await MyActions.Summon(this, Owner,
            (int) ((CalculatedVar)DynamicVars[CalculatedSummon]).Calculate(null), 
            choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars[CalculatedSummon + "Extra"].UpgradeValueBy(1);
    }
}