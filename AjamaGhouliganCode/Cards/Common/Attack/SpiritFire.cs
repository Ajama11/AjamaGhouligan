using AjamaGhouligan.AjamaGhouliganCode.BundledHoverTips.Core;
using AjamaGhouligan.AjamaGhouliganCode.Cards;
using AjamaGhouligan.AjamaGhouliganCode.DynamicVars;
using AjamaGhouligan.AjamaGhouliganCode.Powers;
using AjamaGhouligan.AjamaGhouliganCode.Utils;
using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;

namespace AjamaGhouligan.AjamaGhouliganCode.Cards.Common.Attack;

public class SpiritFire() : AjamaGhouliganCard(2,
    CardType.Attack, CardRarity.Common,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(4, DamageProps.card),
        new RepeatVar(5),
        new CardsVar(2)
    ];
    
    public override BundledHoverTipManager MyBundles
    {
        get
        {
            CardModel burn = ModelDb.Card<Burn>().ToMutable();
            
            burn.AddKeyword(CardKeyword.Ethereal);
            
            return
            [
                BundledHoverTipFactory.FromCard(burn),
            ];
        }
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.CardAttack(this, play,
                hitCount: DynamicVars.Repeat.IntValue)
            .WithHitVfxNode((Func<Creature, Node2D>) (t => NFireBurstVfx.Create(
                NCombatRoom.Instance!.GetCreatureNode(t)!.GetBottomOfHitbox(), 
                0.75f, new Color(0, 0.75f, 0))!))
            .Execute(choiceContext);

        await MyActions.CreateCards(ModelDb.Card<Burn>(), DynamicVars.Cards.IntValue, this, modifyCardsBeforePreview:
            list =>
            {
                foreach (var burn in list)
                {
                    burn.AddKeyword(CardKeyword.Ethereal);
                }
                return list;
            });
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Repeat.UpgradeValueBy(1);
    }
}