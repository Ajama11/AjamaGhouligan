using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace AjamaGhouligan.AjamaGhouliganCode.Extensions;

public static class OstyExtensions
{
    extension(Osty osty)
    {
        /// <summary>
        /// Returns true if Osty is present and alive. Returns false otherwise, and shake's Osty's corpse if he's present but dead.
        /// Only used when Osty isn't attacking. If he's attacking, use !Osty.CheckMissingWithAnim(Owner) instead for the shake animation to always play, to match the base game.
        /// Also, if you're reading this, you're a nerd.
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        public static bool IsReadyToParty(Player owner)
        {
            if (owner.IsOstyMissing)
            {
                NCombatRoom.Instance?._creatureNodes
                    .FirstOrDefault(c => c.Entity.Monster is Osty && c.Entity.PetOwner == owner)?
                    .AnimShake();
            }
            
            return !owner.IsOstyMissing;
        }
    }
}