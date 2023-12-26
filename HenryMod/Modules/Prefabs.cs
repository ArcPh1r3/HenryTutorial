using R2API;
using RoR2;
using System.Collections.Generic;
using UnityEngine;
using HenryMod.Modules.Characters;
using RoR2.CharacterAI;
using static RoR2.CharacterAI.AISkillDriver;
using RoR2.Skills;

namespace HenryMod.Modules
{
    // module for creating body prefabs and whatnot
    // recommended to simply avoid touching this unless you REALLY need to

    internal static class Prefabs
    {
        // cache this just to give our ragdolls the same physic material as vanilla stuff
        private static PhysicMaterial ragdollMaterial;

        public static GameObject CreateDisplayPrefab(AssetBundle assetBundle, string displayPrefabName, GameObject prefab)
        {
            GameObject model = assetBundle.LoadAsset<GameObject>(displayPrefabName);
            if (model == null)
            {
                Log.Error($"could not load display prefab {displayPrefabName}. Make sure this prefab exists in assetbundle {assetBundle.name}");
                return null;
            }

            CharacterModel characterModel = model.GetComponent<CharacterModel>();
            if (!characterModel)
            {
                characterModel = model.AddComponent<CharacterModel>();
            }
            characterModel.baseRendererInfos = prefab.GetComponentInChildren<CharacterModel>().baseRendererInfos;

            //todo material
            Modules.Assets.ConvertAllRenderersToHopooShader(model);

            return model.gameObject;
        }

        public static GameObject CreateBodyPrefab(AssetBundle assetBundle, string modelName, BodyInfo bodyInfo)
        {
            GameObject clonedBody = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterBodies/" + bodyInfo.bodyNameToClone + "Body");
            if (!clonedBody)
            {
                Log.Error(bodyInfo.bodyNameToClone + "Body to clone is not a valid body, character creation failed");
                return null;
            }

            GameObject newBodyPrefab = PrefabAPI.InstantiateClone(clonedBody, bodyInfo.bodyName);

            GameObject model = assetBundle.LoadAsset<GameObject>(modelName);
            if (model == null)
            {
                Log.Error($"could not load model prefab {modelName}. Make sure this prefab exists in assetbundle {assetBundle.name}");
                return null;
            }

            //todo funny: why have this backup if you're going to delete it in this next function?
            //if (model == null) model = newBodyPrefab.GetComponentInChildren<CharacterModel>().gameObject;

            Transform modelBaseTransform = AddCharacterModelToSurvivorBody(newBodyPrefab, model.transform, bodyInfo);

            SetupCharacterBody(newBodyPrefab, bodyInfo);

            //todo setup
            SetupCameraTargetParams(newBodyPrefab, bodyInfo);
            SetupModelLocator(newBodyPrefab, modelBaseTransform, model.transform);
            //SetupRigidbody(newPrefab);
            SetupCapsuleCollider(newBodyPrefab);
            SetupMainHurtbox(newBodyPrefab, model);

            SetupAimAnimator(newBodyPrefab, model);

            if (modelBaseTransform != null) SetupCharacterDirection(newBodyPrefab, modelBaseTransform, model.transform);
            SetupFootstepController(model);
            SetupRagdoll(model);

            Modules.Content.AddCharacterBodyPrefab(newBodyPrefab);

            return newBodyPrefab;
        }

