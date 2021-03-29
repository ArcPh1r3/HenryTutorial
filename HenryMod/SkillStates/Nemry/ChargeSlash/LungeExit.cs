using EntityStates;
using HenryMod.Modules.Components;
using HenryMod.SkillStates.BaseStates;
using RoR2;
using UnityEngine;

namespace HenryMod.SkillStates.Nemry.ChargeSlash
{
    public class LungeExit : BaseNemrySkillState
    {
        public static float duration = 0.4f;

        public override void OnEnter()
        {
            base.OnEnter();
            base.SmallHop(base.characterMotor, Lunge.hopForce);
            base.PlayAnimation("FullBody, Override", "LungeSlash", "LungeSlash.playbackRate", 2f * LungeExit.duration);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.isAuthority && base.fixedAge >= LungeExit.duration)
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