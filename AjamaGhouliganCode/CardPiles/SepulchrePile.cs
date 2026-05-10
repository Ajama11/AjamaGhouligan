using BaseLib.Abstracts;
using BaseLib.Patches.Content;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace AjamaGhouligan.AjamaGhouliganCode.CardPiles;

public class SepulchrePile() : CustomPile(PileType)
{
    [CustomEnum] public static PileType PileType;

    public override bool CardShouldBeVisible(CardModel card) => false;

    public override Vector2 GetTargetPosition(CardModel model, Vector2 size) => NBuryPile.PilePosition + new Vector2(40, 55);
    
    public struct SelectionPrompt
    {
        public static LocString Haunt => new LocString("card_selection", "AJAMAGHOULIGAN-TO_HAUNT");
        public static LocString Bury => new LocString("card_selection", "AJAMAGHOULIGAN-TO_SEPULCHRE");
        public static LocString Disinter => new LocString("card_selection", "AJAMAGHOULIGAN-FROM_SEPULCHRE");
        public static LocString HauntAndBury => new LocString("card_selection", "AJAMAGHOULIGAN-TO_SEPULCHRE_HAUNT");
    }
}