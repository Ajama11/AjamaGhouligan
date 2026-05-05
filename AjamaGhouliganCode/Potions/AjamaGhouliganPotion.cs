using BaseLib.Abstracts;
using BaseLib.Utils;
using AjamaGhouligan.AjamaGhouliganCode.Character;

namespace AjamaGhouligan.AjamaGhouliganCode.Potions;

[Pool(typeof(GhouliganPotionPool))]
public abstract class AjamaGhouliganPotion : CustomPotionModel;