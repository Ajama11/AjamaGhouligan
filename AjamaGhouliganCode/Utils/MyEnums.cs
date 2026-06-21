using AjamaGhouligan.AjamaGhouliganCode.Cards.Token.Treats;
using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace AjamaGhouligan.AjamaGhouliganCode.Utils;

public class MyEnums
{
    [CustomEnum, KeywordProperties(AutoKeywordPosition.None)]
    public static CardKeyword Haunted;
    
    [CustomEnum, KeywordProperties(AutoKeywordPosition.After)]
    public static CardKeyword Bury;
    
    [CustomEnum, KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword Unfortunate;

    [CustomEnum]
    public static StaticHoverTip Haunt;
    
    [CustomEnum]
    public static StaticHoverTip BuryOther;
    
    [CustomEnum]
    public static StaticHoverTip Treats;

    [CustomEnum]
    public static CardTag Treat;

    public enum RandomBuryTargeting
    {
        All,
        NotHaunted,
        PrioritizeHaunted,
        OnlyHaunted
    }
    
    public static IEnumerable<IHoverTip> TreatHovers()
    {
        return
        [
            new CycleHoverTip(MyActions.CanonicalTreats),
            HoverTipFactory.Static(Treats)
        ];
    }
}