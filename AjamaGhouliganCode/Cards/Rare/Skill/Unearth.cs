using AjamaGhouligan.AjamaGhouliganCode.CardPiles;
using AjamaGhouligan.AjamaGhouliganCode.Cards;
using AjamaGhouligan.AjamaGhouliganCode.DynamicVars;
using AjamaGhouligan.AjamaGhouliganCode.Powers;
using AjamaGhouligan.AjamaGhouliganCode.Utils;
using BaseLib.Extensions;
using BaseLib.Patches.Content;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace AjamaGhouligan.AjamaGhouliganCode.Cards.Rare.Skill;

public class Unearth() : AjamaGhouliganCard(0,
    CardType.Skill, CardRarity.Rare,
    TargetType.Self)
{
    private const string Threshold = "Threshold";
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new IntVar(Threshold, 5),
        new PowerVar<StrengthPower>(3),
        new BuryVar(2)
    ];

    public override IEnumerable<IHoverTip> MyHoverTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>(),
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust),
        HoverTipFactory.Static(MyEnums.BuryOther)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        
        if (EnoughAttacks)
        {
            await CommonActions.ApplySelf<StrengthPower>(choiceContext, this);
            await CardCmd.Exhaust(choiceContext, this);
        }
        else
        {
            await MyActions.BuryRandomInPile(PileType.Draw, this, filter: c => c.Type == CardType.Attack);
        }
    }

    protected override bool ShouldGlowGoldInternal => EnoughAttacks;

    private bool EnoughAttacks =>
        CustomPiles.GetCustomPile(Owner.PlayerCombatState, SepulchrePile.PileType)!
            .Cards.Count(c => c.Type == CardType.Attack) >= DynamicVars[Threshold].IntValue;
    
    protected override void AddExtraArgsToDescription(LocString description)
    {
        description.Add("EnoughAttacks", !IsInCombat || EnoughAttacks);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Strength.UpgradeValueBy(2);
    }
}