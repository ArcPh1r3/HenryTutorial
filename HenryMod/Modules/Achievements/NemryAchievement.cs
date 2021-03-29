using RoR2;
using System;
using HenryMod.Modules.Components;
using UnityEngine;

namespace HenryMod.Modules.Achievements
{
    internal class NemryAchievement : ModdedUnlockable
    {
        public override string AchievementIdentifier { get; } = HenryPlugin.developerPrefix + "_NEMRY_BODY_UNLOCKABLE_ACHIEVEMENT_ID";
        public override string UnlockableIdentifier { get; } = HenryPlugin.developerPrefix + "_NEMRY_BODY_UNLOCKABLE_REWARD_ID";
        public override string AchievementNameToken { get; } = HenryPlugin.developerPrefix + "_NEMRY_BODY_UNLOCKABLE_ACHIEVEMENT_NAME";
        public override string PrerequisiteUnlockableIdentifier { get; } = HenryPlugin.developerPrefix + "_HENRY_BODY_UNLOCKABLE_REWARD_ID";
        public override string UnlockableNameToken { get; } = HenryPlugin.developerPrefix + "_NEMRY_BODY_UNLOCKABLE_UNLOCKABLE_NAME";
        public override string AchievementDescToken { get; } = HenryPlugin.developerPrefix + "_NEMRY_BODY_UNLOCKABLE_ACHIEVEMENT_DESC";
        public override Sprite Sprite { get; } = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texNemryAchievement");

        public override Func<string> GetHowToUnlock { get; } = (() => Language.GetStringFormatted("UNLOCK_VIA_ACHIEVEMENT_FORMAT", new object[]
                            {
                                Language.GetString(HenryPlugin.developerPrefix + "_NEMRY_BODY_UNLOCKABLE_ACHIEVEMENT_NAME"),
                                Language.GetString(HenryPlugin.developerPrefix + "_NEMRY_BODY_UNLOCKABLE_ACHIEVEMENT_DESC")
                            }));
        public override Func<string> GetUnlocked { get; } = (() => Language.GetStringFormatted("UNLOCKED_FORMAT", new object[]
                            {
                                Language.GetString(HenryPlugin.developerPrefix + "_NEMRY_BODY_UNLOCKABLE_ACHIEVEMENT_NAME"),
                                Language.GetString(HenryPlugin.developerPrefix + "_NEMRY_BODY_UNLOCKABLE_ACHIEVEMENT_DESC")
                            }));

        private void Check(Run run)
        {
            if (base.isUserAlive) base.Grant();
        }

        public override void OnInstall()
        {
            base.OnInstall();

            NemryUnlockComponent.OnDeath += this.Check;
        }

        public override void OnUninstall()
        {
            base.OnUninstall();

            NemryUnlockComponent.OnDeath -= this.Check;
        }
    }
}