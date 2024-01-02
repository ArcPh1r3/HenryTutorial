using RoR2;
using RoR2.CharacterAI;
using UnityEngine;

namespace HenryMod.Survivors.Henry
{
    public static class HenryAI
    {
        public static void Init(GameObject bodyPrefab, string masterName)
        {
            GameObject master = Modules.Prefabs.CreateBlankMasterPrefab(bodyPrefab, masterName);

            BaseAI baseAI = master.GetComponent<BaseAI>();
            baseAI.aimVectorDampTime = 0.1f;
            baseAI.aimVectorMaxSpeed = 360;

            //mouse over these fields for tooltips
            AISkillDriver swingDriver = master.AddComponent<AISkillDriver>();
            //Selection Conditions
            swingDriver.customName = "Use Primary Swing";
            swingDriver.skillSlot = SkillSlot.Primary;
            swingDriver.requiredSkill = null; //usually used when you have skills that override other skillslots like engi harpoons
            swingDriver.requireSkillReady = false; //usually false for primaries
            swingDriver.requireEquipmentReady = false;
            swingDriver.minUserHealthFraction = float.NegativeInfinity;
            swingDriver.maxUserHealthFraction = float.PositiveInfinity;
            swingDriver.minTargetHealthFraction = float.NegativeInfinity;
            swingDriver.maxTargetHealthFraction = float.PositiveInfinity;
            swingDriver.minDistance = 0;
            swingDriver.maxDistance = 8;
            swingDriver.selectionRequiresTargetLoS = false;
            swingDriver.selectionRequiresOnGround = false;
            swingDriver.selectionRequiresAimTarget = false;
            swingDriver.maxTimesSelected = -1;

            //Behavior
            swingDriver.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            swingDriver.activationRequiresTargetLoS = false;
            swingDriver.activationRequiresAimTargetLoS = false;
            swingDriver.activationRequiresAimConfirmation = false;
            swingDriver.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            swingDriver.moveInputScale = 1;
            swingDriver.aimType = AISkillDriver.AimType.AtMoveTarget;
            swingDriver.ignoreNodeGraph = false; //will chase relentlessly but be kind of stupid
            swingDriver.shouldSprint = false; 
            swingDriver.shouldFireEquipment = false;
            swingDriver.buttonPressType = AISkillDriver.ButtonPressType.Hold; 

            //Transition Behavior
            swingDriver.driverUpdateTimerOverride = -1;
            swingDriver.resetCurrentEnemyOnNextDriverSelection = false;
            swingDriver.noRepeat = false;
            swingDriver.nextHighPriorityOverride = null;

            //some fields omitted that aren't commonly changed. will be set to default values
            AISkillDriver shootDriver = master.AddComponent<AISkillDriver>();
            //Selection Conditions
            shootDriver.customName = "Use Secondary Shoot";
            shootDriver.skillSlot = SkillSlot.Secondary;
            shootDriver.requireSkillReady = true;
            shootDriver.minDistance = 0;
            shootDriver.maxDistance = 25;
            shootDriver.selectionRequiresTargetLoS = false;
            shootDriver.selectionRequiresOnGround = false;
            shootDriver.selectionRequiresAimTarget = false;
            shootDriver.maxTimesSelected = -1;

            //Behavior
            shootDriver.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            shootDriver.activationRequiresTargetLoS = false;
            shootDriver.activationRequiresAimTargetLoS = false;
            shootDriver.activationRequiresAimConfirmation = true;
            shootDriver.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            shootDriver.moveInputScale = 1;
            shootDriver.aimType = AISkillDriver.AimType.AtMoveTarget;
            shootDriver.buttonPressType = AISkillDriver.ButtonPressType.Hold; 
            
            AISkillDriver rollDriver = master.AddComponent<AISkillDriver>();
            //Selection Conditions
            rollDriver.customName = "Use Utility Roll";
            rollDriver.skillSlot = SkillSlot.Utility;
            rollDriver.requireSkillReady = true;
            rollDriver.minDistance = 8;
            rollDriver.maxDistance = 20;
            rollDriver.selectionRequiresTargetLoS = true;
            rollDriver.selectionRequiresOnGround = false;
            rollDriver.selectionRequiresAimTarget = false;
            rollDriver.maxTimesSelected = -1;

            //Behavior
            rollDriver.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            rollDriver.activationRequiresTargetLoS = false;
            rollDriver.activationRequiresAimTargetLoS = false;
            rollDriver.activationRequiresAimConfirmation = false;
            rollDriver.movementType = AISkillDriver.MovementType.StrafeMovetarget;
            rollDriver.moveInputScale = 1;
            rollDriver.aimType = AISkillDriver.AimType.AtMoveTarget;
            rollDriver.buttonPressType = AISkillDriver.ButtonPressType.Hold;

            AISkillDriver bombDriver = master.AddComponent<AISkillDriver>();
            //Selection Conditions
            bombDriver.customName = "Use Special bomb";
            bombDriver.skillSlot = SkillSlot.Special;
            bombDriver.requireSkillReady = true;
            bombDriver.minDistance = 0;
            bombDriver.maxDistance = 20;
            bombDriver.selectionRequiresTargetLoS = false;
            bombDriver.selectionRequiresOnGround = false;
            bombDriver.selectionRequiresAimTarget = false;
            bombDriver.maxTimesSelected = -1;

            //Behavior
            bombDriver.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            bombDriver.activationRequiresTargetLoS = false;
            bombDriver.activationRequiresAimTargetLoS = false;
            bombDriver.activationRequiresAimConfirmation = false;
            bombDriver.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            bombDriver.moveInputScale = 1;
            bombDriver.aimType = AISkillDriver.AimType.AtMoveTarget;
            bombDriver.buttonPressType = AISkillDriver.ButtonPressType.Hold;

            AISkillDriver chaseDriver = master.AddComponent<AISkillDriver>();
            //Selection Conditions
            chaseDriver.customName = "Chase";
            chaseDriver.skillSlot = SkillSlot.None;
            chaseDriver.requireSkillReady = false;
            chaseDriver.minDistance = 0;
            chaseDriver.maxDistance = float.PositiveInfinity;

            //Behavior
            chaseDriver.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            chaseDriver.activationRequiresTargetLoS = false;
            chaseDriver.activationRequiresAimTargetLoS = false;
            chaseDriver.activationRequiresAimConfirmation = false;
            chaseDriver.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            chaseDriver.moveInputScale = 1;
            chaseDriver.aimType = AISkillDriver.AimType.AtMoveTarget;
            chaseDriver.buttonPressType = AISkillDriver.ButtonPressType.Hold;

            //recommend taking these for a spin in game, messing with them in runtimeinspector to get a feel for what they should do at certain ranges and such
        }
    }
}
