using R2API;
using RoR2;
using System.Collections.Generic;
using UnityEngine;
using HenryMod.Modules.Characters;
using RoR2.CharacterAI;
using static RoR2.CharacterAI.AISkillDriver;
using RoR2.Skills;
using System;
using System.Linq;

namespace HenryMod.Modules
{
    // module for creating body prefabs and whatnot
    // recommended to simply avoid touching this unless you REALLY need to
    // some functions are annotated and commented. These ones are useful when you want to learn more what's going on, but when starting out, you don't have to worry about it.
    internal static class Prefabs
    {
        // cache this just to give our ragdolls the same physic material as vanilla stuff
        private static PhysicMaterial ragdollMaterial;

        public static GameObject CreateDisplayPrefab(AssetBundle assetBundle, string displayPrefabName, GameObject prefab)
        {
            GameObject display = assetBundle.LoadAsset<GameObject>(displayPrefabName);
            if (display == null)
            {
                Log.Error($"could not load display prefab {displayPrefabName}. Make sure this prefab exists in assetbundle {assetBundle.name}");
                return null;
            }

            CharacterModel characterModel = display.GetComponent<CharacterModel>();
            if (!characterModel)
            {
                characterModel = display.AddComponent<CharacterModel>();
            }
            characterModel.baseRendererInfos = prefab.GetComponentInChildren<CharacterModel>().baseRendererInfos;

            Modules.Asset.ConvertAllRenderersToHopooShader(display);

            return display;
        }

        #region body setup

        public static GameObject LoadCharacterModel(AssetBundle assetBundle, string modelName)
        {
            GameObject model = assetBundle.LoadAsset<GameObject>(modelName);
            if (model == null)
            {
                Log.Error($"could not load model prefab {modelName}. Make sure this prefab exists in assetbundle {assetBundle.name}");
                return null;
            }
            return model;
        }

        public static GameObject LoadCharacterBody(AssetBundle assetBundle, string bodyName)
        {
            GameObject body = assetBundle.LoadAsset<GameObject>(bodyName);
            if (body == null)
            {
                Log.Error($"could not load body prefab {bodyName}. Make sure this prefab exists in assetbundle {assetBundle.name}");
                return null;
            }
            return body;
        }

        public static GameObject CloneCharacterBody(BodyInfo bodyInfo)
        {
            GameObject clonedBody = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterBodies/" + bodyInfo.bodyNameToClone + "Body");
            if (!clonedBody)
            {
                Log.Error(bodyInfo.bodyNameToClone + " Body to clone is not a valid body, character creation failed");
                return null;
            }

            GameObject newBodyPrefab = PrefabAPI.InstantiateClone(clonedBody, bodyInfo.bodyName);

            for (int i = newBodyPrefab.transform.childCount - 1; i >= 0; i--)
            {
                UnityEngine.Object.DestroyImmediate(newBodyPrefab.transform.GetChild(i).gameObject);
            }

            return newBodyPrefab;
        }

