using R2API;

namespace HenryMod.Modules
{
    internal static class Unlockables
    {
        internal static void RegisterUnlockables()
        {
            UnlockablesAPI.AddUnlockable<Achievements.HenryUnlockAchievement>(true);
            UnlockablesAPI.AddUnlockable<Achievements.MasteryAchievement>(true);
        }
    }
}