using AjamaGhouligan.AjamaGhouliganCode.BundledHoverTips.Core;
using AjamaGhouligan.AjamaGhouliganCode.Cards;
using AjamaGhouligan.AjamaGhouliganCode.Cards.Basic;
using AjamaGhouligan.AjamaGhouliganCode.Cards.Token;
using AjamaGhouligan.AjamaGhouliganCode.DynamicVars;
using AjamaGhouligan.AjamaGhouliganCode.Powers;
using AjamaGhouligan.AjamaGhouliganCode.Utils;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.ValueProps;

namespace AjamaGhouligan.AjamaGhouliganCode.Cards.Uncommon.Power;

public class Jester() : AjamaGhouliganCard(1,
    CardType.Power, CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<JesterPower>(1)
    ];
    
    public override BundledHoverTipManager MyBundles
    {
        get
        {
            CardModel strike = ModelDb.Card<Strike>().ToMutable();
            
            strike.EnergyCost.SetThisCombat(0);
            strike.AddKeyword(MyEnums.Entomb);
            
            return
            [
                BundledHoverTipFactory.FromPower<GoofPower>(),
                BundledHoverTipFactory.FromKeyword(MyEnums.Entomb),
                BundledHoverTipFactory.FromCard<Cavort>(),
                BundledHoverTipFactory.FromCard(strike)
            ];
        }
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "PowerUp", Owner.Character.PowerUpAnimDelay);
        
        await CommonActions.ApplySelf<JesterPower>(choiceContext, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Power<JesterPower>().UpgradeValueBy(1);
    }
}