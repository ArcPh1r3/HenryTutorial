using RoR2;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace HenryMod.Modules.Characters
{
    internal abstract class CharacterBase
    {
        public static CharacterBase instance;

        public abstract string bodyName { get; }

        public abstract BodyInfo bodyInfo { get; set; }

        public abstract CustomRendererInfo[] customRendererInfos { get; set; }

        public abstract Type characterMainState { get; }
        public virtual Type characterSpawnState { get; }

        public abstract ItemDisplaysBase itemDisplays { get; }

        public virtual GameObject bodyPrefab { get; set; }
        public virtual CharacterModel characterBodyModel { get; set; }
        public string fullBodyName => bodyName + "Body";

        public virtual void Initialize()
        {
            instance = this;
            InitializeCharacter();
        }

        public virtual void InitializeCharacter()
        {
            InitializeCharacterBodyAndModel();
            InitializeCharacterMaster();

            InitializeEntityStateMachine();
            InitializeSkills();

            InitializeHitboxes();
            InitializeHurtboxes();

            InitializeSkins();
            InitializeItemDisplays();

            InitializeDoppelganger("Merc");
        }

        protected virtual void InitializeCharacterBodyAndModel()
        {
            bodyPrefab = Modules.Prefabs.CreateBodyPrefab(bodyName + "Body", "mdl" + bodyName, bodyInfo);
            InitializeCharacterModel();
        }
        protected virtual void InitializeCharacterModel()
        {
            characterBodyModel = Modules.Prefabs.SetupCharacterModel(bodyPrefab, customRendererInfos);
        }

        protected virtual void InitializeCharacterMaster() { }
        protected virtual void InitializeEntityStateMachine()
        {
            bodyPrefab.GetComponent<EntityStateMachine>().mainStateType = new EntityStates.SerializableEntityStateType(characterMainState);
            Modules.Content.AddEntityState(characterMainState);
            if (characterSpawnState != null)
            {
                bodyPrefab.GetComponent<EntityStateMachine>().initialStateType = new EntityStates.SerializableEntityStateType(characterSpawnState);
                Modules.Content.AddEntityState(characterSpawnState);
            }
        }

        public abstract void InitializeSkills();

        public virtual void InitializeHitboxes() { }

        public virtual void InitializeHurtboxes()
        {
            Modules.Prefabs.SetupHurtBoxes(bodyPrefab);
        }

        public virtual void InitializeSkins() { }

        public virtual void InitializeDoppelganger(string clone)
        {
            Modules.Prefabs.CreateGenericDoppelganger(instance.bodyPrefab, bodyName + "MonsterMaster", clone);
        }

        public virtual void InitializeItemDisplays()
        {
            ItemDisplayRuleSet itemDisplayRuleSet = ScriptableObject.CreateInstance<ItemDisplayRuleSet>();
            itemDisplayRuleSet.name = "idrs" + bodyName;

            characterBodyModel.itemDisplayRuleSet = itemDisplayRuleSet;

            if (itemDisplays != null)
            {
                RoR2.RoR2Application.onLoad += SetItemDisplays;
            }
        }

        public void SetItemDisplays()
        {
            itemDisplays.SetItemDIsplays(characterBodyModel.itemDisplayRuleSet);
        }

    }

    // for simplifying characterbody creation
    internal class BodyInfo
    {
        public string bodyName = "";
        public string bodyNameToken = "";
        public string subtitleNameToken = "";
        /// <summary>
        /// body prefab you're cloning for your character- commando is the safest
        /// </summary>
        public string bodyNameToClone = "Commando";

        /// <summary>
        /// the color of your characters name and skills and such in the lobby
        /// </summary>
        public Color bodyColor = Color.white;

        public Texture characterPortrait = null;

        public float sortPosition = 69f;

        public GameObject crosshair = null;
        public GameObject podPrefab = null;

        //stats
        public float maxHealth = 100f;
        public float healthRegen = 1f;
        public float armor = 0f;
        /// <summary>
        /// base shield is a thing apparently. neat
        /// </summary>
        public float shield = 0f;

        public float damage = 12f;
        public float attackSpeed = 1f;
        public float crit = 1f;

        public float moveSpeed = 7f;
        public float jumpPower = 15f;

        //misc stats
        public float acceleration = 80f;
        public int jumpCount = 1;

        //stat growth
        /// <summary>
        /// When this is true, you don't need to worry about setting any of the stat growth values. 
        /// </summary>
        public bool autoCalculateLevelStats = true;

        public float healthGrowth = 30f;
        public float regenGrowth = 0.2f;
        public float shieldGrowth = 0f;
        public float armorGrowth = 0f;

        public float damageGrowth = 2.4f;
        public float attackSpeedGrowth = 0f;
        public float critGrowth = 0f;

        public float moveSpeedGrowth = 0f;
        public float jumpPowerGrowth = 0f;// jump power per level exists for some reason


        //camera stuff
        public Vector3 modelBasePosition = new Vector3(0f, -0.92f, 0f);
        public Vector3 cameraPivotPosition = new Vector3(0f, 1.6f, 0f);
        public Vector3 aimOriginPosition = new Vector3(0f, 2f, 0f);

        public float cameraParamsVerticalOffset = 1.5f;
        public float cameraParamsDepth = -12;

        private CharacterCameraParams _cameraParams;
        /// <summary>
        /// taken care of by the fields, cameraParamsVerticalOffset, and cameraParamsDepth. You can override this to create a new CharacterCameraParams for this field, in which case those two fields will be ignored
        /// </summary>
        public CharacterCameraParams cameraParams
        {
            get
            {
                if (_cameraParams == null)
                {
                    _cameraParams = ScriptableObject.CreateInstance<CharacterCameraParams>();
                    _cameraParams.data.minPitch = -70;
                    _cameraParams.data.maxPitch = 70;
                    _cameraParams.data.wallCushion = 0.1f;
                    _cameraParams.data.pivotVerticalOffset = cameraParamsVerticalOffset;
                    _cameraParams.data.idealLocalCameraPos = new Vector3(0, 0, cameraParamsDepth);
                }
                return _cameraParams;
            }
            set => _cameraParams = value;
        }
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
