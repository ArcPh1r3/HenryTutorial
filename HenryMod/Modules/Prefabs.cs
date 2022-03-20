using R2API;
using RoR2;
using System.Collections.Generic;
using UnityEngine;
using HenryMod.Modules.Characters;

namespace HenryMod.Modules {
    // module for creating body prefabs and whatnot
    // recommended to simply avoid touching this unless you REALLY need to
    // oh boy do I need to

    internal static class Prefabs
    {
        // cache this just to give our ragdolls the same physic material as vanilla stuff
        private static PhysicMaterial ragdollMaterial;

        public static GameObject CreateDisplayPrefab(string displayModelName, GameObject prefab, BodyInfo bodyInfo)
        {
            GameObject model = Assets.LoadSurvivorModel(displayModelName);

            CharacterModel characterModel = model.GetComponent<CharacterModel>();
            if (!characterModel) {
                characterModel = model.AddComponent<CharacterModel>();
            }
            characterModel.baseRendererInfos = prefab.GetComponentInChildren<CharacterModel>().baseRendererInfos;

            Modules.Assets.ConvertAllRenderersToHopooShader(model);

            return model.gameObject;
        }

        public static GameObject CreateBodyPrefab(string bodyName, string modelName, BodyInfo bodyInfo)
        {
            GameObject clonedBody = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterBodies/" + bodyInfo.bodyNameToClone + "Body");
            if (!clonedBody)
            {
                Log.Error(bodyInfo.bodyNameToClone + "Body is not a valid body, character creation failed");
                return null;
            }

            GameObject newBodyPrefab = PrefabAPI.InstantiateClone(clonedBody, bodyName);

            Transform modelBaseTransform = null;
            GameObject model = null;
            if (modelName != "mdl")
            {
                model = Assets.LoadSurvivorModel(modelName);
                if (model == null) model = newBodyPrefab.GetComponentInChildren<CharacterModel>().gameObject;

                    modelBaseTransform = AddCharacterModelToSurvivorBody(newBodyPrefab, model.transform, bodyInfo);
            }

            #region CharacterBody
            CharacterBody bodyComponent = newBodyPrefab.GetComponent<CharacterBody>();
            //identity
            bodyComponent.name = bodyInfo.bodyName;
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
            bodyComponent.levelArmor = bodyInfo.armorGrowth;
            bodyComponent.baseMaxShield = bodyInfo.shield;

            bodyComponent.baseDamage = bodyInfo.damage;
            bodyComponent.baseAttackSpeed = bodyInfo.attackSpeed;
            bodyComponent.baseCrit = bodyInfo.crit;

            bodyComponent.baseMoveSpeed = bodyInfo.moveSpeed;
            bodyComponent.baseJumpPower = bodyInfo.jumpPower;

            //level stats
            bodyComponent.autoCalculateLevelStats = bodyInfo.autoCalculateLevelStats;

            bodyComponent.levelDamage = bodyInfo.damageGrowth;
            bodyComponent.levelAttackSpeed = bodyInfo.attackSpeedGrowth;
            bodyComponent.levelCrit = bodyInfo.critGrowth;

            bodyComponent.levelMaxHealth = bodyInfo.healthGrowth;
            bodyComponent.levelRegen = bodyInfo.regenGrowth;
            bodyComponent.baseArmor = bodyInfo.armor;
            bodyComponent.levelMaxShield = bodyInfo.shieldGrowth;

            bodyComponent.levelMoveSpeed = bodyInfo.moveSpeedGrowth;
            bodyComponent.levelJumpPower = bodyInfo.jumpPowerGrowth;

            //other
            bodyComponent.baseAcceleration = bodyInfo.acceleration;

            bodyComponent.baseJumpCount = bodyInfo.jumpCount;

            bodyComponent.sprintingSpeedMultiplier = 1.45f;

            bodyComponent.bodyFlags = CharacterBody.BodyFlags.ImmuneToExecutes;
            bodyComponent.rootMotionInMainState = false;

            bodyComponent.hullClassification = HullClassification.Human;

            bodyComponent.isChampion = false;
            #endregion

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

        public static void CreateGenericDoppelganger(GameObject bodyPrefab, string masterName, string masterToCopy)
        {
            GameObject newMaster = PrefabAPI.InstantiateClone(RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterMasters/" + masterToCopy + "MonsterMaster"), masterName, true);
            newMaster.GetComponent<CharacterMaster>().bodyPrefab = bodyPrefab;

            Modules.Content.AddMasterPrefab(newMaster);
        }

        #region ModelSetup

        private static Transform AddCharacterModelToSurvivorBody(GameObject bodyPrefab, Transform modelTransform, BodyInfo bodyInfo) 
        {
            for (int i = bodyPrefab.transform.childCount - 1; i >= 0; i--) {

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
        public static CharacterModel SetupCharacterModel(GameObject prefab, CustomRendererInfo[] customInfos) {

            CharacterModel characterModel = prefab.GetComponent<ModelLocator>().modelTransform.gameObject.GetComponent<CharacterModel>();
            bool preattached = characterModel != null;
            if (!preattached)
                characterModel = prefab.GetComponent<ModelLocator>().modelTransform.gameObject.AddComponent<CharacterModel>();

            characterModel.body = prefab.GetComponent<CharacterBody>();

            characterModel.autoPopulateLightInfos = true;
            characterModel.invisibilityCount = 0;
            characterModel.temporaryOverlays = new List<TemporaryOverlay>();

            if (!preattached) {
                SetupCustomRendererInfos(characterModel, customInfos);
            }
            else {
                SetupPreAttachedRendererInfos(characterModel);
            }
            return characterModel;
        }

        public static void SetupPreAttachedRendererInfos(CharacterModel characterModel) {
            for (int i = 0; i < characterModel.baseRendererInfos.Length; i++) {
                if (characterModel.baseRendererInfos[i].defaultMaterial == null)
                    characterModel.baseRendererInfos[i].defaultMaterial = characterModel.baseRendererInfos[i].renderer.sharedMaterial;
                characterModel.baseRendererInfos[i].defaultMaterial.SetHopooMaterial();
            }
        }

        public static void SetupCustomRendererInfos(CharacterModel characterModel, CustomRendererInfo[] customInfos) {

            ChildLocator childLocator = characterModel.GetComponent<ChildLocator>();
            if (!childLocator) {
                Log.Error("Failed CharacterModel setup: ChildLocator component does not exist on the model");
                return;
            }

            List<CharacterModel.RendererInfo> rendererInfos = new List<CharacterModel.RendererInfo>();

            for (int i = 0; i < customInfos.Length; i++) {
                if (!childLocator.FindChild(customInfos[i].childName)) {
                    Log.Error("Trying to add a RendererInfo for a renderer that does not exist: " + customInfos[i].childName);
                } else {
                    Renderer rend = childLocator.FindChild(customInfos[i].childName).GetComponent<Renderer>();
                    if (rend) {

                        Material mat = customInfos[i].material;

                        if (mat == null) {
                            if (customInfos[i].dontHotpoo) {
                                mat = rend.material;
                            } else {
                                mat = rend.material.SetHopooMaterial();
                            }
                        }

                        rendererInfos.Add(new CharacterModel.RendererInfo {
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
            //cameraTargetParams.aimMode = CameraTargetParams.AimType.Standard;
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

        private static void SetupCapsuleCollider(GameObject prefab) {
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

        public static void SetupHurtBoxes(GameObject bodyPrefab) {

            HealthComponent healthComponent = bodyPrefab.GetComponent<HealthComponent>();

            foreach (HurtBoxGroup hurtboxGroup in bodyPrefab.GetComponentsInChildren<HurtBoxGroup>()) {
                hurtboxGroup.mainHurtBox.healthComponent = healthComponent;
                for (int i = 0; i < hurtboxGroup.hurtBoxes.Length; i++) {
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

        public static void SetupHitbox(GameObject prefab, Transform hitboxTransform, string hitboxName)
        {
            HitBoxGroup hitBoxGroup = prefab.AddComponent<HitBoxGroup>();

            HitBox hitBox = hitboxTransform.gameObject.AddComponent<HitBox>();
            hitboxTransform.gameObject.layer = LayerIndex.projectile.intVal;

            hitBoxGroup.hitBoxes = new HitBox[]
            {
                hitBox
            };

            hitBoxGroup.groupName = hitboxName;
        }

        public static void SetupHitbox(GameObject prefab, string hitboxName, params Transform[] hitboxTransforms)
        {
            HitBoxGroup hitBoxGroup = prefab.AddComponent<HitBoxGroup>();
            List<HitBox> hitBoxes = new List<HitBox>();

            foreach (Transform i in hitboxTransforms)
            {
                HitBox hitBox = i.gameObject.AddComponent<HitBox>();
                i.gameObject.layer = LayerIndex.projectile.intVal;
                hitBoxes.Add(hitBox);
            }

            hitBoxGroup.hitBoxes = hitBoxes.ToArray();

            hitBoxGroup.groupName = hitboxName;
        }

        #endregion ComponentSetup
    }

    // for simplifying rendererinfo creation
    public class CustomRendererInfo
    {
        //the childname according to how it's set upin your childlocator
        public string childName;
        //the material to use. pass in null to use the material in the bundle
        public Material material = null;
        //don't set the hopoo shader on the material, and simply use the material from your prefab, unchanged
        public bool dontHotpoo = false;
        //ignores shields and other overlays. use if you're not using a hopoo shader
        public bool ignoreOverlays = false;
    }
}