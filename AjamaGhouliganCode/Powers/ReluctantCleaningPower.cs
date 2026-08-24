using AjamaGhouligan.AjamaGhouliganCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace AjamaGhouligan.AjamaGhouliganCode.Powers;

public class ReluctantCleaningPower : AjamaGhouliganPower, IOnBury
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal)
    {
        await DoTheThing(choiceContext, card);
    }

    public async Task OnBury(CardModel card, CardPlay? play = null)
    {
        await DoTheThing(new ThrowingPlayerChoiceContext(), card);
    }

    private async Task DoTheThing(PlayerChoiceContext choiceContext, CardModel card)
    {
        if (card.Owner.Creature != Owner) return;
        if (card.Type != CardType.Status) return;

        await CreatureCmd.Damage(choiceContext,
            CombatState.HittableEnemies,
            Amount, DamageProps.nonCardUnpowered,
            Owner);
    }
}