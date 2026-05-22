using MegaCrit.Sts2.Core.Models;

namespace AjamaGhouligan.AjamaGhouliganCode.Utils;

public interface IOnBury
{
    public Task OnBury(CardModel card);
}