        private static void SetupCharacterBody(GameObject newBodyPrefab, BodyInfo bodyInfo)
        {
            CharacterBody bodyComponent = newBodyPrefab.GetComponent<CharacterBody>();
            //identity
            bodyComponent.baseNameToken = bodyInfo.bodyNameToken;
            bodyComponent.subtitleNameToken = bodyInfo.subtitleNameToken;
            bodyComponent.portraitIcon = bodyInfo.characterPortrait;
            bodyComponent.bodyColor = bodyInfo.bodyColor;

            bodyComponent._defaultCrosshairPrefab = bodyInfo.crosshair;
            bodyComponent.hideCrosshair = false;
            bodyComponent.preferredPodPrefab = bodyInfo.podPrefab;

            //stats
            bodyComponent.baseMaxHealth = bodyInfo.maxHealth;
            bodyComponent.baseRegen = bodyInfo.healthRegen;
            bodyComponent.baseArmor = bodyInfo.armor;
            bodyComponent.baseMaxShield = bodyInfo.shield;

            bodyComponent.baseDamage = bodyInfo.damage;
            bodyComponent.baseAttackSpeed = bodyInfo.attackSpeed;
            bodyComponent.baseCrit = bodyInfo.crit;

            bodyComponent.baseMoveSpeed = bodyInfo.moveSpeed;
            bodyComponent.baseJumpPower = bodyInfo.jumpPower;

            //level stats
            bodyComponent.autoCalculateLevelStats = bodyInfo.autoCalculateLevelStats;

            if (bodyInfo.autoCalculateLevelStats)
            {

                bodyComponent.levelMaxHealth = Mathf.Round(bodyComponent.baseMaxHealth * 0.3f);
                bodyComponent.levelMaxShield = Mathf.Round(bodyComponent.baseMaxShield * 0.3f);
                bodyComponent.levelRegen = bodyComponent.baseRegen * 0.2f;

                bodyComponent.levelMoveSpeed = 0f;
                bodyComponent.levelJumpPower = 0f;

                bodyComponent.levelDamage = bodyComponent.baseDamage * 0.2f;
                bodyComponent.levelAttackSpeed = 0f;
                bodyComponent.levelCrit = 0f;

                bodyComponent.levelArmor = 0f;

            }
            else
            {

                bodyComponent.levelMaxHealth = bodyInfo.healthGrowth;
                bodyComponent.levelMaxShield = bodyInfo.shieldGrowth;
                bodyComponent.levelRegen = bodyInfo.regenGrowth;

                bodyComponent.levelMoveSpeed = bodyInfo.moveSpeedGrowth;
                bodyComponent.levelJumpPower = bodyInfo.jumpPowerGrowth;

                bodyComponent.levelDamage = bodyInfo.damageGrowth;
                bodyComponent.levelAttackSpeed = bodyInfo.attackSpeedGrowth;
                bodyComponent.levelCrit = bodyInfo.critGrowth;

                bodyComponent.levelArmor = bodyInfo.armorGrowth;
            }
            //other
            bodyComponent.baseAcceleration = bodyInfo.acceleration;

            bodyComponent.baseJumpCount = bodyInfo.jumpCount;

            bodyComponent.sprintingSpeedMultiplier = 1.45f;

            bodyComponent.bodyFlags = CharacterBody.BodyFlags.ImmuneToExecutes;
            bodyComponent.rootMotionInMainState = false;

            bodyComponent.hullClassification = HullClassification.Human;

            bodyComponent.isChampion = false;
        }

        #region ModelSetup

