using AjamaGhouligan.AjamaGhouliganCode.DynamicVars;
using AjamaGhouligan.AjamaGhouliganCode.Utils;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Audio.Debug;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;

namespace AjamaGhouligan.AjamaGhouliganCode.Cards.Common.Attack;

public class DoublePoke() : AjamaGhouliganCard(1,
    CardType.Attack, CardRarity.Common,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new OstyDamageVar(3, DamageProps.card),
        new DamageVar(3, DamageProps.card),
        new DisinterVar(2)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        AttackCommand ghouliganFakeCommand = CommonActions.CardAttack(this, play,
            vfx: VfxCmd.bluntPath,
            tmpSfx: TmpSfx.bluntAttack);
        
        AttackCommand ostyFakeCommand = DamageCmd.Attack(DynamicVars.OstyDamage.BaseValue)
            .FromOsty(Owner.Osty!, this, play)
            .WithAttackerAnim("attack_poke", 0.3f)
            .WithHitFx(
                vfx: VfxCmd.bluntPath,
                tmpSfx: TmpSfx.bluntAttack);
        
        AttackContext attackContext =
            await AttackCommand.CreateContextAsync(CombatState!, choiceContext, play);

        if (Owner.IsOstyAlive) await Hook.BeforeAttack(CombatState!, ostyFakeCommand);
        
        await CreatureCmd.TriggerAnim(Owner.Creature,
            ghouliganFakeCommand._attackerAnimName!, ghouliganFakeCommand._attackerAnimDelay);
        NDebugAudioManager.Instance?.Play(ghouliganFakeCommand.TmpHitSfx!);
        VfxCmd.PlayOnCreatureCenter(play.Target!, ghouliganFakeCommand.HitVfx!);
        
        IEnumerable<DamageResult> ghouliganHit =
            await CreatureCmd.Damage(choiceContext, play.Target!,
                DynamicVars.Damage, Owner.Creature, this, play);
        
        IEnumerable<DamageResult>? ostyHit = null;
        if (!Osty.CheckMissingWithAnim(Owner))
        {
            await CreatureCmd.TriggerAnim(Owner.Osty!,
                ostyFakeCommand._attackerAnimName!, ostyFakeCommand._attackerAnimDelay);
            NDebugAudioManager.Instance?.Play(ostyFakeCommand.TmpHitSfx!);
            VfxCmd.PlayOnCreatureCenter(play.Target!, ostyFakeCommand.HitVfx!);
                
            ostyHit =
                await CreatureCmd.Damage(choiceContext, play.Target!,
                    DynamicVars.OstyDamage.BaseValue, DynamicVars.OstyDamage.Props, Owner.Osty, this, play);
        }
        
        attackContext.AddHit(ghouliganHit);
        
        if (ostyHit != null)
        {
            // ReSharper disable PossibleMultipleEnumeration
            attackContext.AddHit(ostyHit);
            ostyFakeCommand._results.Add(ostyHit.ToList());
            // ReSharper restore PossibleMultipleEnumeration
        }
        
        await attackContext.DisposeAsync();

        if (ostyHit != null)
        {
            CombatManager.Instance.History.CreatureAttacked(CombatState!, ostyFakeCommand.Attacker!,
                ostyFakeCommand._results.SelectMany(r => r).ToList());
            
            await Hook.AfterAttack(CombatState!, choiceContext, ostyFakeCommand);
        }
        
        await MyActions.DisinterSelect(choiceContext, this, true);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.OstyDamage.UpgradeValueBy(2);
        DynamicVars.Damage.UpgradeValueBy(2);
    }
}