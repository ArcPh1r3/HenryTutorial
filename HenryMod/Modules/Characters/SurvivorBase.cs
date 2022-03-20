using BepInEx.Configuration;
using RoR2;
using System;
using HenryMod.Modules.Characters;
using UnityEngine;
using System.Collections.Generic;
using RoR2.Skills;

namespace HenryMod.Modules.Survivors
{
    internal abstract class SurvivorBase : CharacterBase
    {
        public abstract string survivorTokenPrefix { get; }
        
        public abstract UnlockableDef characterUnlockableDef { get; }

        public virtual ConfigEntry<bool> characterEnabledConfig { get; }

        public virtual GameObject displayPrefab { get; set; }

        public override void InitializeCharacter()
        {

            if (characterEnabledConfig != null && !characterEnabledConfig.Value)
                return;

            InitializeUnlockables();

            base.InitializeCharacter();

            InitializeSurvivor();
        }

        protected override void InitializeCharacterBodyAndModel()
        {
            base.InitializeCharacterBodyAndModel();
            InitializeDisplayPrefab();
        }

        protected virtual void InitializeSurvivor()
        {
            RegisterNewSurvivor(bodyPrefab, displayPrefab, Color.grey, survivorTokenPrefix, characterUnlockableDef, bodyInfo.sortPosition);
        }

        protected virtual void InitializeDisplayPrefab()
        {
            displayPrefab = Modules.Prefabs.CreateDisplayPrefab(bodyName + "Display", bodyPrefab, bodyInfo);
        }
        public virtual void InitializeUnlockables()
        {
        }

        public static void RegisterNewSurvivor(GameObject bodyPrefab, GameObject displayPrefab, Color charColor, string tokenPrefix) { RegisterNewSurvivor(bodyPrefab, displayPrefab, charColor, tokenPrefix, null, 100f); }
        public static void RegisterNewSurvivor(GameObject bodyPrefab, GameObject displayPrefab, Color charColor, string tokenPrefix, float sortPosition) { RegisterNewSurvivor(bodyPrefab, displayPrefab, charColor, tokenPrefix, null, sortPosition); }
        public static void RegisterNewSurvivor(GameObject bodyPrefab, GameObject displayPrefab, Color charColor, string tokenPrefix, UnlockableDef unlockableDef) { RegisterNewSurvivor(bodyPrefab, displayPrefab, charColor, tokenPrefix, unlockableDef, 100f); }
        public static void RegisterNewSurvivor(GameObject bodyPrefab, GameObject displayPrefab, Color charColor, string tokenPrefix, UnlockableDef unlockableDef, float sortPosition)
        {
            SurvivorDef survivorDef = ScriptableObject.CreateInstance<SurvivorDef>();
            survivorDef.bodyPrefab = bodyPrefab;
            survivorDef.displayPrefab = displayPrefab;
            survivorDef.primaryColor = charColor;

            survivorDef.displayNameToken = tokenPrefix + "NAME";
            survivorDef.descriptionToken = tokenPrefix + "DESCRIPTION";
            survivorDef.outroFlavorToken = tokenPrefix + "OUTRO_FLAVOR";
            survivorDef.mainEndingEscapeFailureFlavorToken = tokenPrefix + "OUTRO_FAILURE";

            survivorDef.desiredSortPosition = sortPosition;
            survivorDef.unlockableDef = unlockableDef;

            Modules.Content.AddSurvivorDef(survivorDef);
        }

        #region CharacterSelectSurvivorPreviewDisplayController
        protected virtual void AddCssPreviewSkill(int indexFromEditor, SkillFamily skillFamily, SkillDef skillDef)
        {
            CharacterSelectSurvivorPreviewDisplayController CSSPreviewDisplayConroller = displayPrefab.GetComponent<CharacterSelectSurvivorPreviewDisplayController>();
            if (!CSSPreviewDisplayConroller)
            {
                Log.Error("trying to add skillChangeResponse to null CharacterSelectSurvivorPreviewDisplayController.\nMake sure you created one on your Display prefab in editor");
                return;
            }

            CSSPreviewDisplayConroller.skillChangeResponses[indexFromEditor].triggerSkillFamily = skillFamily;
            CSSPreviewDisplayConroller.skillChangeResponses[indexFromEditor].triggerSkill = skillDef;
        }

        protected virtual void AddCssPreviewSkin(int indexFromEditor, SkinDef skinDef)
        {
            CharacterSelectSurvivorPreviewDisplayController CSSPreviewDisplayConroller = displayPrefab.GetComponent<CharacterSelectSurvivorPreviewDisplayController>();
            if (!CSSPreviewDisplayConroller)
            {
                Log.Error("trying to add skinChangeResponse to null CharacterSelectSurvivorPreviewDisplayController.\nMake sure you created one on your Display prefab in editor");
                return;
            }

            CSSPreviewDisplayConroller.skinChangeResponses[indexFromEditor].triggerSkin = skinDef;
        }

        protected virtual void FinalizeCSSPreviewDisplayController()
        {
            if (!displayPrefab)
                return;

            CharacterSelectSurvivorPreviewDisplayController CSSPreviewDisplayConroller = displayPrefab.GetComponent<CharacterSelectSurvivorPreviewDisplayController>();
            if (!CSSPreviewDisplayConroller)
                return;

            //set body prefab
            CSSPreviewDisplayConroller.bodyPrefab = bodyPrefab;

            //clear list of null entries
            List<CharacterSelectSurvivorPreviewDisplayController.SkillChangeResponse> newlist = new List<CharacterSelectSurvivorPreviewDisplayController.SkillChangeResponse>();

            for (int i = 0; i < CSSPreviewDisplayConroller.skillChangeResponses.Length; i++)
            {
                if (CSSPreviewDisplayConroller.skillChangeResponses[i].triggerSkillFamily != null)
                {
                    newlist.Add(CSSPreviewDisplayConroller.skillChangeResponses[i]);
                }
            }

            CSSPreviewDisplayConroller.skillChangeResponses = newlist.ToArray();
        }
        #endregion
    }
}
