using R2API;
using R2API.Utils;
using RoR2;
using System;

namespace HenryMod.Modules.Achievements
{
    [R2APISubmoduleDependency(nameof(UnlockablesAPI))]

    public class HenryUnlockAchievement : ModdedUnlockableAndAchievement<CustomSpriteProvider>
    {
        public override String AchievementIdentifier { get; } = HenryPlugin.developerPrefix + "_HENRY_BODY_UNLOCKABLE_ACHIEVEMENT_ID";
        public override String UnlockableIdentifier { get; } = HenryPlugin.developerPrefix + "_HENRY_BODY_UNLOCKABLE_REWARD_ID";
        public override String PrerequisiteUnlockableIdentifier { get; } = "";
        public override String AchievementNameToken { get; } = HenryPlugin.developerPrefix + "_HENRY_BODY_UNLOCKABLE_ACHIEVEMENT_NAME";
        public override String AchievementDescToken { get; } = HenryPlugin.developerPrefix + "_HENRY_BODY_UNLOCKABLE_ACHIEVEMENT_DESC";
        public override String UnlockableNameToken { get; } = HenryPlugin.developerPrefix + "_HENRY_BODY_UNLOCKABLE_UNLOCKABLE_NAME";
        protected override CustomSpriteProvider SpriteProvider { get; } = new CustomSpriteProvider("@Henry:Assets/HenryAssets/Icons/texHenryAchievement.png");

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