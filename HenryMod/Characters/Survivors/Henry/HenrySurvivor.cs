using BepInEx.Configuration;
using HenryMod.Characters.Survivors.Henry.Content;
using HenryMod.Henry.Components;
using HenryMod.Modules.Characters;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using UnityEngine;

//todo windows namespace
namespace HenryMod.Modules.Survivors
{
    internal class HenrySurvivor : SurvivorBase<HenrySurvivor>
    {
        //todo guide
        public override string assetBundleName => "myassetbundlee";

        public override string bodyName => "HenryBody";
        //used when building your character using the prefabs you set up in unity
        public override string modelPrefabName => "mdlHenry";
        public override string displayPrefabName => "HenryDisplay";

        public const string HENRY_PREFIX = HenryPlugin.DEVELOPER_PREFIX + "_HENRY_";

        //used when registering your survivor's language tokens
        public override string survivorTokenPrefix => HENRY_PREFIX;

        public override BodyInfo bodyInfo { get; } = new BodyInfo
        {
            bodyName = instance.bodyName,
            bodyNameToken = HENRY_PREFIX + "NAME",
            subtitleNameToken = HENRY_PREFIX + "SUBTITLE",

            characterPortrait = instance.assetBundle.LoadAsset<Texture>("texHenryIcon"),
            bodyColor = Color.white,

            crosshair = Modules.Assets.LoadCrosshair("Standard"),
            podPrefab = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/SurvivorPod"),

            maxHealth = 110f,
            healthRegen = 1.5f,
            armor = 0f,

            jumpCount = 1,
        };

        public override CustomRendererInfo[] customRendererInfos { get; } = new CustomRendererInfo[] 
        {
                new CustomRendererInfo
                {
                    childName = "SwordModel",
                    //material = instance.assetBundle.LoadMaterial("matHenrySword"),
                },
                new CustomRendererInfo
                {
                    childName = "GunModel",
                },
                new CustomRendererInfo
                {
                    childName = "Model",
                }
        };

        public override UnlockableDef characterUnlockableDef => HenryUnlockables.characterUnlockableDef;

        public override ItemDisplaysBase itemDisplays => new HenryItemDisplays();

        public static ConfigEntry<bool> characterEnabled;

        public override void InitializeCharacter()
        {
            //uncomment if you have multiple characters
            //characterEnabled = Config.CharacterEnableConfig("Survivors", "Henry");

            //if (!characterEnabled.Value)
            //    return;

            //need the character unlockable before you initialize the survivordef
            HenryUnlockables.Init();

            base.InitializeCharacter();

            HenryConfig.Init();
            HenryStates.Init();
            HenryTokens.Init();

            HenryAssets.Init(assetBundle);
            HenryBuffs.Init(assetBundle);

            AdditionalBodySetup();

            InitializeEntityStateMachines();

            InitializeSkills();

            InitializeSkins();

            InitializeCharacterMaster();

            AddHooks();
        }

        private void AdditionalBodySetup() {
            AddHitboxes();
            bodyPrefab.AddComponent<HenryWeaponComponent>();
            //bodyPrefab.AddComponent<HuntressTrackerComopnent>();
            //anything else here
        }

        public void AddHitboxes() {
            ChildLocator childLocator = bodyPrefab.GetComponentInChildren<ChildLocator>();

            //example of how to create a hitbox
            Transform hitboxTransform = childLocator.FindChild("SwordHitbox");
            Modules.Prefabs.SetupHitbox(characterModelObject, hitboxTransform, "Sword");
        }

        public override void InitializeEntityStateMachines()
        {
            //clear existing state machines from your cloned body probably commando
            //omit all this if you want to just keep his
            Modules.Prefabs.ClearEntityStateMachines(bodyPrefab);

            //if you set up a custom main characterstate, set it up here
            //don't forget to register custom entitystates in your HenryStates.cs
            //todo setup example state?
            Modules.Prefabs.AddEntityStateMachine(bodyPrefab, "Body", typeof(EntityStates.GenericCharacterMain), typeof(EntityStates.GenericCharacterSpawnState));
            Modules.Prefabs.AddEntityStateMachine(bodyPrefab, "Weapon");
            Modules.Prefabs.AddEntityStateMachine(bodyPrefab, "Weapon2");
        }

