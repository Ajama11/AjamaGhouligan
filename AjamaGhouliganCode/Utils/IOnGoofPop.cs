using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace AjamaGhouligan.AjamaGhouliganCode.Utils;

public interface IOnGoofPop
{
    public Task OnGoofPop(PlayerChoiceContext choiceContext, Player player);
}