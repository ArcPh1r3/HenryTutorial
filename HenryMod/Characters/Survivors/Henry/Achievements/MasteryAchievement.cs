using RoR2;
using System;
using UnityEngine;
using HenryMod.Modules.Survivors;

namespace HenryMod.Modules.Achievements
{
    internal class MasteryAchievement : BaseMasteryUnlockable
    {
        public override string AchievementTokenPrefix => HenrySurvivor.HENRY_PREFIX + "MASTERY";
        public override Sprite Sprite => HenrySurvivor.instance.assetBundle.LoadAsset<Sprite>("texMasteryAchievement");
        //the token of your character's unlock achievement if you have one
        public override string PrerequisiteUnlockableIdentifier => HenrySurvivor.HENRY_PREFIX + "UNLOCKABLE_REWARD_ID";

        public override string RequiredCharacterBody => HenrySurvivor.instance.bodyName;
        //difficulty coeff 3 is monsoon. 3.5 is typhoon for grandmastery skins
        public override float RequiredDifficultyCoefficient => 3;
    }
}