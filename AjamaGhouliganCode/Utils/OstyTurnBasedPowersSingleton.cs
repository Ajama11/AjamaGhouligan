using AjamaGhouligan.AjamaGhouliganCode.Powers;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace AjamaGhouligan.AjamaGhouliganCode.Utils;

public class OstyTurnBasedPowersSingleton() : CustomSingletonModel(HookType.Combat)
{
    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        var participantsList = participants.ToList();
        
        foreach (var participant in participantsList.Where(c => c.IsPlayer && c.Player!.IsOstyAlive))
        {
            var osty = participant.Player!.Osty!;

            var strain = osty.GetPower<StrainPower>();
            if (strain != null)
            {
                await strain.AfterSideTurnEnd(choiceContext, side, [..participantsList, osty]);
            }
        }
    }
}