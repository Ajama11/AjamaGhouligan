using AjamaGhouligan.AjamaGhouliganCode.Utils;
using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace AjamaGhouligan.AjamaGhouliganCode.Powers;

public class BlackCatPower : AjamaGhouliganPower, IHasSecondAmount
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    private const string SelfDoomAmount = "SelfDoomAmount";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new IntVar(SelfDoomAmount, 0)
    ];

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature != Owner) return;
        
        Flash();

        await PowerCmd.Apply<DoomPower>(choiceContext,
            Owner, DynamicVars[SelfDoomAmount].BaseValue,
            Owner, null);
    }

    public override decimal ModifyMaxEnergy(Player player, decimal amount)
    {
        return player != Owner.Player ? amount : amount + Amount;
    }

    public void IncreaseSelfDoom(int amount)
    {
        AssertMutable();
        DynamicVars[SelfDoomAmount].BaseValue += amount;
        this.InvokeSecondAmountChanged();
    }

    public string GetSecondAmount()
    {
        return DynamicVars[SelfDoomAmount].IntValue.ToString();
    }
}