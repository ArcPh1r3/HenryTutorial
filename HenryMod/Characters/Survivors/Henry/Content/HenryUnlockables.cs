using RoR2;
using UnityEngine;

namespace HenryMod.Survivors.Henry
{
    public static class HenryUnlockables
    {
        public static UnlockableDef characterUnlockableDef = null;
        public static UnlockableDef masterySkinUnlockableDef = null;

        public static void Init()
        {
            masterySkinUnlockableDef = Modules.Content.CreateAndAddUnlockbleDef(
                HenrySurvivor.HENRY_PREFIX + "masteryUnlockable",
                HenrySurvivor.HENRY_PREFIX + "MASTERY_UNLOCKABLE",
                HenrySurvivor.instance.assetBundle.LoadAsset<Sprite>("texHenryIcon"));
        }
    }
}
