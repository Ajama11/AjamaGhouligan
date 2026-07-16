using AjamaGhouligan.AjamaGhouliganCode.DynamicVars;
using AjamaGhouligan.AjamaGhouliganCode.Utils;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace AjamaGhouligan.AjamaGhouliganCode.Cards.Uncommon.Skill;

public class ThinkFast() : AjamaGhouliganCard(1,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self)
{
    private const string MoreSurprises = "MoreSurprises";
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new SurpriseVar(2),
        new (MoreSurprises, 2),
        new CardsVar(1)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Innate,
        CardKeyword.Exhaust
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await MyActions.CreateSurprises(this, PileType.Draw, CardPilePosition.Top, previewTime: 0.0f);
        
        await MyActions.CreateSurprises(this, PileType.Draw, CardPilePosition.Random, DynamicVars[MoreSurprises].IntValue, previewTime: 0.0f);

        await Cmd.Wait(0.75f);

        await CommonActions.Draw(this, choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars[MoreSurprises].UpgradeValueBy(1);
    }
}