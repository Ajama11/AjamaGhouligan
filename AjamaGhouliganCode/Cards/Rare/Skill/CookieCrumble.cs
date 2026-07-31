using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace AjamaGhouligan.AjamaGhouliganCode.Cards.Rare.Skill;

public class CookieCrumble() : AjamaGhouliganCard(1,
    CardType.Skill, CardRarity.Rare,
    TargetType.Self)
{
    public override bool CanBeGeneratedInCombat => false;
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new MaxHpVar(3),
        new CardsVar(6),
        ..MakeCalculatedVar("CardsAlreadyPlayed", 0, static (card, _) => CardsPlayedThisTurn(card.Owner))
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        if (!HasBeenPlayed && HaveEnoughCardsBeenPlayedThisTurn)
        {
            await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
            
            await CreatureCmd.GainMaxHp(Owner.Creature, DynamicVars.MaxHp.BaseValue);
            
            if (LocalContext.IsMe(Owner))
                VfxCmd.PlayFullScreenInCombat(VfxCmd.bitePath, Owner.Creature);
        }
    }

    private bool HasBeenPlayed
    {
        get
        {
            return CombatManager.Instance.History.CardPlaysFinished.Any(e => e.CardPlay.Card == this);
        }
    }

    private bool HaveEnoughCardsBeenPlayedThisTurn => CardsPlayedThisTurn(Owner) >= DynamicVars.Cards.IntValue;

    private static int CardsPlayedThisTurn(Player player)
    {
        return CombatManager.Instance.History.CardPlaysFinished.Count(e =>
            e.Actor == player.Creature &&
            e.HappenedThisTurn(player.Creature.CombatState)
        );
    }
    
    protected override bool ShouldGlowGoldInternal => !HasBeenPlayed && HaveEnoughCardsBeenPlayedThisTurn;

    protected override bool ShouldGlowRedInternal => HasBeenPlayed;

    protected override void AddExtraArgsToDescription(LocString description)
    {
        description.Add("FirstPlay", !IsInCombat || !HasBeenPlayed);
        description.Add("EnoughCardsPlayedThisTurn", !IsInCombat || HaveEnoughCardsBeenPlayedThisTurn);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(-1);
    }
}