        /// <summary>
        /// clone a body according to your BodyInfo, load your model prefab from the assetbundle, and set up components on both objects through code
        /// </summary>
        public static GameObject CreateBodyPrefab(AssetBundle assetBundle, string modelPrefabName, BodyInfo bodyInfo)
        {
            return CreateBodyPrefab(LoadCharacterModel(assetBundle, modelPrefabName), bodyInfo);
        }
        /// <summary>
        /// clone a body according to your BodyInfo, pass in your model prefab, and set up components on both objects through code
        /// </summary>
        public static GameObject CreateBodyPrefab(GameObject model, BodyInfo bodyInfo)
        {
            return CreateBodyPrefab(CloneCharacterBody(bodyInfo), model, bodyInfo);
        }
        /// <summary>
        /// Pass in a body prefab, loads your model from the assetbundle, and set up components on both objects through code
        /// </summary>
        public static GameObject CreateBodyPrefab(GameObject newBodyPrefab, AssetBundle assetBundle, string modelName, BodyInfo bodyInfo)
        {
            return CreateBodyPrefab(newBodyPrefab, LoadCharacterModel(assetBundle, modelName), bodyInfo);
        }
        /// <summary>
        /// loads your body from the assetbundle, loads your model from the assetbundle, and set up components on both objects through code
        /// </summary>
        public static GameObject CreateBodyPrefab(AssetBundle assetBundle, string bodyPrefabName, string modelPrefabName, BodyInfo bodyInfo)
        {
            return CreateBodyPrefab(LoadCharacterBody(assetBundle, bodyPrefabName), LoadCharacterModel(assetBundle, modelPrefabName), bodyInfo);
        }
        /// <summary>
        /// Pass in a body prefab, pass in a model prefab, and set up components on both objects through code
        /// </summary>
        public static GameObject CreateBodyPrefab(GameObject newBodyPrefab, GameObject model, BodyInfo bodyInfo)
        {
            if (model == null || newBodyPrefab == null)
            {
                Log.Error($"Character creation failed. Model: {model}, Body: {newBodyPrefab}");
                return null;
            }

            SetupCharacterBody(newBodyPrefab, bodyInfo);

            Transform modelBaseTransform = AddCharacterModelToSurvivorBody(newBodyPrefab, model.transform, bodyInfo);

            SetupModelLocator(newBodyPrefab, modelBaseTransform, model.transform);
            SetupCharacterDirection(newBodyPrefab, modelBaseTransform, model.transform);
            SetupCameraTargetParams(newBodyPrefab, bodyInfo);
            //SetupRigidbody(newPrefab);
            SetupCapsuleCollider(newBodyPrefab);

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

            //there is a standard for survivors that should be followed for how much they gain from level up.
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

        private static Transform AddCharacterModelToSurvivorBody(GameObject bodyPrefab, Transform modelTransform, BodyInfo bodyInfo)
        {
            Transform modelBase = bodyPrefab.transform.Find("ModelBase");
            if (modelBase == null) // if these objects exist, you must have set them as you want them in editor
            {
                modelBase = new GameObject("ModelBase").transform;
                modelBase.parent = bodyPrefab.transform;
                modelBase.localPosition = bodyInfo.modelBasePosition;
                modelBase.localRotation = Quaternion.identity;
            }

            modelTransform.parent = modelBase.transform;
            modelTransform.localPosition = Vector3.zero;
            modelTransform.localRotation = Quaternion.identity;

            Transform cameraPivot = bodyPrefab.transform.Find("CameraPivot");
            if (cameraPivot == null)
            {
                cameraPivot = new GameObject("CameraPivot").transform;
                cameraPivot.parent = bodyPrefab.transform;
                cameraPivot.localPosition = bodyInfo.cameraPivotPosition;
                cameraPivot.localRotation = Quaternion.identity;
            }

            Transform aimOrigin = bodyPrefab.transform.Find("AimOrigin");
            if (aimOrigin == null)
            {
                aimOrigin = new GameObject("AimOrigin").transform;
                aimOrigin.parent = bodyPrefab.transform;
                aimOrigin.localPosition = bodyInfo.aimOriginPosition;
                aimOrigin.localRotation = Quaternion.identity;
            }
            bodyPrefab.GetComponent<CharacterBody>().aimOriginTransform = aimOrigin;

            return modelBase.transform;
        }

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
            //character collider MUST be commando's size!
            CapsuleCollider capsuleCollider = prefab.GetComponent<CapsuleCollider>();
            capsuleCollider.center = new Vector3(0f, 0f, 0f);
            capsuleCollider.radius = 0.5f;
            capsuleCollider.height = 1.82f;
            capsuleCollider.direction = 1;
        }
        #endregion body setup

