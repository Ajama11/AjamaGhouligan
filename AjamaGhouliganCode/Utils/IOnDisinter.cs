using MegaCrit.Sts2.Core.Models;

namespace AjamaGhouligan.AjamaGhouliganCode.Utils;

public interface IOnDisinter
{
    public Task OnDisinter(CardModel card);
}