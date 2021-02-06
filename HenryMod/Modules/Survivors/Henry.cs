using BepInEx.Configuration;
using RoR2;
using RoR2.Skills;
using System.Collections.Generic;
using UnityEngine;

namespace HenryMod.Modules.Survivors
{
    internal static class Henry
    {
        internal static GameObject characterPrefab;
        internal static GameObject displayPrefab;

        internal static ConfigEntry<bool> characterEnabled;

        public const string bodyName = "RobHenryBody";

        internal static void CreateCharacter()
        {
            // this creates a config option to enable the character- feel free to remove if the character is the only thing in your mod
            characterEnabled = Modules.Config.CharacterEnableConfig("Henry");

            if (characterEnabled.Value)
            {
                #region Body
                characterPrefab = Modules.Prefabs.CreatePrefab(bodyName, "mdlHenry", new BodyInfo
                {
                    armor = 20f,
                    armorGrowth = 0f,
                    bodyName = bodyName,
                    bodyNameToken = HenryPlugin.developerPrefix + "_HENRY_BODY_NAME",
                    characterPortrait = Modules.Assets.LoadCharacterIcon("Henry"),
                    crosshair = Resources.Load<GameObject>("Prefabs/Crosshair/StandardCrosshair"),
                    damage = 12f,
                    healthGrowth = 33f,
                    healthRegen = 1.5f,
                    jumpCount = 1,
                    maxHealth = 110f,
                    subtitleNameToken = HenryPlugin.developerPrefix + "_HENRY_BODY_SUBTITLE",
                    podPrefab = Resources.Load<GameObject>("Prefabs/NetworkedObjects/SurvivorPod")
                });
                #endregion

                #region Model
                Material henryMat = Modules.Assets.CreateMaterial("matHenry");

                Modules.Prefabs.SetupCharacterModel(characterPrefab, new CustomRendererInfo[]
                {
                new CustomRendererInfo
                {
                    childName = "SwordModel",
                    material = henryMat,
                },
                new CustomRendererInfo
                {
                    childName = "GunModel",
                    material = henryMat,
                },
                new CustomRendererInfo
                {
                    childName = "Model",
                    material = henryMat,
                }
                }, 0);
                #endregion

                displayPrefab = Modules.Prefabs.CreateDisplayPrefab("HenryDisplay", characterPrefab);

                Modules.Prefabs.RegisterNewSurvivor(characterPrefab, displayPrefab, Color.grey, "HENRY", HenryPlugin.developerPrefix + "_HENRY_BODY_UNLOCKABLE_REWARD_ID");

                CreateHitboxes();
                CreateSkills();
                CreateSkins();
            }
        }

        private static void CreateHitboxes()
        {
            ChildLocator childLocator = characterPrefab.GetComponentInChildren<ChildLocator>();
            GameObject model = childLocator.gameObject;
            Transform hitboxTransform = childLocator.FindChild("SwordHitbox");
            Modules.Prefabs.SetupHitbox(model, hitboxTransform, "Sword");
        }

        private static void CreateSkills()
        {
            Modules.Skills.CreateSkillFamilies(characterPrefab);

            string prefix = HenryPlugin.developerPrefix;

            #region Primary
            Modules.Skills.AddPrimarySkill(characterPrefab, Modules.Skills.CreatePrimarySkillDef(new EntityStates.SerializableEntityStateType(typeof(SkillStates.SlashCombo)), "Weapon", prefix + "_HENRY_BODY_PRIMARY_SLASH_NAME", prefix + "_HENRY_BODY_PRIMARY_SLASH_DESCRIPTION", Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texPrimaryIcon"), true));
            #endregion

            #region Secondary
            SkillDef shootSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_HENRY_BODY_SECONDARY_GUN_NAME",
                skillNameToken = prefix + "_HENRY_BODY_SECONDARY_GUN_NAME",
                skillDescriptionToken = prefix + "_HENRY_BODY_SECONDARY_GUN_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSecondaryIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Shoot)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 1f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                isBullets = false,
                isCombatSkill = false,
                mustKeyPress = false,
                noSprint = true,
                rechargeStock = 1,
                requiredStock = 1,
                shootDelay = 0f,
                stockToConsume = 1
            });

            Modules.Skills.AddSecondarySkill(characterPrefab, shootSkillDef);
            #endregion

            #region Utility
            SkillDef rollSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_HENRY_BODY_UTILITY_ROLL_NAME",
                skillNameToken = prefix + "_HENRY_BODY_UTILITY_ROLL_NAME",
                skillDescriptionToken = prefix + "_HENRY_BODY_UTILITY_ROLL_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texUtilityIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Roll)),
                activationStateMachineName = "Body",
                baseMaxStock = 1,
                baseRechargeInterval = 4f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = true,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                isBullets = false,
                isCombatSkill = false,
                mustKeyPress = false,
                noSprint = false,
                rechargeStock = 1,
                requiredStock = 1,
                shootDelay = 0f,
                stockToConsume = 1
            });

