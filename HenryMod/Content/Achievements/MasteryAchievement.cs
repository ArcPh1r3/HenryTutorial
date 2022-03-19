using RoR2;
using System;
using UnityEngine;

namespace HenryMod.Modules.Achievements
{
    internal class MasteryAchievement : BaseMasteryUnlockable
    {
        public override string AchievementTokenPrefix => HenryPlugin.DEVELOPER_PREFIX + "_HENRY_BODY_MASTERY";
        public override string AchievementSpriteName => "texMasteryAchievement";
        public override string PrerequisiteUnlockableIdentifier => HenryPlugin.DEVELOPER_PREFIX + "_HENRY_BODY_UNLOCKABLE_REWARD_ID";

        public override string RequiredCharacterBody => "HenryBody";
        public override float RequiredDifficultyCoefficient => 3;
    }
}