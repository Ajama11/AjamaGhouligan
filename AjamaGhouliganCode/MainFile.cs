using System.Reflection;
using AjamaGhouligan.AjamaGhouliganCode.CardPiles;
using AjamaGhouligan.AjamaGhouliganCode.Cards;
using AjamaGhouligan.AjamaGhouliganCode.Utils;
using BaseLib.Patches.Localization;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;

namespace AjamaGhouligan.AjamaGhouliganCode;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    public const string ModId = "AjamaGhouligan"; //Used for resource filepath
    public const string ResPath = $"res://{ModId}";

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } =
        new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static void Initialize()
    {
        Harmony harmony = new(ModId);

        harmony.PatchAll();
        
        var assembly = Assembly.GetExecutingAssembly();
        Godot.Bridge.ScriptManagerBridge.LookupScriptsInAssembly(assembly);
        
        DescriptionOverrides.CustomizeDescriptionPost += (CardModel card, Creature? _, ref string description) =>
        {
            if (card is not AjamaGhouliganCard) { return; }
            
            if (LocString.GetIfExists("card_keywords", "AJAMAGHOULIGAN-HAUNTED.title_fancy") == null) { return; }

            if (card.Keywords.Contains(MyEnums.Haunted))
            {
                if (card.Pile != null && card.Pile.Type == SepulchrePile.PileType)
                {
                    description = LocString.GetIfExists("card_keywords", "AJAMAGHOULIGAN-HAUNTED.title_fancy_buried")?.GetFormattedText() + "\n" + description;
                }
                else
                {
                    description = LocString.GetIfExists("card_keywords", "AJAMAGHOULIGAN-HAUNTED.title_fancy")?.GetFormattedText() + "\n" + description;
                }
            }
        };
    }
}