using BaseLib.Abstracts;
using BaseLib.Patches.Content;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace AjamaGhouligan.AjamaGhouliganCode.CardPiles;

public class SepulchrePile() : CustomPile(PileType)
{
    [CustomEnum] public static PileType PileType;

    public override bool CardShouldBeVisible(CardModel card) => false;

    public override Vector2 GetTargetPosition(CardModel model, Vector2 size)
    {
        // ReSharper disable once InvertIf
        if (NCombatRoom.Instance != null)
        {
            var container = NCombatRoom.Instance.Ui.GetNode<NCombatPilesContainer>("%CombatPileContainer");
            NBuryPile? pile = container.GetNodeOrNull<NBuryPile>("_BuryPile");

            if (pile != null) return NBuryPile.PilePosition + pile.Size * 0.5f;
        }
        
        return NBuryPile.PilePosition + new Vector2(40, 55);
    }
}