        #region ModelSetup
        public static CharacterModel SetupCharacterModel(GameObject bodyPrefab, CustomRendererInfo[] customInfos = null)
        {

            CharacterModel characterModel = bodyPrefab.GetComponent<ModelLocator>().modelTransform.gameObject.GetComponent<CharacterModel>();
            bool preattached = characterModel != null;
            if (!preattached)
                characterModel = bodyPrefab.GetComponent<ModelLocator>().modelTransform.gameObject.AddComponent<CharacterModel>();

            characterModel.body = bodyPrefab.GetComponent<CharacterBody>();

            characterModel.autoPopulateLightInfos = true;
            characterModel.invisibilityCount = 0;
            characterModel.temporaryOverlays = new List<TemporaryOverlayInstance>();

            if (!preattached)
            {
                SetupCustomRendererInfos(characterModel, customInfos);
            }
            else
            {
                SetupPreAttachedRendererInfos(characterModel);
            }

            SetupHurtboxGroup(bodyPrefab, characterModel.gameObject);
            SetupAimAnimator(bodyPrefab, characterModel.gameObject);
            SetupFootstepController(characterModel.gameObject);
            SetupRagdoll(characterModel.gameObject);

            return characterModel;
        }

        public static void SetupPreAttachedRendererInfos(CharacterModel characterModel)
        {
            for (int i = 0; i < characterModel.baseRendererInfos.Length; i++)
            {
                if (characterModel.baseRendererInfos[i].defaultMaterial == null)
                {
                    characterModel.baseRendererInfos[i].defaultMaterial = characterModel.baseRendererInfos[i].renderer.sharedMaterial;
                }

                if (characterModel.baseRendererInfos[i].defaultMaterial == null)
                {
                    Log.Error($"no material for rendererinfo of this renderer: {characterModel.baseRendererInfos[i].renderer}");
                }
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
                                mat = rend.sharedMaterial;
                            }
                            else
                            {
                                mat = rend.sharedMaterial.ConvertDefaultShaderToHopoo();
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

        private static void SetupHurtboxGroup(GameObject bodyPrefab, GameObject model) 
        {         
            SetupMainHurtboxesFromChildLocator(bodyPrefab, model);

            SetHurtboxesHealthComponents(bodyPrefab);
        }
        /// <summary>
        /// Sets up the main Hurtbox from a collider assigned to the child locator called "MainHurtbox".
        /// <para>If a "HeadHurtbox" child is also set up, automatically sets that one up and assigns that one as a sniper weakpoint. if not, MainHurtbox is set as a sniper weakpoint.</para>
        /// </summary>
        private static void SetupMainHurtboxesFromChildLocator(GameObject bodyPrefab, GameObject model)
        {
            if (bodyPrefab.GetComponent<HurtBoxGroup>() != null)
            {
                Log.Debug("Hitboxgroup already exists on model prefab. aborting code setup");
                return;
            }

            ChildLocator childLocator = model.GetComponent<ChildLocator>();

            if (string.IsNullOrEmpty(childLocator.FindChildNameInsensitive("MainHurtbox")))
            {
                Log.Error("Could not set up main hurtbox: make sure you have a transform pair in your prefab's ChildLocator called 'MainHurtbox'");
                return;
            }

            HurtBoxGroup hurtBoxGroup = model.AddComponent<HurtBoxGroup>();

            HurtBox headHurtbox = null;
            GameObject headHurtboxObject = childLocator.FindChildGameObjectInsensitive("HeadHurtbox");
            if (headHurtboxObject)
            {
                Log.Debug("HeadHurtboxFound. Setting up");
                headHurtbox = headHurtboxObject.AddComponent<HurtBox>();
                headHurtbox.gameObject.layer = LayerIndex.entityPrecise.intVal;
                headHurtbox.healthComponent = bodyPrefab.GetComponent<HealthComponent>();
                headHurtbox.isBullseye = false;
                headHurtbox.isSniperTarget = true;
                headHurtbox.damageModifier = HurtBox.DamageModifier.Normal;
                headHurtbox.hurtBoxGroup = hurtBoxGroup;
                headHurtbox.indexInGroup = 1;
            }

            HurtBox mainHurtbox = childLocator.FindChildGameObjectInsensitive("MainHurtbox").AddComponent<HurtBox>();
            mainHurtbox.gameObject.layer = LayerIndex.entityPrecise.intVal;
            mainHurtbox.healthComponent = bodyPrefab.GetComponent<HealthComponent>();
            mainHurtbox.isBullseye = true;
            mainHurtbox.isSniperTarget = headHurtbox == null;
            mainHurtbox.damageModifier = HurtBox.DamageModifier.Normal;
            mainHurtbox.hurtBoxGroup = hurtBoxGroup;
            mainHurtbox.indexInGroup = 0;

            if (headHurtbox)
            {
                hurtBoxGroup.hurtBoxes = new HurtBox[]
                {
                    mainHurtbox,
                    headHurtbox
                };
            }
            else
            {
                hurtBoxGroup.hurtBoxes = new HurtBox[]
                {
                    mainHurtbox,
                };
            }
            hurtBoxGroup.mainHurtBox = mainHurtbox;
            hurtBoxGroup.bullseyeCount = 1;
        }

        private static string FindChildNameInsensitive(this ChildLocator childLocator, string child)
        {
            return childLocator.transformPairs.Where((pair) => pair.name.ToLowerInvariant() == child.ToLowerInvariant()).FirstOrDefault().name;
        }
        private static Transform FindChildInsensitive(this ChildLocator childLocator, string child)
        {
            return childLocator.FindChild(childLocator.FindChildNameInsensitive(child));
        }
        private static GameObject FindChildGameObjectInsensitive(this ChildLocator childLocator, string child)
        {
            return childLocator.FindChildGameObject(childLocator.FindChildNameInsensitive(child));
        }

        public static void SetHurtboxesHealthComponents(GameObject bodyPrefab)
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
                        //boneCollider.material = ragdollMaterial;
                        boneCollider.sharedMaterial = ragdollMaterial;
                    }
                    else
                    {
                        Log.Error($"Ragdoll bone {boneTransform.gameObject} doesn't have a collider. Ragdoll will break.");
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
        #endregion

        #region master
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

        public static GameObject LoadMaster(this AssetBundle assetBundle, GameObject bodyPrefab, string assetName)
        {
            GameObject newMaster = assetBundle.LoadAsset<GameObject>(assetName);

            BaseAI baseAI = newMaster.GetComponent<BaseAI>();
            if(baseAI == null)
            {
                baseAI = newMaster.AddComponent<BaseAI>();
                baseAI.aimVectorDampTime = 0.1f;
                baseAI.aimVectorMaxSpeed = 360;
            }
            baseAI.scanState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.AI.Walker.Wander));

            EntityStateMachine stateMachine = newMaster.GetComponent<EntityStateMachine>();
            if(stateMachine == null)
            {
                AddEntityStateMachine(newMaster, "AI", typeof(EntityStates.AI.Walker.Wander), typeof(EntityStates.AI.Walker.Wander));
            }

            baseAI.stateMachine = stateMachine;

            CharacterMaster characterMaster = newMaster.GetComponent<CharacterMaster>();
            if(characterMaster == null)
            {
                characterMaster = newMaster.AddComponent<CharacterMaster>();
            }
            characterMaster.bodyPrefab = bodyPrefab;
            characterMaster.teamIndex = TeamIndex.Monster;

            Modules.Content.AddMasterPrefab(newMaster);
            return newMaster;
        }
        #endregion master

