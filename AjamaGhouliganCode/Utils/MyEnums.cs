using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace AjamaGhouligan.AjamaGhouliganCode.Utils;

public class MyEnums
{
    [CustomEnum, KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword Haunted;
    
    [CustomEnum, KeywordProperties(AutoKeywordPosition.After)]
    public static CardKeyword Bury;
    
    [CustomEnum, KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword Buried;
}