using EntityStates;
using HenryMod.Modules.Components;
using RoR2;
using UnityEngine;

namespace HenryMod.SkillStates.Stinger
{
    public class StingerExit : BaseSkillState
    {
        public static float duration = 0.1f;

        public override void OnEnter()
        {
            base.OnEnter();
            base.SmallHop(base.characterMotor, Stinger.hopForce);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.isAuthority && base.fixedAge >= StingerExit.duration)
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