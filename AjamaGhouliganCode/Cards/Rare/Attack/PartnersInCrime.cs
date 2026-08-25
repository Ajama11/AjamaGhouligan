using AjamaGhouligan.AjamaGhouliganCode.Cards;
using AjamaGhouligan.AjamaGhouliganCode.DynamicVars;
using AjamaGhouligan.AjamaGhouliganCode.Powers;
using AjamaGhouligan.AjamaGhouliganCode.Utils;
using BaseLib.Extensions;
using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Audio.Debug;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Nodes.Vfx.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace AjamaGhouligan.AjamaGhouliganCode.Cards.Rare.Attack;

public class PartnersInCrime() : AjamaGhouliganCard(1,
    CardType.Attack, CardRarity.Rare,
    TargetType.AnyEnemy)
{
    private const string Threshold = "Threshold";
    private const string HitCount = "HitCount";
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(2, DamageProps.card),
        new OstyDamageVar(2, DamageProps.card),
        new IntVar(Threshold, 10),
        ..MakeCalculatedVar(HitCount, 2, (card, _) =>
            Math.Floor(card.Owner.Osty?.MaxHp / card.DynamicVars[Threshold].BaseValue ?? 0))
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        AttackCommand ghouliganFakeCommand = CommonActions.CardAttack(this, play,
            tmpSfx: TmpSfx.bluntAttack)
            .WithHitVfxNode(t => NStabVfx.Create(t, true, VfxColor.Green)!);
        
        AttackCommand ostyFakeCommand = DamageCmd.Attack(1)
            .FromOsty(Owner.Osty!, this, play)
            .WithHitFx(
                vfx: VfxCmd.thrashPath, 
                tmpSfx: TmpSfx.heavyAttack);

        if (Owner.IsOstyAlive) await Hook.BeforeAttack(CombatState!, ostyFakeCommand);

        await using AttackContext attackContext =
            await AttackCommand.CreateContextAsync(CombatState!, choiceContext, play);

        bool wasThereAtLeastOneOstyHit = false;

        for (int i = 0; i < ((CalculatedVar) DynamicVars[HitCount]).Calculate(null); i++)
        {
            await CreatureCmd.TriggerAnim(Owner.Creature,
                ghouliganFakeCommand._attackerAnimName!, 0);
            NDebugAudioManager.Instance?.Play(ghouliganFakeCommand.TmpHitSfx!);
            play.Target!.GetVfxContainer()?.AddChildSafely(ghouliganFakeCommand._customHitVfxNodes[0](play.Target!));
            
            IEnumerable<DamageResult> ghouliganHit =
                await CreatureCmd.Damage(choiceContext, play.Target!,
                    DynamicVars.Damage, Owner.Creature, this, play);

            IEnumerable<DamageResult>? ostyHit = null;
            if (!Osty.CheckMissingWithAnim(Owner))
            {
                await CreatureCmd.TriggerAnim(Owner.Osty!,
                    ostyFakeCommand._attackerAnimName!, 0);
                NDebugAudioManager.Instance?.Play(ostyFakeCommand.TmpHitSfx!);
                VfxCmd.PlayOnCreatureCenter(play.Target!, ostyFakeCommand.HitVfx!);
                
                 ostyHit =
                    await CreatureCmd.Damage(choiceContext, play.Target!,
                        DynamicVars.Damage, Owner.Osty, this, play);
            }
            
            attackContext.AddHit(ghouliganHit);
            
            if (ostyHit != null)
            {
                // ReSharper disable PossibleMultipleEnumeration
                attackContext.AddHit(ostyHit);
                ostyFakeCommand._results.Add(ostyHit.ToList());
                // ReSharper restore PossibleMultipleEnumeration

                wasThereAtLeastOneOstyHit = true;
            }
        }
        
        if (wasThereAtLeastOneOstyHit)
        {
            CombatManager.Instance.History.CreatureAttacked(CombatState!, ostyFakeCommand.Attacker!,
                ostyFakeCommand._results.SelectMany(r => r).ToList());
            
            await Hook.AfterAttack(CombatState!, choiceContext, ostyFakeCommand);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2);
        DynamicVars.OstyDamage.UpgradeValueBy(2);
    }
}