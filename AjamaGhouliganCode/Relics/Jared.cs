using AjamaGhouligan.AjamaGhouliganCode.CardPiles;
using AjamaGhouligan.AjamaGhouliganCode.Relics;
using AjamaGhouligan.AjamaGhouliganCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace AjamaGhouligan.AjamaGhouliganCode.Relics;

public class Jared() : AjamaGhouliganRelic, IAfterSepulchreAutoplayOnTurnStart
{
    public override RelicRarity Rarity =>
        RelicRarity.Rare;

    public const int TurnsThreshold = 3;
    public const string TurnsKey = "Turns";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new(TurnsKey, TurnsThreshold)
    ];

    private int _turnsSeen;

    [SavedProperty]
    public int TurnsSeen
    {
        get => _turnsSeen;
        set
        {
            AssertMutable();
            _turnsSeen = value;
            InvokeDisplayAmountChanged();
        }
    }

    private bool _isActivating;

    [SavedProperty]
    public bool IsActivating
    {
        get => _isActivating;
        set
        {
            AssertMutable();
            _isActivating = value;
            InvokeDisplayAmountChanged();
        }
    }

    public override bool ShowCounter => true;

    public override int DisplayAmount => !IsActivating ? TurnsSeen : DynamicVars[TurnsKey].IntValue;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(MyEnums.Haunted),
        HoverTipFactory.FromKeyword(MyEnums.Bury)
    ];

    public async Task DoActivateVisuals()
    {
        IsActivating = true;
        Flash();
        await Cmd.Wait(1);
        IsActivating = false;
    }


    public async Task AfterSepulchreAutoplayOnTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner) return;

        TurnsSeen = (TurnsSeen + 1) % DynamicVars[TurnsKey].IntValue;
        Status = TurnsSeen == DynamicVars[TurnsKey].IntValue - 1 ? RelicStatus.Active : RelicStatus.Normal;
        
        if (TurnsSeen != 0) return;

        _ = TaskHelper.RunSafely(DoActivateVisuals());
        
        CardModel? card = CardFactory.GetDistinctForCombat(Owner, 
            Owner.Character.CardPool
                .GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint)
                .Where(c => c.Type == CardType.Attack), 
            1, Owner.RunState.Rng.CombatCardGeneration)
            .FirstOrDefault();
        
        if (card == null) return;
        
        MyActions.GainsHauntedAndBury(card, false);
        
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(card, SepulchrePile.PileType, Owner));
    }

    public override Task AfterCombatEnd(CombatRoom _)
    {
        Status = RelicStatus.Normal;
        return Task.CompletedTask;
    }
}