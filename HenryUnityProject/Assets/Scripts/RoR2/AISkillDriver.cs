using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2.CharacterAI
{
    public class AISkillDriver : MonoBehaviour
    {// Token: 0x0400458C RID: 17804
        [Tooltip("The name of this skill driver for reference purposes.")]
        public string customName;

        // Token: 0x0400458D RID: 17805
        [Tooltip("The slot of the associated skill. Set to None to allow this behavior to run regardless of skill availability.")]
        public SkillSlot skillSlot;


        // Token: 0x0400458F RID: 17807
        [Header("Selection Conditions")]
        [Tooltip("If set, this cannot be the dominant driver while the skill is on cooldown or out of stock.")]
        public bool requireSkillReady;

        // Token: 0x0400458E RID: 17806
        //[Tooltip("The skill that the specified slot must have for this behavior to run. Set to none to allow any skill.")]
        //public SkillDef requiredSkill;

        // Token: 0x04004590 RID: 17808
        [Tooltip("If set, this cannot be the dominant driver while the equipment is on cooldown or out of stock.")]
        public bool requireEquipmentReady;

        // Token: 0x04004591 RID: 17809
        [Tooltip("The minimum health fraction required of the user for this behavior.")]
        public float minUserHealthFraction = float.NegativeInfinity;

        // Token: 0x04004592 RID: 17810
        [Tooltip("The maximum health fraction required of the user for this behavior.")]
        public float maxUserHealthFraction = float.PositiveInfinity;

        // Token: 0x04004593 RID: 17811
        [Tooltip("The minimum health fraction required of the target for this behavior.")]
        public float minTargetHealthFraction = float.NegativeInfinity;

        // Token: 0x04004594 RID: 17812
        [Tooltip("The maximum health fraction required of the target for this behavior.")]
        public float maxTargetHealthFraction = float.PositiveInfinity;

        // Token: 0x04004595 RID: 17813
        [Tooltip("The minimum distance from the target required for this behavior.")]
        public float minDistance;

        // Token: 0x04004596 RID: 17814
        [Tooltip("The maximum distance from the target required for this behavior.")]
        public float maxDistance = float.PositiveInfinity;

        // Token: 0x04004597 RID: 17815
        public bool selectionRequiresTargetLoS;

        // Token: 0x04004598 RID: 17816
        public bool selectionRequiresOnGround;

        // Token: 0x04004599 RID: 17817
        public bool selectionRequiresAimTarget;

        // Token: 0x0400459A RID: 17818
        [Tooltip("The maximum number of times that this skill can be selected.  If the value is < 0, then there is no maximum.")]
        public int maxTimesSelected = -1;

        // Token: 0x0400459B RID: 17819
        [Header("Behavior")]
        [Tooltip("The type of object targeted for movement.")]
        public AISkillDriver.TargetType moveTargetType;

        // Token: 0x0400459C RID: 17820
        [Tooltip("If set, this skill will not be activated unless there is LoS to the target.")]
        public bool activationRequiresTargetLoS;

        // Token: 0x0400459D RID: 17821
        [Tooltip("If set, this skill will not be activated unless there is LoS to the aim target.")]
        public bool activationRequiresAimTargetLoS;

        // Token: 0x0400459E RID: 17822
        [Tooltip("If set, this skill will not be activated unless the aim vector is pointing close to the target.")]
        public bool activationRequiresAimConfirmation;

        // Token: 0x0400459F RID: 17823
        [Tooltip("The movement type to use while this is the dominant skill driver.")]
        public AISkillDriver.MovementType movementType = AISkillDriver.MovementType.ChaseMoveTarget;

        // Token: 0x040045A0 RID: 17824
        public float moveInputScale = 1f;

        // Token: 0x040045A1 RID: 17825
        [Tooltip("Where to look while this is the dominant skill driver")]
        public AISkillDriver.AimType aimType = AISkillDriver.AimType.AtMoveTarget;

        // Token: 0x040045A2 RID: 17826
        [Tooltip("If set, the nodegraph will not be used to direct the local navigator while this is the dominant skill driver. Direction toward the target will be used instead.")]
        public bool ignoreNodeGraph;

        // Token: 0x040045A3 RID: 17827
        [Tooltip("If true, the AI will attempt to sprint while this is the dominant skill driver.")]
        public bool shouldSprint;

        // Token: 0x040045A4 RID: 17828
        public bool shouldFireEquipment;

        // Token: 0x040045A6 RID: 17830
        public AISkillDriver.ButtonPressType buttonPressType;

        // Token: 0x040045A7 RID: 17831
        [Header("Transition Behavior")]
        [Tooltip("If non-negative, this value will be used for the driver evaluation timer while this is the dominant skill driver.")]
        public float driverUpdateTimerOverride = -1f;

        // Token: 0x040045A8 RID: 17832
        [Tooltip("If set and this is the dominant skill driver, the current enemy will be reset at the time of the next evaluation.")]
        public bool resetCurrentEnemyOnNextDriverSelection;

        // Token: 0x040045A9 RID: 17833
        [Tooltip("If true, this skill driver cannot be chosen twice in a row.")]
        public bool noRepeat;

        // Token: 0x040045AA RID: 17834
        [Tooltip("The AI skill driver that will be treated as having top priority after this one.")]
        public AISkillDriver nextHighPriorityOverride;

        // Token: 0x02000C71 RID: 3185
        public enum TargetType
        {
            // Token: 0x040045AD RID: 17837
            CurrentEnemy,
            // Token: 0x040045AE RID: 17838
            NearestFriendlyInSkillRange,
            // Token: 0x040045AF RID: 17839
            CurrentLeader,
            // Token: 0x040045B0 RID: 17840
            Custom
        }

        // Token: 0x02000C72 RID: 3186
        public enum AimType
        {
            // Token: 0x040045B2 RID: 17842
            None,
            // Token: 0x040045B3 RID: 17843
            AtMoveTarget,
            // Token: 0x040045B4 RID: 17844
            AtCurrentEnemy,
            // Token: 0x040045B5 RID: 17845
            AtCurrentLeader,
            // Token: 0x040045B6 RID: 17846
            MoveDirection
        }

        // Token: 0x02000C73 RID: 3187
        public enum MovementType
        {
            // Token: 0x040045B8 RID: 17848
            Stop,
            // Token: 0x040045B9 RID: 17849
            ChaseMoveTarget,
            // Token: 0x040045BA RID: 17850
            StrafeMovetarget,
            // Token: 0x040045BB RID: 17851
            FleeMoveTarget
        }

        // Token: 0x02000C74 RID: 3188
        public enum ButtonPressType
        {
            // Token: 0x040045BD RID: 17853
            Hold,
            // Token: 0x040045BE RID: 17854
            Abstain,
            // Token: 0x040045BF RID: 17855
            TapContinuous
        }
    }
}