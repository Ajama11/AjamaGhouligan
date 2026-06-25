using AjamaGhouligan.AjamaGhouliganCode.DynamicVars;
using AjamaGhouligan.AjamaGhouliganCode.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace AjamaGhouligan.AjamaGhouliganCode.Relics;

public class Jared() : AjamaGhouliganRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Rare;
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new TreatVar(1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        ..MyEnums.TreatHovers()
    ];

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (!participants.Contains(Owner.Creature)) return;
        
        Flash();
        
        await MyActions.CreateTreats(1, Owner, combatState);
    }
}