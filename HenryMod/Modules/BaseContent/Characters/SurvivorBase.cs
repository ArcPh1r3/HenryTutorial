using BepInEx.Configuration;
using RoR2;
using UnityEngine;
using System.Collections.Generic;
using RoR2.Skills;

namespace HenryMod.Modules.Characters
{
    public abstract class SurvivorBase<T> : CharacterBase<T> where T : SurvivorBase<T>, new()
    {
        public abstract string masterName { get; }

        public abstract string displayPrefabName { get; }

        public abstract string survivorTokenPrefix { get; }

        public abstract UnlockableDef characterUnlockableDef { get; }

        public abstract GameObject displayPrefab { get; protected set; }

        public override void InitializeCharacter()
        {
            base.InitializeCharacter();

            InitializeDisplayPrefab();

            InitializeSurvivor();
        }

        protected virtual void InitializeDisplayPrefab()
        {
            displayPrefab = Prefabs.CreateDisplayPrefab(assetBundle, displayPrefabName, bodyPrefab);
        }

        protected virtual void InitializeSurvivor()
        {
            Content.CreateSurvivor(bodyPrefab, displayPrefab, bodyInfo.bodyColor, survivorTokenPrefix, characterUnlockableDef, bodyInfo.sortPosition);
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
