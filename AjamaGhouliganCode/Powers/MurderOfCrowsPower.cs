using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.ValueProps;

namespace AjamaGhouligan.AjamaGhouliganCode.Powers;

public class MurderOfCrowsPower : AjamaGhouliganPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerInstanceType InstanceType => PowerInstanceType.InstancedPerApplier;

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (Applier?.Player == null) return Task.CompletedTask;
        if (cardPlay.Card.Owner != Applier.Player) return Task.CompletedTask;
        
        GetInternalData<Data>().AmountsForPlayedCards.Add(cardPlay.Card, Amount);

        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (!GetInternalData<Data>().AmountsForPlayedCards.Remove(cardPlay.Card, out var amount)) return;
        
        Flash();
        
        var room = NCombatRoom.Instance;
        var creatureNode = room?.GetCreatureNode(Owner);

        if (creatureNode != null)
        {
            Vector2 startPosition = creatureNode.VfxSpawnPosition +
                                    new Vector2(Rng.Chaotic.NextFloat(-50f, 50f), -50);
            Vector2 endPosition = creatureNode.VfxSpawnPosition;
        
            NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NShivThrowVfx.Create(startPosition, endPosition, new Color("513666")));
        }
        
        await CreatureCmd.Damage(choiceContext, Owner, amount, DamageProps.nonCardHpLoss, null, null);
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner)) return;

        await PowerCmd.Remove(this);
    }

    protected override object InitInternalData() => new Data();
    public class Data
    {
        public readonly Dictionary<CardModel, int> AmountsForPlayedCards = [];
    }
}