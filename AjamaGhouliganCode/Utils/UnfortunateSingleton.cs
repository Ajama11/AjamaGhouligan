using AjamaGhouligan.AjamaGhouliganCode.Powers;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace AjamaGhouligan.AjamaGhouliganCode.Utils;

public class UnfortunateSingleton() : CustomSingletonModel(true, false)
{
    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer,
        CardModel? cardSource)
    {
        if (cardSource == null) return 0; // or not Necro Mockery
        if (target == null) return 0;
        if (!cardSource.Keywords.Contains(MyEnums.Unfortunate)) return 0;

        return target.GetPowerAmount<MisfortunePower>();
    }
}