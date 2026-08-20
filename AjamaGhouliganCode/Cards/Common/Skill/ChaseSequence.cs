using AjamaGhouligan.AjamaGhouliganCode.Cards;
using AjamaGhouligan.AjamaGhouliganCode.DynamicVars;
using AjamaGhouligan.AjamaGhouliganCode.Powers;
using AjamaGhouligan.AjamaGhouliganCode.Utils;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace AjamaGhouligan.AjamaGhouliganCode.Cards.Common.Skill;

public class ChaseSequence() : AjamaGhouliganCard(1,
    CardType.Skill, CardRarity.Common,
    TargetType.Self)
{
    private const string BlockNextTurn = "BlockNextTurn";
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(6, BlockProps.card),
        new BlockVar(BlockNextTurn, 6, BlockProps.card),
        new CardsVar(1)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [

    ];

    public override HashSet<CardTag> MyCanonicalTags =>
    [

    ];

    public override IEnumerable<IHoverTip> MyHoverTips =>
    [

    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.CardBlock(this, play);

        BlockVar blockVar = (BlockVar) DynamicVars[BlockNextTurn];
        
        decimal nextTurnAmount = Hook.ModifyBlock(CombatState!, Owner.Creature,
            blockVar.BaseValue, blockVar.Props, 
            this, play, out _);

        await CommonActions.ApplySelf<BlockNextTurnPower>(choiceContext, this, nextTurnAmount);

        await MyActions.CreateCards(ModelDb.Card<Dazed>(),
            DynamicVars.Cards.IntValue, this,
            PileType.Draw, CardPilePosition.Top);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2);
        DynamicVars[BlockNextTurn].UpgradeValueBy(2);
    }
}