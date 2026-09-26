using AjamaGhouligan.AjamaGhouliganCode.DynamicVars;
using AjamaGhouligan.AjamaGhouliganCode.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace AjamaGhouligan.AjamaGhouliganCode.Relics;

public class Jared() : AjamaGhouliganRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Rare;

    public override bool ShowCounter => true;
    public override int DisplayAmount =>
        IsActivating ? DynamicVars[Turns].IntValue : TurnsSeen;

    public const int TurnsAmount = 2;
    public const string Turns = "Turns";
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new TreatVar(1),
        new IntVar(Turns, TurnsAmount)
    ];

    [SavedProperty]
    public int TurnsSeen
    {
        get;
        set
        {
            AssertMutable();
            field = value;
            InvokeDisplayAmountChanged();
        }
    }
    
    public bool IsActivating
    {
        get;
        set
        {
            AssertMutable();
            field = value;
            InvokeDisplayAmountChanged();
        }
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        ..MyEnums.TreatHovers(true)
    ];

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        if (player != Owner) return;
        
        TurnsSeen = (TurnsSeen + 1) % DynamicVars[Turns].IntValue;
        Status = TurnsSeen == DynamicVars[Turns].IntValue - 1 ?
            RelicStatus.Active :
            RelicStatus.Normal;
        
        if (TurnsSeen != 0) return;
        
        _ = TaskHelper.RunSafely(DoActivateVisuals());
        
        await MyActions.CreateTreats(DynamicVars.Treat.IntValue, Owner, combatState, modifyCardsBeforePreview: list =>
        {
            CardCmd.Upgrade(list, CardPreviewStyle.None);
            return list;
        });
    }

    public override Task AfterCombatEnd(CombatRoom _)
    {
        Status = RelicStatus.Normal;
        return Task.CompletedTask;
    }

    public async Task DoActivateVisuals()
    {
        IsActivating = true;
        Flash();
        await Cmd.Wait(1);
        IsActivating = false;
    }
}