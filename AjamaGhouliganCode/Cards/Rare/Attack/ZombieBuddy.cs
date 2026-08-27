using AjamaGhouligan.AjamaGhouliganCode.Utils;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Audio.Debug;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace AjamaGhouligan.AjamaGhouliganCode.Cards.Rare.Attack;

public class ZombieBuddy() : AjamaGhouliganCard(1,
    CardType.Attack, CardRarity.Rare,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        ..MakeCalculatedDamage(3, static (card, _) => CombatManager.Instance.History.Entries.OfType<CardGeneratedEntry>().Count(e => e.Creator == card.Owner), 1),
        new CardsVar(1)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.CardAttack(this, play,
                vfx: VfxCmd.bluntPath,
                tmpSfx: TmpSfx.bluntAttack)
            .Execute(choiceContext);
        
        var cards = MyActions.GetRandomCards(this, PileType.Hand, _ => true, DynamicVars.Cards.IntValue);

        foreach (var card in cards)
        {
            await CardCmd.Transform(card, CreateClone());
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.ExtraDamage.UpgradeValueBy(1);
    }
}