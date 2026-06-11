using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prefabs.Reefscape.Robots.Mods.StemColiseumMod._172
{
    [CreateAssetMenu(fileName = "Setpoint", menuName = "Robot/Northern Force Setpoint", order = 0)]
    public class NorthernForceSetpoint : ScriptableObject
    {
        [Tooltip("Inches")] public float elevatorHeight;
        [Tooltip("Degrees")] public float algaeArmAngle;
    }
}
