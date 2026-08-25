using AjamaGhouligan.AjamaGhouliganCode.DynamicVars;
using AjamaGhouligan.AjamaGhouliganCode.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.CommonUi;

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
        ..MyEnums.TreatHovers(true)
    ];

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        if (player != Owner) return;
        
        Flash();
        
        await MyActions.CreateTreats(DynamicVars.Treat.IntValue, Owner, combatState, modifyCardsBeforePreview: list =>
        {
            CardCmd.Upgrade(list, CardPreviewStyle.None);
            return list;
        });
    }
}