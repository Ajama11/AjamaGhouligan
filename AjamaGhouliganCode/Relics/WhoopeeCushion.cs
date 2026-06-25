using AjamaGhouligan.AjamaGhouliganCode.Cards.Token;
using AjamaGhouligan.AjamaGhouliganCode.DynamicVars;
using AjamaGhouligan.AjamaGhouliganCode.Powers;
using AjamaGhouligan.AjamaGhouliganCode.Utils;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace AjamaGhouligan.AjamaGhouliganCode.Relics;

public class WhoopeeCushion() : AjamaGhouliganRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Rare;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<GoofPower>(1),
        new SurpriseVar(3)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<GoofPower>(),
        HoverTipFactory.FromCard<Surprise>()
    ];

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (!participants.Contains(Owner.Creature)) return;
        
        Flash();
        
        await PowerCmd.Apply<GoofPower>(new ThrowingPlayerChoiceContext(), Owner.Creature,
            DynamicVars.Power<GoofPower>().BaseValue, Owner.Creature, null);

        await MyActions.CreateSurprises(DynamicVars.Surprise().IntValue, Owner, Owner.Creature.CombatState!,
            PileType.Discard, CardPilePosition.Bottom);
    }
}