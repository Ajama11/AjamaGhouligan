using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace AjamaGhouligan.AjamaGhouliganCode.Utils;

public interface IOnBury
{
    public Task OnBury(CardModel card, CardPlay? play = null);
}