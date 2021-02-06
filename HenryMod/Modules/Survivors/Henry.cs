using BepInEx.Configuration;
using RoR2;
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
                characterPrefab = Modules.Prefabs.CreatePrefab(bodyName, "mdlHenry", new BodyInfo
                {
                    armor = 5f,
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

                displayPrefab = Modules.Prefabs.CreateDisplayPrefab("HenryDisplay", characterPrefab);

                Modules.Prefabs.RegisterNewSurvivor(characterPrefab, displayPrefab, Color.grey, "HENRY", HenryPlugin.developerPrefix + "_HENRY_BODY_UNLOCKABLE_REWARD_ID");

                CreateSkins();
            }
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
                Assets.mainAssetBundle.LoadAsset<Sprite>("texMainSkin"),
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