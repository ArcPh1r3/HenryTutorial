using RoR2;
using System;
using UnityEngine;
using HenryMod.Survivors.Henry;

namespace HenryMod.Modules.Achievements
{
    [RegisterAchievement(HenrySurvivor.HENRY_PREFIX + "masteryAchievement", HenrySurvivor.HENRY_PREFIX + "masteryUnlockable", null, null)]
    public class HenryMasteryAchievement : BaseMasteryAchievement
    {
        public override string RequiredCharacterBody => HenrySurvivor.instance.bodyName;

        //difficulty coeff 3 is monsoon. 3.5 is typhoon for grandmastery skins
        public override float RequiredDifficultyCoefficient => 3;
    }
}