using AjamaGhouligan.AjamaGhouliganCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace AjamaGhouligan.AjamaGhouliganCode.Powers;

public class GoofPower : AjamaGhouliganPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    private const decimal Threshold = 10;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar(3),
        new CardsVar(2)
    ];

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier,
        CardModel? cardSource)
    {
        if (power != this) return;
        if (power.Amount < Threshold) return;
        if (Owner.Player is null) return;

        Flash();

        switch (Config.PlayGoofAudio)
        {
            case Config.GoofAudio.Everyone:
                MySounds.GoofPop.Play();
                break;
            case Config.GoofAudio.Self:
                if (LocalContext.IsMe(Owner)) MySounds.GoofPop.Play();
                break;
            case Config.GoofAudio.Never:
            default:
                break;
        }
        
        VfxCmd.PlayFullScreenInCombat("vfx/vfx_dramatic_entrance_fullscreen", Owner);
        
        await PowerCmd.ModifyAmount(new ThrowingPlayerChoiceContext(), this, -Threshold, Owner.Player.Creature, null);

        await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner.Player);

        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, Owner.Player);
        
        foreach (var model in Owner.Player.Creature.CombatState!.IterateHookListeners())
        {
            if (model is not IOnGoofPop goofPopModel) continue;
            await goofPopModel.OnGoofPop(choiceContext, Owner.Player);
            model.InvokeExecutionFinished();
        }
    }
}