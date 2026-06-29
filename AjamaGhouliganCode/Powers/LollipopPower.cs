using AjamaGhouligan.AjamaGhouliganCode.Cards.Token.Treats;
using AjamaGhouligan.AjamaGhouliganCode.Extensions;
using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models.Powers;

namespace AjamaGhouligan.AjamaGhouliganCode.Powers;

public class LollipopPower : CustomTemporaryPowerModelWrapper<Lollipop, StrengthPower>
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override bool InvertInternalPowerAmount => true;

    public override LocString Title => new ("powers", "AJAMAGHOULIGAN-LOLLIPOP_POWER.title");

    public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
    public override string CustomBigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();
}