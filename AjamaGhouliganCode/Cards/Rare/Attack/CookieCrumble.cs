using AjamaGhouligan.AjamaGhouliganCode.Cards;
using AjamaGhouligan.AjamaGhouliganCode.DynamicVars;
using AjamaGhouligan.AjamaGhouliganCode.Powers;
using AjamaGhouligan.AjamaGhouliganCode.Utils;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.ValueProps;

namespace AjamaGhouligan.AjamaGhouliganCode.Cards.Rare.Attack;

public class CookieCrumble() : AjamaGhouliganCard(1,
    CardType.Attack, CardRarity.Rare,
    TargetType.AnyEnemy)
{
    public override bool CanBeGeneratedInCombat => false;
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(12, ValueProp.Move),
        new PowerVar<TastySoulPower>(3)
    ];
    
    public override IEnumerable<IHoverTip> MyHoverTips =>
    [
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust),
        HoverTipFactory.FromPower<TastySoulPower>()
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        if (!HasBeenPlayedThisTurn && 
            play.Target!.Powers.All(p => p.ShouldOwnerDeathTriggerFatal()))
        {
            await CommonActions.Apply<TastySoulPower>(choiceContext, this, play);
        }
        
        await CommonActions.CardAttack(this, play,
                1,
                "vfx/vfx_bite",
                null,
                "blunt_attack.mp3")
            .Execute(choiceContext);
    }
    
    public bool HasBeenPlayedThisTurn
    {
        get
        {
            return CombatManager.Instance.History.CardPlaysFinished.Any(e => e.CardPlay.Card == this && e.HappenedThisTurn(CombatState));
        }
    }
    
    protected override bool ShouldGlowGoldInternal => !HasBeenPlayedThisTurn;

    protected override void AddExtraArgsToDescription(LocString description)
    {
        description.Add("AlreadyPlayed", HasBeenPlayedThisTurn);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Power<TastySoulPower>().UpgradeValueBy(1);
    }
}