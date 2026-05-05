using BaseLib.Abstracts;
using AjamaGhouligan.AjamaGhouliganCode.Extensions;
using Godot;

namespace AjamaGhouligan.AjamaGhouliganCode.Character;

public class GhouliganPotionPool : CustomPotionPoolModel
{
    public override Color LabOutlineColor => Ghouligan.Color;


    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}