        #region skills
        public override void InitializeSkills()
        {
            Modules.Skills.CreateSkillFamilies(bodyPrefab);
            AddPrmarySkills();
            AddSecondarySkills();
            AddUtiitySkills();
            AddSpecialSkills();
        }

        //let's look at secondary before primary because it is simpler
        private void AddSecondarySkills()
        {
            //here is a basic skill def with all fields accounted for
            SkillDef gunSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "HenryGun",
                skillNameToken = HENRY_PREFIX + "SECONDARY_GUN_NAME",
                skillDescriptionToken = HENRY_PREFIX + "SECONDARY_GUN_DESCRIPTION",
                keywordTokens = new string[] { "KEYWORD_AGILE" },
                skillIcon = assetBundle.LoadAsset<Sprite>("texSecondaryIcon"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Shoot)),
                activationStateMachineName = "Weapon2",
                interruptPriority = EntityStates.InterruptPriority.Skill,

                baseRechargeInterval = 1f,
                baseMaxStock = 1,

                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = true,
                dontAllowPastMaxStocks = false,
                beginSkillCooldownOnSkillEnd = false,
                mustKeyPress = false,

                isCombatSkill = true,
                canceledFromSprinting = false,
                cancelSprintingOnActivation = false,
                forceSprintDuringState = false,

            });

            Modules.Skills.AddSecondarySkills(bodyPrefab, gunSkillDef);
        }

        private void AddPrmarySkills()
        {
            //the primary skill is created using a constructor for a typical primary
            //it is also a SteppedSkillDef. Custom Skilldefs are very useful for custom behaviors related to casting a skill. see ror2's different skilldefs for reference
            SteppedSkillDef slashSkillDef = Modules.Skills.CreateSkillDef<SteppedSkillDef>(new SkillDefInfo
                (
                    "HenrySlash",
                    HENRY_PREFIX + "PRIMARY_SLASH_NAME",
                    HENRY_PREFIX + "PRIMARY_SLASH_DESCRIPTION",
                    assetBundle.LoadAsset<Sprite>("texPrimaryIcon"),
                    new EntityStates.SerializableEntityStateType(typeof(SkillStates.SlashCombo)),
                    "Weapon",
                    true
                ));
            //custom Skilldefs can have additional fields that you can set manually
            slashSkillDef.stepCount = 2;
            slashSkillDef.stepGraceDuration = 0.5f;

            Modules.Skills.AddPrimarySkills(bodyPrefab, slashSkillDef);
        }

        private void AddUtiitySkills()
        {
            //here's a skilldef of a typical movement skill. some fields are omitted and will just have default values
            SkillDef rollSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "HenryRoll",
                skillNameToken = HENRY_PREFIX + "UTILITY_ROLL_NAME",
                skillDescriptionToken = HENRY_PREFIX + "UTILITY_ROLL_DESCRIPTION",
                skillIcon = assetBundle.LoadAsset<Sprite>("texUtilityIcon"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Roll)),
                activationStateMachineName = "Body",
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,

                baseMaxStock = 1,
                baseRechargeInterval = 4f,

                isCombatSkill = false,
                mustKeyPress = false,
                forceSprintDuringState = true,
                cancelSprintingOnActivation = false,
            });

            Modules.Skills.AddUtilitySkills(bodyPrefab, rollSkillDef);
        }

        private void AddSpecialSkills()
        {
            //a basic skill
            SkillDef bombSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "HenryBomb",
                skillNameToken = HENRY_PREFIX + "SPECIAL_BOMB_NAME",
                skillDescriptionToken = HENRY_PREFIX + "SPECIAL_BOMB_DESCRIPTION",
                skillIcon = assetBundle.LoadAsset<Sprite>("texSpecialIcon"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.ThrowBomb)),
                activationStateMachineName = "Weapon2",
                interruptPriority = EntityStates.InterruptPriority.Skill,

                baseMaxStock = 1,
                baseRechargeInterval = 10f,

                isCombatSkill = true,
                mustKeyPress = false,
            });

            Modules.Skills.AddSpecialSkills(bodyPrefab, bombSkillDef);
        }
        #endregion skills

        #region skins
        public override void InitializeSkins()
        {
            ModelSkinController skinController = prefabCharacterModel.gameObject.AddComponent<ModelSkinController>();
            ChildLocator childLocator = prefabCharacterModel.GetComponent<ChildLocator>();

            CharacterModel.RendererInfo[] defaultRendererinfos = prefabCharacterModel.baseRendererInfos;

            List<SkinDef> skins = new List<SkinDef>();

            #region DefaultSkin
            //this creates a SkinDef with all default fields
            SkinDef defaultSkin = Modules.Skins.CreateSkinDef("DEFAULT_SKIN",
                assetBundle.LoadAsset<Sprite>("texMainSkin"),
                defaultRendererinfos,
                prefabCharacterModel.gameObject);

            //these are your Mesh Replacements. The order here is based on your CustomRendererInfos from earlier
            //pass in meshes as they are named in your assetbundle
            //currently not needed as with only 1 skin they will simply take the default meshes
                //uncomment this when you have another skin
            //defaultSkin.meshReplacements = Modules.Skins.getMeshReplacements(defaultRendererinfos,
            //    "meshHenrySword",
            //    "meshHenryGun",
            //    "meshHenry");

            //add new skindef to our list of skindefs. this is what we'll be passing to the SkinController
            skins.Add(defaultSkin);
            #endregion
            
            //uncomment this when you have a mastery skin
            #region MasterySkin
            /*
            //creating a new skindef as we did before
            SkinDef masterySkin = Modules.Skins.CreateSkinDef(HenryPlugin.DEVELOPER_PREFIX + "_HENRY_BODY_MASTERY_SKIN_NAME",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texMasteryAchievement"),
                defaultRendererinfos,
                prefabCharacterModel.gameObject,
                masterySkinUnlockableDef);

            //adding the mesh replacements as above. 
            //if you don't want to replace the mesh (for example, you only want to replace the material), pass in null so the order is preserved
            masterySkin.meshReplacements = Modules.Skins.getMeshReplacements(defaultRendererinfos,
                "meshHenrySwordAlt",
                null,//no gun mesh replacement. use same gun mesh
                "meshHenryAlt");

            //masterySkin has a new set of RendererInfos (based on default rendererinfos)
            //you can simply access the RendererInfos defaultMaterials and set them to the new materials for your skin.
            masterySkin.rendererInfos[0].defaultMaterial = Modules.Materials.CreateHopooMaterial("matHenryAlt");
            masterySkin.rendererInfos[1].defaultMaterial = Modules.Materials.CreateHopooMaterial("matHenryAlt");
            masterySkin.rendererInfos[2].defaultMaterial = Modules.Materials.CreateHopooMaterial("matHenryAlt");

            //here's a barebones example of using gameobjectactivations that could probably be streamlined or rewritten entirely, truthfully, but it works
            masterySkin.gameObjectActivations = new SkinDef.GameObjectActivation[]
            {
                new SkinDef.GameObjectActivation
                {
                    gameObject = childLocator.FindChildGameObject("GunModel"),
                    shouldActivate = false,
                }
            };
            //simply find an object on your child locator you want to activate/deactivate and set if you want to activate/deacitvate it with this skin

            skins.Add(masterySkin);
            */
            #endregion

            skinController.skins = skins.ToArray();
        }
        #endregion skins

        //Character Master is what governs the AI of your character when it is not controlled by a player (artifact of vengeance, goobo)
        public override void InitializeCharacterMaster() {
            //if you're lazy or prototyping you can simply copy the AI of a different character to be used
            //Modules.Prefabs.CloneAndAddDopplegangerMaster(bodyPrefab, bodyName + "MonsterMaster", "Merc");

            //how to set up AI in code
            HenryAI.Init(bodyPrefab);

            //how to load a master set up in unity (recommended)
            Modules.Prefabs.LoadMaster(assetBundle, "HenryMaster", bodyPrefab);
        }

        private void AddHooks() {

            R2API.RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, R2API.RecalculateStatsAPI.StatHookEventArgs args) {

            if (sender.HasBuff(HenryBuffs.armorBuff)) {
                args.armorAdd += 300;
            }
        }
    }
}