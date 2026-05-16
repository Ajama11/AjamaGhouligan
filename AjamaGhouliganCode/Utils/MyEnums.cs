using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;

namespace AjamaGhouligan.AjamaGhouliganCode.Utils;

public class MyEnums
{
    [CustomEnum, KeywordProperties(AutoKeywordPosition.None)]
    public static CardKeyword Haunted;
    
    [CustomEnum, KeywordProperties(AutoKeywordPosition.After)]
    public static CardKeyword Bury;
    
    [CustomEnum, KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword Unfortunate;
    
    [CustomEnum, KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword Buried;

    [CustomEnum]
    public static StaticHoverTip Haunt;
}