        private static Transform AddCharacterModelToSurvivorBody(GameObject bodyPrefab, Transform modelTransform, BodyInfo bodyInfo)
        {
            for (int i = bodyPrefab.transform.childCount - 1; i >= 0; i--)
            {

                Object.DestroyImmediate(bodyPrefab.transform.GetChild(i).gameObject);
            }

            Transform modelBase = new GameObject("ModelBase").transform;
            modelBase.parent = bodyPrefab.transform;
            modelBase.localPosition = bodyInfo.modelBasePosition;
            modelBase.localRotation = Quaternion.identity;

            modelTransform.parent = modelBase.transform;
            modelTransform.localPosition = Vector3.zero;
            modelTransform.localRotation = Quaternion.identity;

            GameObject cameraPivot = new GameObject("CameraPivot");
            cameraPivot.transform.parent = bodyPrefab.transform;
            cameraPivot.transform.localPosition = bodyInfo.cameraPivotPosition;
            cameraPivot.transform.localRotation = Quaternion.identity;

            GameObject aimOrigin = new GameObject("AimOrigin");
            aimOrigin.transform.parent = bodyPrefab.transform;
            aimOrigin.transform.localPosition = bodyInfo.aimOriginPosition;
            aimOrigin.transform.localRotation = Quaternion.identity;
            bodyPrefab.GetComponent<CharacterBody>().aimOriginTransform = aimOrigin.transform;

            return modelBase.transform;
        }
        public static CharacterModel SetupCharacterModel(GameObject prefab) => SetupCharacterModel(prefab, null);
        public static CharacterModel SetupCharacterModel(GameObject prefab, CustomRendererInfo[] customInfos)
        {

            CharacterModel characterModel = prefab.GetComponent<ModelLocator>().modelTransform.gameObject.GetComponent<CharacterModel>();
            bool preattached = characterModel != null;
            if (!preattached)
                characterModel = prefab.GetComponent<ModelLocator>().modelTransform.gameObject.AddComponent<CharacterModel>();

            characterModel.body = prefab.GetComponent<CharacterBody>();

            characterModel.autoPopulateLightInfos = true;
            characterModel.invisibilityCount = 0;
            characterModel.temporaryOverlays = new List<TemporaryOverlay>();

            if (!preattached)
            {
                SetupCustomRendererInfos(characterModel, customInfos);
            }
            else
            {
                SetupPreAttachedRendererInfos(characterModel);
            }
            return characterModel;
        }

        public static void SetupPreAttachedRendererInfos(CharacterModel characterModel)
        {
            for (int i = 0; i < characterModel.baseRendererInfos.Length; i++)
            {
                if (characterModel.baseRendererInfos[i].defaultMaterial == null)
                    characterModel.baseRendererInfos[i].defaultMaterial = characterModel.baseRendererInfos[i].renderer.sharedMaterial;
                characterModel.baseRendererInfos[i].defaultMaterial.ConvertDefaultShaderToHopoo();
            }
        }

        public static void SetupCustomRendererInfos(CharacterModel characterModel, CustomRendererInfo[] customInfos)
        {

            ChildLocator childLocator = characterModel.GetComponent<ChildLocator>();
            if (!childLocator)
            {
                Log.Error("Failed CharacterModel setup: ChildLocator component does not exist on the model");
                return;
            }

            List<CharacterModel.RendererInfo> rendererInfos = new List<CharacterModel.RendererInfo>();

            for (int i = 0; i < customInfos.Length; i++)
            {
                if (!childLocator.FindChild(customInfos[i].childName))
                {
                    Log.Error("Trying to add a RendererInfo for a renderer that does not exist: " + customInfos[i].childName);
                }
                else
                {
                    Renderer rend = childLocator.FindChild(customInfos[i].childName).GetComponent<Renderer>();
                    if (rend)
                    {

                        Material mat = customInfos[i].material;

                        if (mat == null)
                        {
                            if (customInfos[i].dontHotpoo)
                            {
                                mat = rend.material;
                            }
                            else
                            {
                                mat = rend.material.ConvertDefaultShaderToHopoo();
                            }
                        }

                        rendererInfos.Add(new CharacterModel.RendererInfo
                        {
                            renderer = rend,
                            defaultMaterial = mat,
                            ignoreOverlays = customInfos[i].ignoreOverlays,
                            defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On
                        });
                    }
                }
            }

            characterModel.baseRendererInfos = rendererInfos.ToArray();
        }
        #endregion

        #region ComponentSetup
        //todo ser see which ones of these are fuckinnnnnnnnnnnnnnn serialized
        private static void SetupCharacterDirection(GameObject prefab, Transform modelBaseTransform, Transform modelTransform)
        {
            if (!prefab.GetComponent<CharacterDirection>())
                return;

            CharacterDirection characterDirection = prefab.GetComponent<CharacterDirection>();
            characterDirection.targetTransform = modelBaseTransform;
            characterDirection.overrideAnimatorForwardTransform = null;
            characterDirection.rootMotionAccumulator = null;
            characterDirection.modelAnimator = modelTransform.GetComponent<Animator>();
            characterDirection.driveFromRootRotation = false;
            characterDirection.turnSpeed = 720f;
        }

