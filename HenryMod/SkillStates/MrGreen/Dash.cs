using EntityStates;
using RoR2;
using UnityEngine;

namespace HenryMod.SkillStates.MrGreen
{
    public class Dash : BaseState
    {
        private Vector3 dashVector = Vector3.zero;
        public float duration = 0.2f;
        public float speedCoefficient = 8f;

        public override void OnEnter()
        {
            base.OnEnter();
            base.characterBody.isSprinting = true;
            this.dashVector = ((base.inputBank.moveVector == Vector3.zero) ? base.characterDirection.forward : base.inputBank.moveVector).normalized;

            Util.PlaySound("HenryStinger", base.gameObject);
            base.PlayAnimation("Gesture, Override", "BufferEmpty");
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            base.characterMotor.velocity = Vector3.zero;
            base.characterMotor.rootMotion = this.dashVector * (this.moveSpeedStat * this.speedCoefficient * Time.fixedDeltaTime) * Mathf.Cos(base.fixedAge / (this.duration * 1.3f) * 1.57079637f);

            if (base.isAuthority)
            {
                if (base.characterDirection)
                {
                    base.characterDirection.forward = this.dashVector;
                }
            }

            if (base.isAuthority && base.fixedAge >= this.duration)
            {
                this.outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            base.characterMotor.velocity *= 0.1f;
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }
    }
}