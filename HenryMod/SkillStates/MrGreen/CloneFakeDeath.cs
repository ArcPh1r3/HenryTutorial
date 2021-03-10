using EntityStates;
using RoR2;
using UnityEngine;

namespace HenryMod.SkillStates.MrGreen
{
    public class CloneFakeDeath : BaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();

            Vector3 vector = Vector3.up * 3f;

            if (base.characterMotor)
            {
                vector += base.characterMotor.velocity;
                base.characterMotor.enabled = false;
            }

            if (base.modelLocator)
            {
                RagdollController ragdoll = base.modelLocator.modelTransform.GetComponent<RagdollController>();
                if (ragdoll) ragdoll.BeginRagdoll(vector);
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Death;
        }
    }
}