        private static void SetupCameraTargetParams(GameObject prefab, BodyInfo bodyInfo)
        {
            CameraTargetParams cameraTargetParams = prefab.GetComponent<CameraTargetParams>();
            cameraTargetParams.cameraParams = bodyInfo.cameraParams;
            cameraTargetParams.cameraPivotTransform = prefab.transform.Find("CameraPivot");
        }

        private static void SetupModelLocator(GameObject prefab, Transform modelBaseTransform, Transform modelTransform)
        {
            ModelLocator modelLocator = prefab.GetComponent<ModelLocator>();
            modelLocator.modelTransform = modelTransform;
            modelLocator.modelBaseTransform = modelBaseTransform;
        }

        //private static void SetupRigidbody(GameObject prefab)
        //{
        //    Rigidbody rigidbody = prefab.GetComponent<Rigidbody>();
        //    rigidbody.mass = 100f;
        //}

        private static void SetupCapsuleCollider(GameObject prefab)
        {
            CapsuleCollider capsuleCollider = prefab.GetComponent<CapsuleCollider>();
            capsuleCollider.center = new Vector3(0f, 0f, 0f);
            capsuleCollider.radius = 0.5f;
            capsuleCollider.height = 1.82f;
            capsuleCollider.direction = 1;
        }

        private static void SetupMainHurtbox(GameObject prefab, GameObject model)
        {
            ChildLocator childLocator = model.GetComponent<ChildLocator>();

            if (!childLocator.FindChild("MainHurtbox"))
            {
                Debug.LogWarning("Could not set up main hurtbox: make sure you have a transform pair in your prefab's ChildLocator component called 'MainHurtbox'");
                return;
            }

            HurtBoxGroup hurtBoxGroup = model.AddComponent<HurtBoxGroup>();
            HurtBox mainHurtbox = childLocator.FindChild("MainHurtbox").gameObject.AddComponent<HurtBox>();
            mainHurtbox.gameObject.layer = LayerIndex.entityPrecise.intVal;
            mainHurtbox.healthComponent = prefab.GetComponent<HealthComponent>();
            mainHurtbox.isBullseye = true;
            mainHurtbox.damageModifier = HurtBox.DamageModifier.Normal;
            mainHurtbox.hurtBoxGroup = hurtBoxGroup;
            mainHurtbox.indexInGroup = 0;

            hurtBoxGroup.hurtBoxes = new HurtBox[]
            {
                mainHurtbox
            };

            hurtBoxGroup.mainHurtBox = mainHurtbox;
            hurtBoxGroup.bullseyeCount = 1;
        }

        public static void SetupHurtBoxes(GameObject bodyPrefab)
        {

            HealthComponent healthComponent = bodyPrefab.GetComponent<HealthComponent>();

            foreach (HurtBoxGroup hurtboxGroup in bodyPrefab.GetComponentsInChildren<HurtBoxGroup>())
            {
                hurtboxGroup.mainHurtBox.healthComponent = healthComponent;
                for (int i = 0; i < hurtboxGroup.hurtBoxes.Length; i++)
                {
                    hurtboxGroup.hurtBoxes[i].healthComponent = healthComponent;
                }
            }
        }

        private static void SetupFootstepController(GameObject model)
        {
            FootstepHandler footstepHandler = model.AddComponent<FootstepHandler>();
            footstepHandler.baseFootstepString = "Play_player_footstep";
            footstepHandler.sprintFootstepOverrideString = "";
            footstepHandler.enableFootstepDust = true;
            footstepHandler.footstepDustPrefab = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/GenericFootstepDust");
        }

        private static void SetupRagdoll(GameObject model)
        {
            RagdollController ragdollController = model.GetComponent<RagdollController>();

            if (!ragdollController) return;

            if (ragdollMaterial == null) ragdollMaterial = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterBodies/CommandoBody").GetComponentInChildren<RagdollController>().bones[1].GetComponent<Collider>().material;

            foreach (Transform boneTransform in ragdollController.bones)
            {
                if (boneTransform)
                {
                    boneTransform.gameObject.layer = LayerIndex.ragdoll.intVal;
                    Collider boneCollider = boneTransform.GetComponent<Collider>();
                    if (boneCollider)
                    {
                        boneCollider.material = ragdollMaterial;
                        boneCollider.sharedMaterial = ragdollMaterial;
                    }
                }
            }
        }

