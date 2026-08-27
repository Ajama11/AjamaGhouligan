using AjamaGhouligan.AjamaGhouliganCode.Cards;
using AjamaGhouligan.AjamaGhouliganCode.Powers;
using AjamaGhouligan.AjamaGhouliganCode.Utils;
using HarmonyLib;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace AjamaGhouligan.AjamaGhouliganCode.Patches;

[HarmonyPatch(typeof(CardModel), nameof(CardModel.HoverTips), MethodType.Getter)]
public static class OffclassHoverTipsPatch
{
    [HarmonyPostfix]
    public static IEnumerable<IHoverTip> Postfix(IEnumerable<IHoverTip> __result, CardModel __instance)
    {
        if (__instance is AjamaGhouliganCard) return __result;

        List<IHoverTip> resultToReturn = [..__result];

        if (__instance.Keywords.Contains(MyEnums.Unfortunate) && !resultToReturn.Contains(HoverTipFactory.FromPower<MisfortunePower>()))
        {
            resultToReturn.Insert(
                resultToReturn.IndexOf(HoverTipFactory.FromKeyword(MyEnums.Unfortunate)) + 1, 
                HoverTipFactory.FromPower<MisfortunePower>());
        }

        return resultToReturn;
    }
}