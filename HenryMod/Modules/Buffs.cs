using R2API;
using RoR2;
using UnityEngine;

namespace HenryMod.Modules
{
    public static class Buffs
    {
        // armor buff gained during roll
        internal static BuffIndex armorBuff;

        internal static void RegisterBuffs()
        {
            armorBuff = AddNewBuff("HenryArmorBuff", "Textures/BuffIcons/texBuffGenericShield", Color.white, false, false);
        }

        // simple helper method
        internal static BuffIndex AddNewBuff(string buffName, string iconPath, Color buffColor, bool canStack, bool isDebuff)
        {
            CustomBuff tempBuff = new CustomBuff(new BuffDef
            {
                name = buffName,
                iconPath = iconPath,
                buffColor = buffColor,
                canStack = canStack,
                isDebuff = isDebuff,
                eliteIndex = EliteIndex.None
            });

            return BuffAPI.Add(tempBuff);
        }
    }
}