using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace AjamaGhouligan.AjamaGhouliganCode.Utils;

public interface IAfterSepulchreAutoplayOnTurnStart
{
    public Task AfterSepulchreAutoplayOnTurnStart(PlayerChoiceContext choiceContext, Player player);
}