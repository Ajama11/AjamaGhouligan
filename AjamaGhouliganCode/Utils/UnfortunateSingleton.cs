using AjamaGhouligan.AjamaGhouliganCode.Powers;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace AjamaGhouligan.AjamaGhouliganCode.Utils;

public class UnfortunateSingleton() : CustomSingletonModel(HookType.Combat)
{
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (!cardPlay.Card.Keywords.Contains(MyEnums.Unfortunate)) return;

        await Trigger(cardPlay.Card.CombatState!, choiceContext: choiceContext, cardPlay: cardPlay);
    }

    public static async Task Trigger(ICombatState combatState, int times = 1, PlayerChoiceContext? choiceContext = null, CardPlay? cardPlay = null)
    {
        choiceContext ??= new ThrowingPlayerChoiceContext();
        
        for (int i = 0; i < times; i++)
        {
            foreach (var enemy in combatState.HittableEnemies)
            {
                MisfortunePower? misfortune = enemy.GetPower<MisfortunePower>();
                if (misfortune == null) continue;
                
                VfxCmd.PlayOnCreatureCenter(enemy, VfxCmd.slimeImpactVfxPath);
                
                await CreatureCmd.Damage(choiceContext, enemy,
                    misfortune.Amount, DamageProps.nonCardUnpowered,
                    cardPlay?.Card, cardPlay);

                if (combatState.PlayerCreatures.Any(p => p.HasPower<WildRidePower>()))
                {
                    await PowerCmd.Decrement(misfortune);
                }
            }
        }
    }
}