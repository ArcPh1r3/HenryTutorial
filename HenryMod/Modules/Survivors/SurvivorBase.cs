using BepInEx.Configuration;
using RoR2;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace HenryMod.Modules.Survivors
{
    internal abstract class SurvivorBase
    {
        internal static SurvivorBase instance;

        internal abstract string bodyName { get; set; }

        internal abstract GameObject bodyPrefab { get; set; }
        internal abstract GameObject displayPrefab { get; set; }

        internal abstract float sortPosition { get; set; }

        internal string fullBodyName => bodyName + "Body";

        internal abstract ConfigEntry<bool> characterEnabled { get; set; }

        internal abstract UnlockableDef characterUnlockableDef { get; set; }

        internal abstract BodyInfo bodyInfo { get; set; }

        internal abstract int mainRendererIndex { get; set; }
        internal abstract CustomRendererInfo[] customRendererInfos { get; set; }

        internal abstract Type characterMainState { get; set; }

        internal abstract ItemDisplayRuleSet itemDisplayRuleSet { get; set; }
        internal abstract List<ItemDisplayRuleSet.KeyAssetRuleGroup> itemDisplayRules { get; set; }

        internal virtual void Initialize()
        {
            instance = this;
            InitializeCharacter();
        }

        internal virtual void InitializeCharacter()
        {
            // this creates a config option to enable the character- feel free to remove if the character is the only thing in your mod
            characterEnabled = Modules.Config.CharacterEnableConfig(bodyName);

            if (characterEnabled.Value)
            {
                InitializeUnlockables();

                bodyPrefab = Modules.Prefabs.CreatePrefab(bodyName + "Body", "mdl" + bodyName, bodyInfo);
                bodyPrefab.GetComponent<EntityStateMachine>().mainStateType = new EntityStates.SerializableEntityStateType(characterMainState);

                Modules.Prefabs.SetupCharacterModel(bodyPrefab, customRendererInfos, mainRendererIndex);

                displayPrefab = Modules.Prefabs.CreateDisplayPrefab(bodyName + "Display", bodyPrefab, bodyInfo);

                Modules.Prefabs.RegisterNewSurvivor(bodyPrefab, displayPrefab, Color.grey, bodyName.ToUpper(), characterUnlockableDef, sortPosition);

                InitializeHitboxes();
                InitializeSkills();
                InitializeSkins();
                InitializeItemDisplays();
                InitializeDoppelganger();
            }
        }


        internal virtual void InitializeUnlockables()
        {
        }

        internal virtual void InitializeSkills()
        {
        }

        internal virtual void InitializeHitboxes()
        {
        }

        internal virtual void InitializeSkins()
        {
        }

        internal virtual void InitializeDoppelganger()
        {
            Modules.Prefabs.CreateGenericDoppelganger(instance.bodyPrefab, bodyName + "MonsterMaster", "Merc");
        }

        internal virtual void InitializeItemDisplays()
        {
            CharacterModel characterModel = bodyPrefab.GetComponentInChildren<CharacterModel>();

            itemDisplayRuleSet = ScriptableObject.CreateInstance<ItemDisplayRuleSet>();
            itemDisplayRuleSet.name = "idrs" + bodyName;

            characterModel.itemDisplayRuleSet = itemDisplayRuleSet;
        }

        internal virtual void SetItemDisplays()
        {

        }
    }
}
