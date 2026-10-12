using AjamaGhouligan.AjamaGhouliganCode.Cards.Rare.Attack;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Random;

namespace AjamaGhouligan.AjamaGhouliganCode.Patches;

[HarmonyPatch]
public static class ZombieBuddyAfterTransformedPatch
{
    [HarmonyPatch(typeof(CardCmd), nameof(CardCmd.Transform),
        typeof(IEnumerable<CardTransformation>),
        typeof(Rng),
        typeof(CardPreviewStyle))]
    [HarmonyPrefix]
    private static void TransformPrefix(ref IEnumerable<CardTransformation> transformations, out List<CardTransformation> __state)
    {
        __state = transformations.ToList();
    }
    
    [HarmonyPatch(typeof(CardCmd), nameof(CardCmd.Transform),
        typeof(IEnumerable<CardTransformation>),
        typeof(Rng),
        typeof(CardPreviewStyle))]
    [HarmonyPostfix]
    public static void TransformPostfix(ref Task<IEnumerable<CardPileAddResult>> __result, List<CardTransformation> __state)
    {
        __result = UpdateZombieBuddies(__result, __state);
    }

    private static async Task<IEnumerable<CardPileAddResult>> UpdateZombieBuddies(Task<IEnumerable<CardPileAddResult>> originalTask, List<CardTransformation> transformations)
    {
        var originalResults = await originalTask;

        foreach (var transformation in transformations)
        {
            CardModel original = transformation.Original;
            CardModel? replacement = transformation.Replacement;

            if (!transformation.IsInCombat ||
                replacement?.Owner.PlayerCombatState == null)
            {
                return originalResults;
            }

            foreach (var zombie in replacement.Owner.PlayerCombatState.AllCards.OfType<ZombieBuddy>())
            {
                if (zombie.Card == original)
                {
                    zombie.SetCardToTransform();
                    // MainFile.Logger.Warn($"A Zombie's target {original.Title} got transformed, so it is now targeting {zombie.Card?.Title}");
                }
            }
        }

        return originalResults;
    }
}