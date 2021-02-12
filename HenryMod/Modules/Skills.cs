using EntityStates;
using R2API;
using RoR2;
using RoR2.Skills;
using System;
using UnityEngine;

namespace HenryMod.Modules
{
    internal static class Skills
    {
        internal static void CreateSkillFamilies(GameObject targetPrefab)
        {
            foreach (GenericSkill obj in targetPrefab.GetComponentsInChildren<GenericSkill>())
            {
                HenryPlugin.DestroyImmediate(obj);
            }

            SkillLocator skillLocator = targetPrefab.GetComponent<SkillLocator>();

            skillLocator.primary = targetPrefab.AddComponent<GenericSkill>();
            SkillFamily primaryFamily = ScriptableObject.CreateInstance<SkillFamily>();
            primaryFamily.variants = new SkillFamily.Variant[0];
            skillLocator.primary._skillFamily = primaryFamily;
            LoadoutAPI.AddSkillFamily(primaryFamily);

            skillLocator.secondary = targetPrefab.AddComponent<GenericSkill>();
            SkillFamily secondaryFamily = ScriptableObject.CreateInstance<SkillFamily>();
            secondaryFamily.variants = new SkillFamily.Variant[0];
            skillLocator.secondary._skillFamily = secondaryFamily;
            LoadoutAPI.AddSkillFamily(secondaryFamily);

            skillLocator.utility = targetPrefab.AddComponent<GenericSkill>();
            SkillFamily utilityFamily = ScriptableObject.CreateInstance<SkillFamily>();
            utilityFamily.variants = new SkillFamily.Variant[0];
            skillLocator.utility._skillFamily = utilityFamily;
            LoadoutAPI.AddSkillFamily(utilityFamily);

            skillLocator.special = targetPrefab.AddComponent<GenericSkill>();
            SkillFamily specialFamily = ScriptableObject.CreateInstance<SkillFamily>();
            specialFamily.variants = new SkillFamily.Variant[0];
            skillLocator.special._skillFamily = specialFamily;
            LoadoutAPI.AddSkillFamily(specialFamily);
        }

        // this could all be a lot cleaner but at least it's simple and easy to work with
        internal static void AddPrimarySkill(GameObject targetPrefab, SkillDef skillDef)
        {
            SkillLocator skillLocator = targetPrefab.GetComponent<SkillLocator>();

            SkillFamily skillFamily = skillLocator.primary.skillFamily;

            Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);
            skillFamily.variants[skillFamily.variants.Length - 1] = new SkillFamily.Variant
            {
                skillDef = skillDef,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(skillDef.skillNameToken, false, null)
            };
        }