        private static void SetupAimAnimator(GameObject prefab, GameObject model)
        {
            AimAnimator aimAnimator = model.AddComponent<AimAnimator>();
            aimAnimator.directionComponent = prefab.GetComponent<CharacterDirection>();
            aimAnimator.pitchRangeMax = 60f;
            aimAnimator.pitchRangeMin = -60f;
            aimAnimator.yawRangeMin = -80f;
            aimAnimator.yawRangeMax = 80f;
            aimAnimator.pitchGiveupRange = 30f;
            aimAnimator.yawGiveupRange = 10f;
            aimAnimator.giveupDuration = 3f;
            aimAnimator.inputBank = prefab.GetComponent<InputBankTest>();
        }
        #endregion ComponentSetup
        //todo windows editorconfig
        public static void CreateGenericDoppelganger(GameObject bodyPrefab, string masterName, string masterToCopy) => CloneDopplegangerMaster(bodyPrefab, masterName, masterToCopy);
        public static GameObject CloneDopplegangerMaster(GameObject bodyPrefab, string masterName, string masterToCopy)
        {
            GameObject newMaster = PrefabAPI.InstantiateClone(RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterMasters/" + masterToCopy + "MonsterMaster"), masterName, true);
            newMaster.GetComponent<CharacterMaster>().bodyPrefab = bodyPrefab;

            Modules.Content.AddMasterPrefab(newMaster);
            return newMaster;
        }

        public static GameObject CreateBlankMasterPrefab(GameObject bodyPrefab, string masterName)
        {
            GameObject masterObject = PrefabAPI.InstantiateClone(RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterMasters/CommandoMonsterMaster"), masterName, true);
            //should the user call this themselves?
            Modules.ContentPacks.masterPrefabs.Add(masterObject);

            CharacterMaster characterMaster = masterObject.GetComponent<CharacterMaster>();
            characterMaster.bodyPrefab = bodyPrefab;

            AISkillDriver[] drivers = masterObject.GetComponents<AISkillDriver>();
            for (int i = 0; i < drivers.Length; i++)
            {
                UnityEngine.Object.Destroy(drivers[i]);
            }

            return masterObject;
        }

        public static GameObject LoadMaster(this AssetBundle assetBundle, string assetName, GameObject bodyPrefab)
        {
            GameObject newMaster = assetBundle.LoadAsset<GameObject>(assetName);
            //todo ser
            //should we add and initialize a new master if one doesn't exist? or should we simply require one be put on the prefab in unity?
            newMaster.GetComponent<CharacterMaster>().bodyPrefab = bodyPrefab;

            Modules.Content.AddMasterPrefab(newMaster);
            return newMaster;
        }

        //nevermind actually adding a component and just editing it is better because you get default values
        public struct AISkillDriverInfo
        {
            //just gonna leave this here while I learn for now
            [Tooltip("The name of this skill driver for reference purposes.")]
            public string customName;

            [Tooltip("The slot of the associated skill. Set to None to allow this behavior to run regardless of skill availability.")]
            public SkillSlot skillSlot;

            [Header("Selection Conditions")]
            [Tooltip("The skill that the specified slot must have for this behavior to run. Set to none to allow any skill.")]
            public SkillDef requiredSkill;

            [Tooltip("If set, this cannot be the dominant driver while the skill is on cooldown or out of stock.")]
            public bool requireSkillReady;

            [Tooltip("If set, this cannot be the dominant driver while the equipment is on cooldown or out of stock.")]
            public bool requireEquipmentReady;

            [Tooltip("The minimum health fraction required of the user for this behavior.")]
            public float minUserHealthFraction;

            [Tooltip("The maximum health fraction required of the user for this behavior.")]
            public float maxUserHealthFraction;

            [Tooltip("The minimum health fraction required of the target for this behavior.")]
            public float minTargetHealthFraction;

            [Tooltip("The maximum health fraction required of the target for this behavior.")]
            public float maxTargetHealthFraction;

            [Tooltip("The minimum distance from the target required for this behavior.")]
            public float minDistance;

            [Tooltip("The maximum distance from the target required for this behavior.")]
            public float maxDistance;

            public bool selectionRequiresTargetLoS;

            public bool selectionRequiresOnGround;

            public bool selectionRequiresAimTarget;

            [Tooltip("The maximum number of times that this skill can be selected.  If the value is < 0, then there is no maximum.")]
            public int maxTimesSelected;

            [Header("Behavior")]
            [Tooltip("The type of object targeted for movement.")]
            public TargetType moveTargetType;

            [Tooltip("If set, this skill will not be activated unless there is LoS to the target.")]
            public bool activationRequiresTargetLoS;

            [Tooltip("If set, this skill will not be activated unless there is LoS to the aim target.")]
            public bool activationRequiresAimTargetLoS;

            [Tooltip("If set, this skill will not be activated unless the aim vector is pointing close to the target.")]
            public bool activationRequiresAimConfirmation;

            [Tooltip("The movement type to use while this is the dominant skill driver.")]
            public MovementType movementType;

            public float moveInputScale;

            [Tooltip("Where to look while this is the dominant skill driver")]
            public AimType aimType;

            [Tooltip("If set, the nodegraph will not be used to direct the local navigator while this is the dominant skill driver. Direction toward the target will be used instead.")]
            public bool ignoreNodeGraph;

            [Tooltip("If true, the AI will attempt to sprint while this is the dominant skill driver.")]
            public bool shouldSprint;

            public bool shouldFireEquipment;

            public ButtonPressType buttonPressType;

            [Header("Transition Behavior")]
            [Tooltip("If non-negative, this value will be used for the driver evaluation timer while this is the dominant skill driver.")]
            public float driverUpdateTimerOverride;

            [Tooltip("If set and this is the dominant skill driver, the current enemy will be reset at the time of the next evaluation.")]
            public bool resetCurrentEnemyOnNextDriverSelection;

            [Tooltip("If true, this skill driver cannot be chosen twice in a row.")]
            public bool noRepeat;

            [Tooltip("The AI skill driver that will be treated as having top priority after this one.")]
            public AISkillDriver nextHighPriorityOverride;
        }

        public static void SetupHitbox(GameObject prefab, Transform hitboxTransform, string hitboxName) => SetupHitbox(prefab, hitboxName, hitboxTransform);
        public static void SetupHitbox(GameObject prefab, string hitboxName, params Transform[] hitboxTransforms)
        {
            List<HitBox> hitBoxes = new List<HitBox>();

            foreach (Transform i in hitboxTransforms)
            {
                if (i == null)
                {
                    Log.Error($"Error setting up hitboxGroup for {hitboxName}: hitbox transform was null");
                    continue;
                }
                HitBox hitBox = i.gameObject.AddComponent<HitBox>();
                i.gameObject.layer = LayerIndex.projectile.intVal;
                hitBoxes.Add(hitBox);
            }

            if(hitBoxes.Count == 0)
            {
                Log.Error($"No hitboxes were set up. aborting setting up hitboxGroup for {hitboxName}");
                return;
            }

            HitBoxGroup hitBoxGroup = prefab.AddComponent<HitBoxGroup>();

            hitBoxGroup.hitBoxes = hitBoxes.ToArray();

            hitBoxGroup.groupName = hitboxName;
        }

    }

    // for simplifying rendererinfo creation
    public class CustomRendererInfo
    {
        //the childname according to how it's set up in your childlocator
        public string childName;
        //the material to use. pass in null to use the material in the bundle
        public Material material = null;
        //don't set the hopoo shader on the material, and simply use the material from your prefab, unchanged
        public bool dontHotpoo = false;
        //ignores shields and other overlays. use if you're not using a hopoo shader
        public bool ignoreOverlays = false;
    }
}