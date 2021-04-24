using R2API;
using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace HenryMod.Modules
{
    // module for creating body prefabs and whatnot
    // recommended to simply avoid touching this unless you REALLY need to

    internal static class Prefabs
    {
        // cache this just to give our ragdolls the same physic material as vanilla stuff
        private static PhysicMaterial ragdollMaterial;

        internal static List<SurvivorDef> survivorDefinitions = new List<SurvivorDef>();
        internal static List<GameObject> bodyPrefabs = new List<GameObject>();
        internal static List<GameObject> masterPrefabs = new List<GameObject>();
        internal static List<GameObject> projectilePrefabs = new List<GameObject>();

        internal static void RegisterNewSurvivor(GameObject bodyPrefab, GameObject displayPrefab, Color charColor, string namePrefix, UnlockableDef unlockableDef, float sortPosition)
        {
            string fullNameString = HenryPlugin.developerPrefix + "_" + namePrefix + "_BODY_NAME";
            string fullDescString = HenryPlugin.developerPrefix + "_" + namePrefix + "_BODY_DESCRIPTION";
            string fullOutroString = HenryPlugin.developerPrefix + "_" + namePrefix + "_BODY_OUTRO_FLAVOR";
            string fullFailureString = HenryPlugin.developerPrefix + "_" + namePrefix + "_BODY_OUTRO_FAILURE";

            SurvivorDef survivorDef = ScriptableObject.CreateInstance<SurvivorDef>();
            survivorDef.bodyPrefab = bodyPrefab;
            survivorDef.displayPrefab = displayPrefab;
            survivorDef.primaryColor = charColor;
            survivorDef.displayNameToken = fullNameString;
            survivorDef.descriptionToken = fullDescString;
            survivorDef.outroFlavorToken = fullOutroString;
            survivorDef.mainEndingEscapeFailureFlavorToken = fullFailureString;
            survivorDef.desiredSortPosition = sortPosition;
            survivorDef.unlockableDef = unlockableDef;

            survivorDefinitions.Add(survivorDef);
        }

        internal static void RegisterNewSurvivor(GameObject bodyPrefab, GameObject displayPrefab, Color charColor, string namePrefix) { RegisterNewSurvivor(bodyPrefab, displayPrefab, charColor, namePrefix, null, 100f); }

        internal static void RegisterNewSurvivor(GameObject bodyPrefab, GameObject displayPrefab, Color charColor, string namePrefix, float sortPosition) { RegisterNewSurvivor(bodyPrefab, displayPrefab, charColor, namePrefix, null, sortPosition); }

        internal static void RegisterNewSurvivor(GameObject bodyPrefab, GameObject displayPrefab, Color charColor, string namePrefix, UnlockableDef unlockableDef) { RegisterNewSurvivor(bodyPrefab, displayPrefab, charColor, namePrefix, unlockableDef, 100f); }

        internal static GameObject CreateDisplayPrefab(string modelName, GameObject prefab, BodyInfo bodyInfo)
        {
            if (!Resources.Load<GameObject>("Prefabs/CharacterBodies/" + bodyInfo.bodyNameToClone + "Body"))
            {
                Debug.LogError(bodyInfo.bodyNameToClone + "Body is not a valid body, character creation failed");
                return null;
            }

            GameObject newPrefab = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/CharacterBodies/" + bodyInfo.bodyNameToClone + "Body"), modelName + "Prefab");

            GameObject model = CreateModel(newPrefab, modelName);
            Transform modelBaseTransform = SetupModel(newPrefab, model.transform, bodyInfo);

            model.AddComponent<CharacterModel>().baseRendererInfos = prefab.GetComponentInChildren<CharacterModel>().baseRendererInfos;

            Modules.Assets.ConvertAllRenderersToHopooShader(model);

            return model.gameObject;
        }

        internal static GameObject CreatePrefab(string bodyName, string modelName, BodyInfo bodyInfo)
        {
            if (!Resources.Load<GameObject>("Prefabs/CharacterBodies/" + bodyInfo.bodyNameToClone + "Body"))
            {
                Debug.LogError(bodyInfo.bodyNameToClone + "Body is not a valid body, character creation failed");
                return null;
            }

            GameObject newPrefab = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/CharacterBodies/" + bodyInfo.bodyNameToClone + "Body"), bodyName);

            Transform modelBaseTransform = null;
            GameObject model = null;
            if (modelName != "mdl")
            {
                model = CreateModel(newPrefab, modelName);
                if (model == null) model = newPrefab.GetComponentInChildren<CharacterModel>().gameObject;
                modelBaseTransform = SetupModel(newPrefab, model.transform, bodyInfo);
            }

            #region CharacterBody
            CharacterBody bodyComponent = newPrefab.GetComponent<CharacterBody>();

            bodyComponent.name = bodyInfo.bodyName;
            bodyComponent.baseNameToken = bodyInfo.bodyNameToken;
            bodyComponent.subtitleNameToken = bodyInfo.subtitleNameToken;
            bodyComponent.portraitIcon = bodyInfo.characterPortrait;
            bodyComponent.crosshairPrefab = bodyInfo.crosshair;

            bodyComponent.bodyFlags = CharacterBody.BodyFlags.ImmuneToExecutes;
            bodyComponent.rootMotionInMainState = false;

            bodyComponent.baseMaxHealth = bodyInfo.maxHealth;
            bodyComponent.levelMaxHealth = bodyInfo.healthGrowth;

            bodyComponent.baseRegen = bodyInfo.healthRegen;
            bodyComponent.levelRegen = bodyComponent.baseRegen * 0.2f;

            bodyComponent.baseMaxShield = bodyInfo.shield;
            bodyComponent.levelMaxShield = bodyInfo.shieldGrowth;

            bodyComponent.baseMoveSpeed = bodyInfo.moveSpeed;
            bodyComponent.levelMoveSpeed = bodyInfo.moveSpeedGrowth;

            bodyComponent.baseAcceleration = bodyInfo.acceleration;

            bodyComponent.baseJumpPower = bodyInfo.jumpPower;
            bodyComponent.levelJumpPower = bodyInfo.jumpPowerGrowth;

            bodyComponent.baseDamage = bodyInfo.damage;
            bodyComponent.levelDamage = bodyComponent.baseDamage * 0.2f;

            bodyComponent.baseAttackSpeed = bodyInfo.attackSpeed;
            bodyComponent.levelAttackSpeed = bodyInfo.attackSpeedGrowth;

            bodyComponent.baseArmor = bodyInfo.armor;
            bodyComponent.levelArmor = bodyInfo.armorGrowth;

            bodyComponent.baseCrit = bodyInfo.crit;
            bodyComponent.levelCrit = bodyInfo.critGrowth;

            bodyComponent.baseJumpCount = bodyInfo.jumpCount;

            bodyComponent.sprintingSpeedMultiplier = 1.45f;

            bodyComponent.hideCrosshair = false;
            bodyComponent.aimOriginTransform = modelBaseTransform.Find("AimOrigin");
            bodyComponent.hullClassification = HullClassification.Human;

            bodyComponent.preferredPodPrefab = bodyInfo.podPrefab;

            bodyComponent.isChampion = false;

            bodyComponent.bodyColor = bodyInfo.bodyColor;
            #endregion

            if (modelBaseTransform != null) SetupCharacterDirection(newPrefab, modelBaseTransform, model.transform);
            SetupCameraTargetParams(newPrefab);
            if (modelBaseTransform != null) SetupModelLocator(newPrefab, modelBaseTransform, model.transform);
            SetupRigidbody(newPrefab);
            SetupCapsuleCollider(newPrefab);
            SetupMainHurtbox(newPrefab, model);
            SetupFootstepController(model);
            SetupRagdoll(model);
            SetupAimAnimator(newPrefab, model);

            bodyPrefabs.Add(newPrefab);

            return newPrefab;
        }

        internal static void CreateGenericDoppelganger(GameObject bodyPrefab, string masterName, string masterToCopy)
        {
            GameObject newMaster = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/CharacterMasters/" + masterToCopy + "MonsterMaster"), masterName, true);
            newMaster.GetComponent<CharacterMaster>().bodyPrefab = bodyPrefab;

            masterPrefabs.Add(newMaster);
        }

        #region ModelSetup
        private static Transform SetupModel(GameObject prefab, Transform modelTransform, BodyInfo bodyInfo)
        {
            GameObject modelBase = new GameObject("ModelBase");
            modelBase.transform.parent = prefab.transform;
            modelBase.transform.localPosition = bodyInfo.modelBasePosition;
            modelBase.transform.localRotation = Quaternion.identity;
            modelBase.transform.localScale = new Vector3(1f, 1f, 1f);

            GameObject cameraPivot = new GameObject("CameraPivot");
            cameraPivot.transform.parent = modelBase.transform;
            cameraPivot.transform.localPosition = bodyInfo.cameraPivotPosition;
            cameraPivot.transform.localRotation = Quaternion.identity;
            cameraPivot.transform.localScale = Vector3.one;

            GameObject aimOrigin = new GameObject("AimOrigin");
            aimOrigin.transform.parent = modelBase.transform;
            aimOrigin.transform.localPosition = bodyInfo.aimOriginPosition;
            aimOrigin.transform.localRotation = Quaternion.identity;
            aimOrigin.transform.localScale = Vector3.one;
            prefab.GetComponent<CharacterBody>().aimOriginTransform = aimOrigin.transform;

            modelTransform.parent = modelBase.transform;
            modelTransform.localPosition = Vector3.zero;
            modelTransform.localRotation = Quaternion.identity;

            return modelBase.transform;
        }

        private static GameObject CreateModel(GameObject main, string modelName)
        {
            HenryPlugin.DestroyImmediate(main.transform.Find("ModelBase").gameObject);
            HenryPlugin.DestroyImmediate(main.transform.Find("CameraPivot").gameObject);
            HenryPlugin.DestroyImmediate(main.transform.Find("AimOrigin").gameObject);

            if (Modules.Assets.mainAssetBundle.LoadAsset<GameObject>(modelName) == null)
            {
                Debug.LogError("Trying to load a null model- check to see if the name in your code matches the name of the object in Unity");
                return null;
            }

            return GameObject.Instantiate(Modules.Assets.mainAssetBundle.LoadAsset<GameObject>(modelName));
        }

        internal static void SetupCharacterModel(GameObject prefab, CustomRendererInfo[] rendererInfo, int mainRendererIndex)
        {
            CharacterModel characterModel = prefab.GetComponent<ModelLocator>().modelTransform.gameObject.AddComponent<CharacterModel>();
            ChildLocator childLocator = characterModel.GetComponent<ChildLocator>();
            characterModel.body = prefab.GetComponent<CharacterBody>();

            if (!childLocator)
            {
                Debug.LogError("Failed CharacterModel setup: ChildLocator component does not exist on the model");
                return;
            }

            List<CharacterModel.RendererInfo> rendererInfos = new List<CharacterModel.RendererInfo>();

            for (int i = 0; i < rendererInfo.Length; i++)
            {
                if (!childLocator.FindChild(rendererInfo[i].childName))
                {
                    Debug.LogError("Trying to add a RendererInfo for a renderer that does not exist: " + rendererInfo[i].childName);
                }
                else
                {
                    Renderer j = childLocator.FindChild(rendererInfo[i].childName).GetComponent<Renderer>();
                    if (!j)
                    {

                    }
                    else
                    {
                        rendererInfos.Add(new CharacterModel.RendererInfo
                        {
                            renderer = childLocator.FindChild(rendererInfo[i].childName).GetComponent<Renderer>(),
                            defaultMaterial = rendererInfo[i].material,
                            ignoreOverlays = rendererInfo[i].ignoreOverlays,
                            defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On
                        });
                    }
                }
            }

            characterModel.baseRendererInfos = rendererInfos.ToArray();

            characterModel.autoPopulateLightInfos = true;
            characterModel.invisibilityCount = 0;
            characterModel.temporaryOverlays = new List<TemporaryOverlay>();

            if (mainRendererIndex > characterModel.baseRendererInfos.Length)
            {
                Debug.LogError("mainRendererIndex out of range: not setting mainSkinnedMeshRenderer for " + prefab.name);
                return;
            }

            characterModel.mainSkinnedMeshRenderer = characterModel.baseRendererInfos[mainRendererIndex].renderer.GetComponent<SkinnedMeshRenderer>();
        }
        #endregion

        #region ComponentSetup
        private static void SetupCharacterDirection(GameObject prefab, Transform modelBaseTransform, Transform modelTransform)
        {
            CharacterDirection characterDirection = prefab.GetComponent<CharacterDirection>();
            characterDirection.targetTransform = modelBaseTransform;
            characterDirection.overrideAnimatorForwardTransform = null;
            characterDirection.rootMotionAccumulator = null;
            characterDirection.modelAnimator = modelTransform.GetComponent<Animator>();
            characterDirection.driveFromRootRotation = false;
            characterDirection.turnSpeed = 720f;
        }

        private static void SetupCameraTargetParams(GameObject prefab)
        {
            CameraTargetParams cameraTargetParams = prefab.GetComponent<CameraTargetParams>();
            cameraTargetParams.cameraParams = Resources.Load<GameObject>("Prefabs/CharacterBodies/MercBody").GetComponent<CameraTargetParams>().cameraParams;
            cameraTargetParams.cameraPivotTransform = prefab.transform.Find("ModelBase").Find("CameraPivot");
            cameraTargetParams.aimMode = CameraTargetParams.AimType.Standard;
            cameraTargetParams.recoil = Vector2.zero;
            cameraTargetParams.idealLocalCameraPos = Vector3.zero;
            cameraTargetParams.dontRaycastToPivot = false;
        }

        private static void SetupModelLocator(GameObject prefab, Transform modelBaseTransform, Transform modelTransform)
        {
            ModelLocator modelLocator = prefab.GetComponent<ModelLocator>();
            modelLocator.modelTransform = modelTransform;
            modelLocator.modelBaseTransform = modelBaseTransform;
        }

        private static void SetupRigidbody(GameObject prefab)
        {
            Rigidbody rigidbody = prefab.GetComponent<Rigidbody>();
            rigidbody.mass = 100f;
        }

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
                Debug.LogError("Could not set up main hurtbox: make sure you have a transform pair in your prefab's ChildLocator component called 'MainHurtbox'");
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

        private static void SetupFootstepController(GameObject model)
        {
            FootstepHandler footstepHandler = model.AddComponent<FootstepHandler>();
            footstepHandler.baseFootstepString = "Play_player_footstep";
            footstepHandler.sprintFootstepOverrideString = "";
            footstepHandler.enableFootstepDust = true;
            footstepHandler.footstepDustPrefab = Resources.Load<GameObject>("Prefabs/GenericFootstepDust");
        }

        private static void SetupRagdoll(GameObject model)
        {
            RagdollController ragdollController = model.GetComponent<RagdollController>();

            if (!ragdollController) return;

            if (ragdollMaterial == null) ragdollMaterial = Resources.Load<GameObject>("Prefabs/CharacterBodies/CommandoBody").GetComponentInChildren<RagdollController>().bones[1].GetComponent<Collider>().material;

            foreach (Transform i in ragdollController.bones)
            {
                if (i)
                {
                    i.gameObject.layer = LayerIndex.ragdoll.intVal;
                    Collider j = i.GetComponent<Collider>();
                    if (j)
                    {
                        j.material = ragdollMaterial;
                        j.sharedMaterial = ragdollMaterial;
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

        internal static void SetupHitbox(GameObject prefab, Transform hitboxTransform, string hitboxName)
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

        internal static void SetupHitbox(GameObject prefab, string hitboxName, params Transform[] hitboxTransforms)
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
        #endregion
    }
}

// for simplifying characterbody creation
internal class BodyInfo
{
    internal string bodyName = "";
    internal string bodyNameToken = "";
    internal string subtitleNameToken = "";

    internal string bodyNameToClone = "Commando";// body prefab you're cloning for your character- commando is the safest

    internal Texture characterPortrait = null;

    internal GameObject crosshair = null;
    internal GameObject podPrefab = null;

    internal float maxHealth = 100f;
    internal float healthGrowth = 2f;

    internal float healthRegen = 0f;

    internal float shield = 0f;// base shield is a thing apparently. neat
    internal float shieldGrowth = 0f;

    internal float moveSpeed = 7f;
    internal float moveSpeedGrowth = 0f;

    internal float acceleration = 80f;

    internal float jumpPower = 15f;
    internal float jumpPowerGrowth = 0f;// jump power per level exists for some reason

    internal float damage = 12f;

    internal float attackSpeed = 1f;
    internal float attackSpeedGrowth = 0f;

    internal float armor = 0f;
    internal float armorGrowth = 0f;

    internal float crit = 1f;
    internal float critGrowth = 0f;

    internal int jumpCount = 1;

    internal Color bodyColor = Color.grey;

    internal Vector3 aimOriginPosition = new Vector3(0f, 1.8f, 0f);
    internal Vector3 modelBasePosition = new Vector3(0f, -0.92f, 0f);
    internal Vector3 cameraPivotPosition = new Vector3(0f, 1.6f, 0f);
}

// for simplifying rendererinfo creation
internal class CustomRendererInfo
{
    internal string childName;
    internal Material material;
    internal bool ignoreOverlays;
}