using MegaCrit.Sts2.Core.Localization;

namespace AjamaGhouligan.AjamaGhouliganCode.Utils;

public class MySelectionPrompts
{
    public static LocString Haunt => new("card_selection", "AJAMAGHOULIGAN-TO_HAUNT");
    public static LocString Bury => new ("card_selection", "AJAMAGHOULIGAN-TO_SEPULCHRE");
    public static LocString BuryUpTo => new ("card_selection", "AJAMAGHOULIGAN-TO_SEPULCHRE_UP_TO");
    public static LocString Disinter => new("card_selection", "AJAMAGHOULIGAN-FROM_SEPULCHRE");
    public static LocString HauntAndBury => new("card_selection", "AJAMAGHOULIGAN-TO_SEPULCHRE_HAUNT");
    public static LocString GainsHauntedAndBury => new("card_selection", "AJAMAGHOULIGAN-GAINS_HAUNTED_AND_BURY");
}