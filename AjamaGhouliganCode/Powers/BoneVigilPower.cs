using AjamaGhouligan.AjamaGhouliganCode.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace AjamaGhouligan.AjamaGhouliganCode.Powers;

public class BoneVigilPower : AjamaGhouliganPower, IOnBury
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    public async Task OnBury(CardModel card, CardPlay? play)
    {
        if (card.Owner.Creature != Owner) return;

        await MyActions.OstyHeal(Owner.Player!, Amount);
    }
}