using RoR2;
using System;
using UnityEngine;

namespace HenryMod.Modules.Achievements
{
    internal class HenryUnlockAchievement : ModdedUnlockable
    {
        public override string AchievementIdentifier { get; } = HenryPlugin.developerPrefix + "_HENRY_BODY_UNLOCKABLE_ACHIEVEMENT_ID";
        public override string UnlockableIdentifier { get; } = HenryPlugin.developerPrefix + "_HENRY_BODY_UNLOCKABLE_REWARD_ID";
        public override string AchievementNameToken { get; } = HenryPlugin.developerPrefix + "_HENRY_BODY_UNLOCKABLE_ACHIEVEMENT_NAME";
        public override string PrerequisiteUnlockableIdentifier { get; } = "";
        public override string UnlockableNameToken { get; } = HenryPlugin.developerPrefix + "_HENRY_BODY_UNLOCKABLE_UNLOCKABLE_NAME";
        public override string AchievementDescToken { get; } = HenryPlugin.developerPrefix + "_HENRY_BODY_UNLOCKABLE_ACHIEVEMENT_DESC";
        public override Sprite Sprite { get; } = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texHenryAchievement");

        public override Func<string> GetHowToUnlock { get; } = (() => Language.GetStringFormatted("UNLOCK_VIA_ACHIEVEMENT_FORMAT", new object[]
                            {
                                Language.GetString(HenryPlugin.developerPrefix + "_HENRY_BODY_UNLOCKABLE_ACHIEVEMENT_NAME"),
                                Language.GetString(HenryPlugin.developerPrefix + "_HENRY_BODY_UNLOCKABLE_ACHIEVEMENT_DESC")
                            }));
        public override Func<string> GetUnlocked { get; } = (() => Language.GetStringFormatted("UNLOCKED_FORMAT", new object[]
                            {
                                Language.GetString(HenryPlugin.developerPrefix + "_HENRY_BODY_UNLOCKABLE_ACHIEVEMENT_NAME"),
                                Language.GetString(HenryPlugin.developerPrefix + "_HENRY_BODY_UNLOCKABLE_ACHIEVEMENT_DESC")
                            }));

        private void Check(On.RoR2.SceneDirector.orig_Start orig, SceneDirector self)
        {
            if (self)
            {
                string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
                if (sceneName == "golemplains" || sceneName == "golemplains2")
                {
                    base.Grant();
                }
            }

            Debug.Log("haha fuyck hopooo!!");

            orig(self);
        }

        public override void OnInstall()
        {
            base.OnInstall();

            On.RoR2.SceneDirector.Start += this.Check;
        }

        public override void OnUninstall()
        {
            base.OnUninstall();

            On.RoR2.SceneDirector.Start -= this.Check;
        }
    }
}