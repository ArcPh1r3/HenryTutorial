using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RoR2
{
    public class HurtBox : MonoBehaviour
    {
        //set in code in the template
        //// Token: 0x04002A7C RID: 10876
        //[Tooltip("The health component to which this hurtbox belongs.")]
        //public HealthComponent healthComponent;

        // Token: 0x04002A7D RID: 10877
        [Tooltip("Whether or not this hurtbox is considered a bullseye. Mainly used for targeting skills. Do not change this at runtime!")]
        public bool isBullseye;

        // Token: 0x04002A7E RID: 10878
        [Tooltip("Whether or not this hurtbox is considered a sniper target. Do not change this at runtime!")]
        public bool isSniperTarget;

        // Token: 0x04002A7F RID: 10879
        public HurtBox.DamageModifier damageModifier;

        // Token: 0x04002A81 RID: 10881
        [SerializeField]
        [HideInInspector]
        public HurtBoxGroup hurtBoxGroup;

        // Token: 0x04002A82 RID: 10882
        [HideInInspector]
        [SerializeField]
        public short indexInGroup = -1;


        // Token: 0x0200073E RID: 1854
        public enum DamageModifier
        {
            // Token: 0x04002A8D RID: 10893
            Normal,
            // Token: 0x04002A8F RID: 10895
            Weak,
            // Token: 0x04002A90 RID: 10896
            Barrier
        }
    }
}