using AjamaGhouligan.AjamaGhouliganCode.Cards;
using AjamaGhouligan.AjamaGhouliganCode.DynamicVars;
using AjamaGhouligan.AjamaGhouliganCode.Powers;
using AjamaGhouligan.AjamaGhouliganCode.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.ValueProps;

namespace AjamaGhouligan.AjamaGhouliganCode.Cards.Common.Attack;

public class NeckSnap() : AjamaGhouliganCard(1,
    CardType.Attack, CardRarity.Common,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new OstyDamageVar(6, ValueProp.Move)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        MyEnums.Unfortunate
    ];

    public override HashSet<CardTag> MyCanonicalTags =>
    [

    ];

    public override IEnumerable<IHoverTip> MyHoverTips =>
    [
        HoverTipFactory.FromKeyword(CardKeyword.Retain)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);

        if (Osty.CheckMissingWithAnim(Owner)) return;

        await DamageCmd.Attack(DynamicVars.OstyDamage.BaseValue)
            .FromOsty(Owner.Osty!, this)
            .Targeting(play.Target)
            .WithAttackerAnim("attack_poke", 0.3f)
            .WithHitFx("vfx/vfx_attack_blunt", tmpSfx: "blunt_attack.mp3")
            .Execute(choiceContext);

        CardSelectorPrefs prefs = new CardSelectorPrefs(SelectionScreenPrompt, 1);

        CardModel? card = (await CardSelectCmd.FromHand(choiceContext, Owner, prefs, 
                c => !c.Keywords.Contains(CardKeyword.Retain),
                this))
            .FirstOrDefault();
        
        if (card != null) CardCmd.ApplyKeyword(card, CardKeyword.Retain);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}