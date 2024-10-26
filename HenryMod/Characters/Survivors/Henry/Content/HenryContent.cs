using HenryMod.Survivors.Henry.Achievements;
using HenryMod.Survivors.Henry;
using HenryMod.Survivors.Henry.SkillStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace HenryMod.Characters.Survivors.Henry.Content
{
    public class HenryContent
    {
        public static AssetBundle assetBundle;

        //static values
        public const float swordDamageCoefficient = 2.8f;
        public const float gunDamageCoefficient = 4.2f;
        public const float bombDamageCoefficient = 16f;

        public static UnlockableDef characterUnlockableDef = null;
        public static UnlockableDef masterySkinUnlockableDef = null;

        // armor buff gained during roll
        public static BuffDef armorBuff;

        //stuff needed before character creation
        public static void PreInit()
        {
            InitUnlockables();
        }
        public static void Init(AssetBundle assetBundle_)
        {
            assetBundle = assetBundle_;

            InitStates();
            InitBuffs();
        }

        #region pre-init
        public static void InitUnlockables()
        {
            masterySkinUnlockableDef = Modules.Content.CreateAndAddUnlockbleDef(
                HenryMasteryAchievement.unlockableIdentifier,
                Modules.Tokens.GetAchievementNameToken(HenryMasteryAchievement.identifier),
                HenrySurvivor.instance.assetBundle.LoadAsset<Sprite>("texMasteryAchievement"));
        }
        #endregion pre-init

        #region init
        private static void InitStates()
        {
            Modules.Content.AddEntityState(typeof(SlashCombo));
            Modules.Content.AddEntityState(typeof(Shoot));
            Modules.Content.AddEntityState(typeof(Roll));
            Modules.Content.AddEntityState(typeof(ThrowBomb));
        }

        private static void InitBuffs()
        {
            armorBuff = Modules.Content.CreateAndAddBuff("HenryArmorBuff",
                            LegacyResourcesAPI.Load<BuffDef>("BuffDefs/HiddenInvincibility").iconSprite,
                            Color.white,
                            false,
                            false);
        }
        #endregion init
    }
}
