using AjamaGhouligan.AjamaGhouliganCode.CardPiles;
using AjamaGhouligan.AjamaGhouliganCode.Cards;
using AjamaGhouligan.AjamaGhouliganCode.DynamicVars;
using AjamaGhouligan.AjamaGhouliganCode.Powers;
using AjamaGhouligan.AjamaGhouliganCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.ValueProps;

namespace AjamaGhouligan.AjamaGhouliganCode.Cards.Common.Attack;

public class Tamp() : AjamaGhouliganCard(2,
    CardType.Attack, CardRarity.Common,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new OstyDamageVar(12, ValueProp.Move),
        new BuryVar(99),
        new HauntVar(1, true)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Retain,
        MyEnums.Unfortunate
    ];

    public override IEnumerable<IHoverTip> MyHoverTips =>
    IsUpgraded ? [HoverTipFactory.Static(MyEnums.Haunt)] : [];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        
        if (!Osty.CheckMissingWithAnim(Owner))
        {
            await DamageCmd.Attack(DynamicVars.OstyDamage.BaseValue)
                .FromOsty(Owner.Osty!, this)
                .Targeting(play.Target)
                .WithHitFx("vfx/vfx_attack_blunt", tmpSfx: "blunt_attack.mp3")
                .Execute(choiceContext);
        }
        
        await MyActions.BurySpecific(PileType.Hand.GetPile(Owner).Cards.ToList());

        if (IsUpgraded)
        {
            await Cmd.Wait(0.33f);
            
            await MyActions.HauntAndPossiblyBury(this, 
                [SepulchrePile.PileType], 
                false, true, choiceContext, 
                c => !c.Keywords.Contains(MyEnums.Haunted) && !c.Keywords.Contains(CardKeyword.Unplayable));
        }
    }
}