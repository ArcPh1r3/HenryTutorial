using EntityStates;
using HenryMod.Modules.Components;
using HenryMod.SkillStates.BaseStates;
using RoR2;
using UnityEngine;

namespace HenryMod.SkillStates.Nemry.Stab
{
    public class StabFail : BaseNemrySkillState
    {
        public static float duration = 0.4f;

        public override void OnEnter()
        {
            base.OnEnter();
            base.PlayAnimation("FullBody, Override", "StabFail", "Stab.playbackRate", 1.25f * StabFail.duration);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.isAuthority && base.fixedAge >= StabFail.duration)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }
    }
}