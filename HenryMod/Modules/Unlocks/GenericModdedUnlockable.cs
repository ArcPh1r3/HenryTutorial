using RoR2;
using System;
using UnityEngine;

namespace HenryMod.Modules
{
    public abstract class GenericModdedUnlockable : ModdedUnlockable
    {
        public abstract string AchievementTokenPrefix { get; }
        public abstract string AchievementSpriteName { get; }

        public override string AchievementIdentifier { get => AchievementTokenPrefix + "UNLOCKABLE_ACHIEVEMENT_ID"; }
        public override string UnlockableIdentifier { get => AchievementTokenPrefix + "UNLOCKABLE_REWARD_ID"; }
        public override string AchievementNameToken { get => AchievementTokenPrefix + "UNLOCKABLE_ACHIEVEMENT_NAME"; }
        public override string AchievementDescToken { get => AchievementTokenPrefix + "UNLOCKABLE_ACHIEVEMENT_DESC"; }
        public override string UnlockableNameToken { get => AchievementTokenPrefix + "UNLOCKABLE_UNLOCKABLE_NAME"; }

        public override Sprite Sprite => Assets.mainAssetBundle.LoadAsset<Sprite>(AchievementSpriteName);

        public override Func<string> GetHowToUnlock
        {
            get => () => Language.GetStringFormatted("UNLOCK_VIA_ACHIEVEMENT_FORMAT", new object[]
                            {
                                Language.GetString(AchievementNameToken),
                                Language.GetString(AchievementDescToken)
                            });
        }

        public override Func<string> GetUnlocked
        {
            get => () => Language.GetStringFormatted("UNLOCKED_FORMAT", new object[]
                            {
                                Language.GetString(AchievementNameToken),
                                Language.GetString(AchievementDescToken)
                            });
        }
    }
}