        /// <summary>
        /// More than remove the EntityStateMachine components, it also clears fields from NetworkStateMachine, CharacterDeathBehavior, and SetStateOnHurt
        /// <para>See AddEntityStateMachine and AddMainEntityStateMachine for more info</para>
        /// </summary>
        /// <param name="bodyPrefab"></param>
        public static void ClearEntityStateMachines(GameObject bodyPrefab)
        {
            EntityStateMachine[] machines = bodyPrefab.GetComponents<EntityStateMachine>();

            for (int i = machines.Length - 1; i >= 0; i--)
            {
                UnityEngine.Object.DestroyImmediate(machines[i]);
            }

            NetworkStateMachine networkMachine = bodyPrefab.GetComponent<NetworkStateMachine>();
            networkMachine.stateMachines = Array.Empty<EntityStateMachine>();

            CharacterDeathBehavior deathBehavior = bodyPrefab.GetComponent<CharacterDeathBehavior>();
            if (deathBehavior)
            {
                deathBehavior.idleStateMachine = Array.Empty<EntityStateMachine>();
            }

            SetStateOnHurt setStateOnHurt = bodyPrefab.GetComponent<SetStateOnHurt>();
            if (setStateOnHurt)
            {
                setStateOnHurt.idleStateMachine = Array.Empty<EntityStateMachine>();
            }
        }

