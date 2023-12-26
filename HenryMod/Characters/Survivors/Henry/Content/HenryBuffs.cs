//todo windows change namespace

using RoR2;
using UnityEngine;

namespace HenryMod.Characters.Survivors.Henry.Content {

    public static class HenryBuffs {
        // armor buff gained during roll
        public static BuffDef armorBuff;

        internal static void Init(AssetBundle assetBundle) {
            armorBuff = Modules.Content.CreateBuff("HenryArmorBuff",
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/HiddenInvincibility").iconSprite,
                Color.white,
                false,
                false);

        }
    }
}
