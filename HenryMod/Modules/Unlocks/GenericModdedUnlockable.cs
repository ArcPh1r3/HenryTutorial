using RoR2;
using System;
using UnityEngine;
using R2API;

namespace HenryMod.Modules
{
    public abstract class GenericModdedUnlockable : ModdedUnlockable
    {
        public abstract string AchievementTokenPrefix { get; }

        public override string AchievementIdentifier { get => AchievementTokenPrefix + "UNLOCKABLE_ACHIEVEMENT_ID"; }
        public override string UnlockableIdentifier { get => AchievementTokenPrefix + "UNLOCKABLE_REWARD_ID"; }
        public override string AchievementNameToken { get => AchievementTokenPrefix + "UNLOCKABLE_ACHIEVEMENT_NAME"; }
        public override string AchievementDescToken { get => AchievementTokenPrefix + "UNLOCKABLE_ACHIEVEMENT_DESC"; }
        public override string UnlockableNameToken { get => AchievementTokenPrefix + "UNLOCKABLE_UNLOCKABLE_NAME"; }

        public override Func<string> GetHowToUnlock
        {
            get => () => RoR2.Language.GetStringFormatted("UNLOCK_VIA_ACHIEVEMENT_FORMAT", new object[]
                            {
                                RoR2.Language.GetString(AchievementNameToken),
                                RoR2.Language.GetString(AchievementDescToken)
                            });
        }

        public override Func<string> GetUnlocked
        {
            get => () => RoR2.Language.GetStringFormatted("UNLOCKED_FORMAT", new object[]
                            {
                                RoR2.Language.GetString(AchievementNameToken),
                                RoR2.Language.GetString(AchievementDescToken)
                            });
        }
    }
}