        //this but in reverse https://media.discordapp.net/attachments/875473107891150878/896193331720237106/caption-7.gif?ex=65989f94&is=65862a94&hm=e1f51da3ad190c00c5da1f90269d5ef10bedb0ae063c0f20aa0dd8721608018a&
        /// <summary>
        /// Creates an EntityStateMachine, and adds it to the NetworkStateMachine, CharacterDeathBehavior, and SetStateOnHurt components. 
        /// <para>See AddMainEntityStateMachine for typically your "Body" state machine.</para>
        /// </summary>
        public static EntityStateMachine AddEntityStateMachine(GameObject prefab, string machineName, Type mainStateType = null, Type initalStateType = null, bool addToHurt = true, bool addToDeath = true)
        {
            EntityStateMachine entityStateMachine = EntityStateMachine.FindByCustomName(prefab, machineName);
            if (entityStateMachine == null)
            {
                entityStateMachine = prefab.AddComponent<EntityStateMachine>();
            }
            else
            {
                Log.Message($"An Entity State Machine already exists with the name {machineName}. replacing.");
            }
            //Set up entitystatemachine
            entityStateMachine.customName = machineName;

            if (mainStateType == null)
            {
                mainStateType = typeof(EntityStates.Idle);
            }
            entityStateMachine.mainStateType = new EntityStates.SerializableEntityStateType(mainStateType);

            if (initalStateType == null)
            {
                initalStateType = typeof(EntityStates.Idle);
            }
            entityStateMachine.initialStateType = new EntityStates.SerializableEntityStateType(initalStateType);

            //Add to NetworkStateMachine so it is networked, as it sounds
            NetworkStateMachine networkMachine = prefab.GetComponent<NetworkStateMachine>();
            if (networkMachine)
            {
                networkMachine.stateMachines = networkMachine.stateMachines.Append(entityStateMachine).ToArray();
            }

            //Add to the array of "idle" StateMachines. For when the character dies.
            //This component sets that state machine to idle, stopping what it was doing
            CharacterDeathBehavior deathBehavior = prefab.GetComponent<CharacterDeathBehavior>();
            if (deathBehavior && addToDeath)
            {
                deathBehavior.idleStateMachine = deathBehavior.idleStateMachine.Append(entityStateMachine).ToArray();
            }

            //Add to the array of "idle" StateMachines.
            //Same as CharacterDeathBehavior but for stunning/freezing/etc
            SetStateOnHurt setStateOnHurt = prefab.GetComponent<SetStateOnHurt>();
            if (setStateOnHurt && addToHurt)
            {
                setStateOnHurt.idleStateMachine = setStateOnHurt.idleStateMachine.Append(entityStateMachine).ToArray();
            }

            return entityStateMachine;
        }

