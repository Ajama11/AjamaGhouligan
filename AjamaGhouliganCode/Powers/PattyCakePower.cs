using AjamaGhouligan.AjamaGhouliganCode.Extensions;
using BaseLib.Cards.Variables;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.ValueProps;

namespace AjamaGhouligan.AjamaGhouliganCode.Powers;

public class PattyCakePower : AjamaGhouliganPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    private const string PassAmount = "PassAmount";
    private const string ShouldPass = "ShouldPass";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DisplayVar<PattyCakePower>(PassAmount, p => (p.Amount - 1).ToString()),
        new BoolVar(ShouldPass, false)
    ];

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        if (Amount > 1)
        {
            ((BoolVar) DynamicVars[ShouldPass]).BoolVal = true;
        }

        return Task.CompletedTask;
    }

    public override Task BeforeAttack(AttackCommand command)
    {
        if (command.Attacker != Owner || !command.DamageProps.IsPoweredAttack())
            return Task.CompletedTask;
        
        Data data = GetInternalData<Data>();
        
        if (data.CommandToModify != null ||
            command.ModelSource != null &&
            command.ModelSource is not CardModel ||
            !command.DamageProps.IsPoweredAttack())
        {
            return Task.CompletedTask;
        }
        
        data.CommandToModify = command;
        data.AmountWhenAttackStarted = Amount;
        
        return Task.CompletedTask;
    }
    
    public override decimal ModifyDamageAdditive(Creature? target, decimal amount,
        ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (Owner != dealer || !props.IsPoweredAttack()) return 0M;
        
        Data data = GetInternalData<Data>();
        
        return data.CommandToModify != null &&
               cardSource != null &&
               cardSource != data.CommandToModify.ModelSource ||
               data.CommandToModify != null &&
               data.CommandToModify.Attacker != dealer
            ? 0M : Amount;
    }

    public override async Task AfterAttack(PlayerChoiceContext choiceContext, AttackCommand command)
    {
        Data data = GetInternalData<Data>();
        
        if (data.CommandToModify != command) return;

        if (Owner.IsPlayer && Osty.IsReadyToParty(Owner.Player!))
        {
            await PowerCmd.Apply<PattyCakePower>(choiceContext,
                Owner.Player!.Osty!, data.AmountWhenAttackStarted - 1,
                Owner, null);
        }
        else if (Owner.PetOwner != null)
        {
            await PowerCmd.Apply<PattyCakePower>(choiceContext,
                Owner.PetOwner.Creature, data.AmountWhenAttackStarted - 1,
                Owner, null);
        }

        await PowerCmd.ModifyAmount(choiceContext, this,
            -data.AmountWhenAttackStarted, null, null);

        data.CommandToModify = null;
    }

    protected override object InitInternalData() => new Data();
    public class Data
    {
        public AttackCommand? CommandToModify;
        public int AmountWhenAttackStarted;
    }
}