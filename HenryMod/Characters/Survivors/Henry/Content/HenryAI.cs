//todo windows change namespace

using RoR2;
using RoR2.CharacterAI;
using UnityEngine;

namespace HenryMod.Characters.Survivors.Henry.Content
{
    public static class HenryAI
    {
        internal static void Init(GameObject bodyPrefab)
        {
            //todo ai mess with baseai?
                //in which case todo ser
            GameObject master = Modules.Prefabs.CreateBlankMasterPrefab(bodyPrefab, "HenryMonsterMaster");

            AISkillDriver swingDriver = master.AddComponent<AISkillDriver>();
            swingDriver.customName = "Blast";
            swingDriver.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            swingDriver.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            swingDriver.activationRequiresAimConfirmation = true;
            swingDriver.activationRequiresTargetLoS = false;
            swingDriver.selectionRequiresTargetLoS = true;
            swingDriver.maxDistance = 80f;
            swingDriver.minDistance = 8f;
            swingDriver.requireSkillReady = true;
            swingDriver.aimType = AISkillDriver.AimType.AtCurrentEnemy;
            swingDriver.ignoreNodeGraph = true;
            swingDriver.moveInputScale = 1f;
            swingDriver.driverUpdateTimerOverride = 2.5f;
            swingDriver.buttonPressType = AISkillDriver.ButtonPressType.Hold;
            swingDriver.minTargetHealthFraction = Mathf.NegativeInfinity;
            swingDriver.maxTargetHealthFraction = Mathf.Infinity;
            swingDriver.minUserHealthFraction = Mathf.NegativeInfinity;
            swingDriver.maxUserHealthFraction = 0.5f;
            swingDriver.skillSlot = SkillSlot.Primary;

            //todo ai and so on and so forth
            //bruh just do it in unity
        }
    }
}
