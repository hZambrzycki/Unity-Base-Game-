using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Abilities;
using UnityEngine;

namespace RPG.Abilities.Targeting 
{
    [CreateAssetMenu(fileName = "Demo Targeting", menuName = "Abilities/Targeting", order = 0)]
    public class DemoTargeting : TargetingStrategy
    {
        public override void StartTargeting(AbilityData data, Action finished)
        {
            finished();
        }
    }
}