            Modules.Skills.AddUtilitySkill(characterPrefab, rollSkillDef);
            #endregion

            #region Special
            SkillDef bombSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_HENRY_BODY_SPECIAL_BOMB_NAME",
                skillNameToken = prefix + "_HENRY_BODY_SPECIAL_BOMB_NAME",
                skillDescriptionToken = prefix + "_HENRY_BODY_SPECIAL_BOMB_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSpecialIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.ThrowBomb)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 8f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                isBullets = false,
                isCombatSkill = true,
                mustKeyPress = false,
                noSprint = true,
                rechargeStock = 1,
                requiredStock = 1,
                shootDelay = 0f,
                stockToConsume = 1
            });

            Modules.Skills.AddSpecialSkill(characterPrefab, bombSkillDef);
            #endregion
        }

        private static void CreateSkins()
        {
            GameObject model = characterPrefab.GetComponentInChildren<ModelLocator>().modelTransform.gameObject;
            CharacterModel characterModel = model.GetComponent<CharacterModel>();

            ModelSkinController skinController = model.AddComponent<ModelSkinController>();
            ChildLocator childLocator = model.GetComponent<ChildLocator>();

            SkinnedMeshRenderer mainRenderer = characterModel.mainSkinnedMeshRenderer;

            CharacterModel.RendererInfo[] defaultRenderers = characterModel.baseRendererInfos;

            List<SkinDef> skins = new List<SkinDef>();

            #region DefaultSkin
            SkinDef defaultSkin = Modules.Skins.CreateSkinDef(HenryPlugin.developerPrefix + "_HENRY_BODY_DEFAULT_SKIN_NAME",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texMainSkin"),
                defaultRenderers,
                mainRenderer,
                model);
            skins.Add(defaultSkin);
            #endregion

            #region MasterySkin
            Material masteryMat = Modules.Assets.CreateMaterial("matHenryAlt");
            CharacterModel.RendererInfo[] masteryRendererInfos = SkinRendererInfos(defaultRenderers, new Material[]
            {
                masteryMat,
                masteryMat,
                masteryMat
            });

            SkinDef masterySkin = Modules.Skins.CreateSkinDef(HenryPlugin.developerPrefix + "_HENRY_BODY_MASTERY_SKIN_NAME",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texMasteryAchievement"),
                masteryRendererInfos,
                mainRenderer,
                model,
                HenryPlugin.developerPrefix + "_HENRY_BODY_MASTERYUNLOCKABLE_REWARD_ID");
            skins.Add(masterySkin);
            #endregion

            skinController.skins = skins.ToArray();
        }

        private static CharacterModel.RendererInfo[] SkinRendererInfos(CharacterModel.RendererInfo[] defaultRenderers, Material[] materials)
        {
            CharacterModel.RendererInfo[] newRendererInfos = new CharacterModel.RendererInfo[defaultRenderers.Length];
            defaultRenderers.CopyTo(newRendererInfos, 0);

            newRendererInfos[0].defaultMaterial = Modules.Assets.CreateMaterial("matHenryAlt");
            newRendererInfos[1].defaultMaterial = Modules.Assets.CreateMaterial("matHenryAlt");
            newRendererInfos[2].defaultMaterial = Modules.Assets.CreateMaterial("matHenryAlt");

            return newRendererInfos;
        }
    }
}