        internal static void AddSecondarySkill(GameObject targetPrefab, SkillDef skillDef)
        {
            SkillLocator skillLocator = targetPrefab.GetComponent<SkillLocator>();

            SkillFamily skillFamily = skillLocator.secondary.skillFamily;

            Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);
            skillFamily.variants[skillFamily.variants.Length - 1] = new SkillFamily.Variant
            {
                skillDef = skillDef,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(skillDef.skillNameToken, false, null)
            };
        }

        internal static void AddUtilitySkill(GameObject targetPrefab, SkillDef skillDef)
        {
            SkillLocator skillLocator = targetPrefab.GetComponent<SkillLocator>();

            SkillFamily skillFamily = skillLocator.utility.skillFamily;

            Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);
            skillFamily.variants[skillFamily.variants.Length - 1] = new SkillFamily.Variant
            {
                skillDef = skillDef,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(skillDef.skillNameToken, false, null)
            };
        }

        internal static void AddSpecialSkill(GameObject targetPrefab, SkillDef skillDef)
        {
            SkillLocator skillLocator = targetPrefab.GetComponent<SkillLocator>();

            SkillFamily skillFamily = skillLocator.special.skillFamily;

            Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);
            skillFamily.variants[skillFamily.variants.Length - 1] = new SkillFamily.Variant
            {
                skillDef = skillDef,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(skillDef.skillNameToken, false, null)
            };
        }

        internal static SkillDef CreatePrimarySkillDef(SerializableEntityStateType state, string stateMachine, string skillNameToken, string skillDescriptionToken, Sprite skillIcon, bool agile)
        {
            SkillDef skillDef = ScriptableObject.CreateInstance<SkillDef>();

            skillDef.skillName = skillNameToken;
            skillDef.skillNameToken = skillNameToken;
            skillDef.skillDescriptionToken = skillDescriptionToken;
            skillDef.icon = skillIcon;

            skillDef.activationState = state;
            skillDef.activationStateMachineName = stateMachine;
            skillDef.baseMaxStock = 1;
            skillDef.baseRechargeInterval = 0;
            skillDef.beginSkillCooldownOnSkillEnd = false;
            skillDef.canceledFromSprinting = false;
            skillDef.forceSprintDuringState = false;
            skillDef.fullRestockOnAssign = true;
            skillDef.interruptPriority = InterruptPriority.Any;
            skillDef.isBullets = false;
            skillDef.isCombatSkill = true;
            skillDef.mustKeyPress = false;
            skillDef.noSprint = !agile;
            skillDef.rechargeStock = 1;
            skillDef.requiredStock = 0;
            skillDef.shootDelay = 0f;
            skillDef.stockToConsume = 0;

            if (agile) skillDef.keywordTokens = new string[] { "KEYWORD_AGILE" };

            LoadoutAPI.AddSkillDef(skillDef);

            return skillDef;
        }

        internal static SkillDef CreateSkillDef(SkillDefInfo skillDefInfo)
        {
            SkillDef skillDef = ScriptableObject.CreateInstance<SkillDef>();

            skillDef.skillName = skillDefInfo.skillName;
            skillDef.skillNameToken = skillDefInfo.skillNameToken;
            skillDef.skillDescriptionToken = skillDefInfo.skillDescriptionToken;
            skillDef.icon = skillDefInfo.skillIcon;

            skillDef.activationState = skillDefInfo.activationState;
            skillDef.activationStateMachineName = skillDefInfo.activationStateMachineName;
            skillDef.baseMaxStock = skillDefInfo.baseMaxStock;
            skillDef.baseRechargeInterval = skillDefInfo.baseRechargeInterval;
            skillDef.beginSkillCooldownOnSkillEnd = skillDefInfo.beginSkillCooldownOnSkillEnd;
            skillDef.canceledFromSprinting = skillDefInfo.canceledFromSprinting;
            skillDef.forceSprintDuringState = skillDefInfo.forceSprintDuringState;
            skillDef.fullRestockOnAssign = skillDefInfo.fullRestockOnAssign;
            skillDef.interruptPriority = skillDefInfo.interruptPriority;
            skillDef.isBullets = skillDefInfo.isBullets;
            skillDef.isCombatSkill = skillDefInfo.isCombatSkill;
            skillDef.mustKeyPress = skillDefInfo.mustKeyPress;
            skillDef.noSprint = skillDefInfo.noSprint;
            skillDef.rechargeStock = skillDefInfo.rechargeStock;
            skillDef.requiredStock = skillDefInfo.requiredStock;
            skillDef.shootDelay = skillDefInfo.shootDelay;
            skillDef.stockToConsume = skillDefInfo.stockToConsume;

            skillDef.keywordTokens = skillDefInfo.keywordTokens;

            LoadoutAPI.AddSkillDef(skillDef);

            return skillDef;
        }
    }
}

internal class SkillDefInfo
{
    public string skillName;
    public string skillNameToken;
    public string skillDescriptionToken;
    public Sprite skillIcon;

    public SerializableEntityStateType activationState;
    public string activationStateMachineName;
    public int baseMaxStock;
    public float baseRechargeInterval;
    public bool beginSkillCooldownOnSkillEnd;
    public bool canceledFromSprinting;
    public bool forceSprintDuringState;
    public bool fullRestockOnAssign;
    public InterruptPriority interruptPriority;
    public bool isBullets;
    public bool isCombatSkill;
    public bool mustKeyPress;
    public bool noSprint;
    public int rechargeStock;
    public int requiredStock;
    public float shootDelay;
    public int stockToConsume;

    public string[] keywordTokens;
}