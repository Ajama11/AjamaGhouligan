using AjamaGhouligan.AjamaGhouliganCode.DynamicVars;
using AjamaGhouligan.AjamaGhouliganCode.Powers;
using AjamaGhouligan.AjamaGhouliganCode.Relics;
using AjamaGhouligan.AjamaGhouliganCode.Utils;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace AjamaGhouligan.AjamaGhouliganCode.Relics;

public class WhoopeeCushion() : AjamaGhouliganRelic, IAfterSepulchreAutoplayOnTurnStart
{
    public override RelicRarity Rarity =>
        RelicRarity.Rare;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<GoofPower>(1),
        new HauntVar(2),
        new BuryVar(2)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<GoofPower>(),
        HoverTipFactory.Static(MyEnums.Haunt),
        HoverTipFactory.FromKeyword(MyEnums.Haunted),
        HoverTipFactory.Static(MyEnums.BuryOther)
    ];

    public async Task AfterSepulchreAutoplayOnTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner) return;
        
        Flash();
        
        await PowerCmd.Apply<GoofPower>(new ThrowingPlayerChoiceContext(), Owner.Creature,
            DynamicVars.Power<GoofPower>().BaseValue, Owner.Creature, null);

        List<CardModel> cards = MyActions.GetRandomCards(Owner, PileType.Draw, _ => true, DynamicVars.Haunt().IntValue);

        await MyActions.HauntAndBurySpecific(cards);
    }
}