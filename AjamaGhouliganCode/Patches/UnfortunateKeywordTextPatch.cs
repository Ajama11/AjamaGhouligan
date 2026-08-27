using AjamaGhouligan.AjamaGhouliganCode.Utils;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace AjamaGhouligan.AjamaGhouliganCode.Patches;

[HarmonyPatch(typeof(CardKeywordOrder), MethodType.StaticConstructor)]
public static class UnfortunateKeywordTextPatch
{
    [HarmonyPostfix]
    public static void Postfix(ref CardKeyword[] ___afterDescription)
    {
        ___afterDescription = [MyEnums.Unfortunate, ..___afterDescription];
    }
}