        /// <summary>
        /// Creates an EntityStateMachine, and adds it to the NetworkStateMachine, CharacterDeathBehavior, and SetStateOnHurt components.
        /// <para>Similar to AddEntityStateMachine, however when adding to these components, what we'll consider the "main state machine" (typically the "Body" state machine) has to be set in certain fields.</para>
        /// </summary>
        public static EntityStateMachine AddMainEntityStateMachine(GameObject bodyPrefab, string machineName = "Body", Type mainStateType = null, Type initalStateType = null)
        {
            EntityStateMachine entityStateMachine = EntityStateMachine.FindByCustomName(bodyPrefab, machineName);
            if (entityStateMachine == null)
            {
                entityStateMachine = bodyPrefab.AddComponent<EntityStateMachine>();
            }
            else
            {
                Log.Message($"An Entity State Machine already exists with the name {machineName}. replacing.");
            }

            //Create entitystatemachine
            entityStateMachine.customName = machineName;

            if (mainStateType == null)
            {
                mainStateType = typeof(EntityStates.GenericCharacterMain);
            }
            entityStateMachine.mainStateType = new EntityStates.SerializableEntityStateType(mainStateType);

            if (initalStateType == null)
            {
                initalStateType = typeof(EntityStates.SpawnTeleporterState);
            }
            entityStateMachine.initialStateType = new EntityStates.SerializableEntityStateType(initalStateType);

            //Add to NetworkStateMachine so it is networked, as it sounds
            NetworkStateMachine networkMachine = bodyPrefab.GetComponent<NetworkStateMachine>();
            if (networkMachine)
            {
                networkMachine.stateMachines = networkMachine.stateMachines.Append(entityStateMachine).ToArray();
            }

            //Add to the main state machine field of CharacterDeathBehavior for when the character dies.
            //This EntityStateMachine will enter the death state, while other state machines are set to idle
            //The death state is set elsewhere, (likely in the commando clone). It is typically GenericCharacterDeath, but you can set it to whatever you want.
            CharacterDeathBehavior deathBehavior = bodyPrefab.GetComponent<CharacterDeathBehavior>();
            if (deathBehavior)
            {
                deathBehavior.deathStateMachine = entityStateMachine;
            }

            //Add to the main state machine field of SetStateOnHurt for when the character is Stunned/Frozen/etc,
            //This EntityStateMachine will enter the relative state, while other state machines are set to idle.
            SetStateOnHurt setStateOnHurt = bodyPrefab.GetComponent<SetStateOnHurt>();
            if (setStateOnHurt)
            {
                setStateOnHurt.targetStateMachine = entityStateMachine;
            }

            return entityStateMachine;
        }

        /// <summary>
        /// Sets up a hitboxgroup with passed in child transforms as hitboxes
        /// </summary>
        /// <param name="hitBoxGroupName">name that is used by melee or other overlapattacks</param>
        /// <param name="hitboxChildNames">childname of the transform set up in editor</param>
        public static void SetupHitBoxGroup(GameObject modelPrefab, string hitBoxGroupName, params string[] hitboxChildNames)
        {
            ChildLocator childLocator = modelPrefab.GetComponent<ChildLocator>();

            Transform[] hitboxTransforms = new Transform[hitboxChildNames.Length];
            for (int i = 0; i < hitboxChildNames.Length; i++)
            {
                hitboxTransforms[i] = childLocator.FindChild(hitboxChildNames[i]);

                if (hitboxTransforms[i] == null)
                {
                    Log.Error("missing hitbox for " + hitboxChildNames[i]);
                }
            }
            SetupHitBoxGroup(modelPrefab, hitBoxGroupName, hitboxTransforms);
        }
        /// <summary>
        /// Sets up a hitboxgroup with passed in transforms as hitboxes
        /// </summary>
        /// <param name="hitBoxGroupName">name that is used by melee or other overlapattacks</param>
        /// <param name="hitBoxTransforms">the transforms to be used in this hitboxgroup</param>
        public static void SetupHitBoxGroup(GameObject prefab, string hitBoxGroupName, params Transform[] hitBoxTransforms)
        {
            List<HitBox> hitBoxes = new List<HitBox>();

            foreach (Transform i in hitBoxTransforms)
            {
                if (i == null)
                {
                    Log.Error($"Error setting up hitboxGroup for {hitBoxGroupName}: hitbox transform was null");
                    continue;
                }
                HitBox hitBox = i.gameObject.AddComponent<HitBox>();
                i.gameObject.layer = LayerIndex.projectile.intVal;
                hitBoxes.Add(hitBox);
            }

            if(hitBoxes.Count == 0)
            {
                Log.Error($"No hitboxes were set up. aborting setting up hitboxGroup for {hitBoxGroupName}");
                return;
            }

            HitBoxGroup hitBoxGroup = prefab.AddComponent<HitBoxGroup>();

            hitBoxGroup.hitBoxes = hitBoxes.ToArray();

            hitBoxGroup.groupName = hitBoxGroupName;
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