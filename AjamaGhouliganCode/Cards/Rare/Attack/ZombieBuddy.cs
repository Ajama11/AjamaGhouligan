using AjamaGhouligan.AjamaGhouliganCode.BundledHoverTips.Core;
using AjamaGhouligan.AjamaGhouliganCode.Utils;
using BaseLib.Cards.Variables;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Audio.Debug;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace AjamaGhouligan.AjamaGhouliganCode.Cards.Rare.Attack;

public class ZombieBuddy() : AjamaGhouliganCard(0,
    CardType.Attack, CardRarity.Rare,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DisplayVar<ZombieBuddy>("FormattedCardName", z => z.FormattedCardName),
        ..MakeCalculatedDamage(6, 
            static (card, _) =>
                card.Owner.PlayerCombatState?.AllCards.Count(
                    c => c is ZombieBuddy && c != card && !c.IsDupe) ?? 0,
            5)
    ];
    
    public string FormattedCardName = LocString.GetIfExists("cards", "AJAMAGHOULIGAN-ZOMBIE_BUDDY.nothing")?.GetRawText() ?? "nothing";
    
    public CardModel? Card
    {
        get;
        set
        {
            field = value;
            FormattedCardName = field != null ?
                $"[gold]{field.Title}[/gold]" :
                LocString.GetIfExists("cards", "AJAMAGHOULIGAN-ZOMBIE_BUDDY.nothing")?.GetRawText() ?? "nothing";
        }
    }
    
    public override BundledHoverTipManager MyBundles =>
        Card != null && IsOutsideDrawPile ?
            [BundledHoverTipFactory.FromCard(Card)] :
            [];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.CardAttack(this, play,
                vfx: VfxCmd.bloodyImpactPath,
                tmpSfx: TmpSfx.bluntAttack)
            .Execute(choiceContext);

        if (Card?.Pile != null)
            await CardCmd.Transform(Card, CreateClone());
    }

    public override Task AfterCardEnteredCombat(CardModel card)
    {
        if (card == this && !card.IsDupe) SetCardToTransform();
        return Task.CompletedTask;
    }

    public override Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? clonedBy)
    {
        if (Card == null && card.Pile?.Type == PileType.Draw)
        {
            SetCardToTransform();
            return Task.CompletedTask;
        }
        
        if (card == Card && oldPileType == PileType.Draw) SetCardToTransform();
        
        return Task.CompletedTask;
    }

    public void SetCardToTransform()
    {
        Card = Owner.RunState.Rng.CombatCardSelection
            .NextItem(Owner.PlayerCombatState!.DrawPile.Cards
                .Where(c => c != this));
    }
    
    private bool IsOutsideDrawPile => Pile is { Type: not PileType.Draw };
    
    protected override void AddExtraArgsToDescription(LocString description)
    {
        description.Add("IsOutsideDrawPile", IsOutsideDrawPile);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.ExtraDamage.UpgradeValueBy(3);
    }
}