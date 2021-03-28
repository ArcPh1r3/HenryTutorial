using RoR2;
using EntityStates;
using UnityEngine;

namespace HenryMod.SkillStates.Nemry
{
    public class DodgeSlash : BaseSkillState
    {
        public static float duration = 0.6f;

        private float previousAirControl;

        public override void OnEnter()
        {
            base.OnEnter();
            this.previousAirControl = base.characterMotor.airControl;
            base.characterMotor.airControl = EntityStates.Croco.Leap.airControl;

            Vector3 direction = -base.GetAimRay().direction;

            if (base.isAuthority)
            {
                base.characterBody.isSprinting = true;

                direction.y = Mathf.Max(direction.y, EntityStates.Croco.Leap.minimumY);
                Vector3 a = direction.normalized * EntityStates.Croco.Leap.aimVelocity * 10f;
                Vector3 b = Vector3.up * EntityStates.Croco.Leap.upwardVelocity;
                Vector3 b2 = new Vector3(direction.x, 0f, direction.z).normalized * EntityStates.Croco.Leap.forwardVelocity;

                base.characterMotor.Motor.ForceUnground();
                base.characterMotor.velocity = a + b + b2;
                base.characterMotor.velocity.y *= 0.8f;
                if (base.characterMotor.velocity.y < 0) base.characterMotor.velocity.y *= 0.1f;
            }

            base.characterDirection.moveVector = direction;

            base.characterBody.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;

            base.PlayAnimation("FullBody, Override", "DodgeSlash", "DodgeSlash.playbackRate", DodgeSlash.duration);
            Util.PlayAttackSpeedSound(EntityStates.Commando.DodgeState.dodgeSoundString, base.gameObject, 1.5f);
        }

        public override void OnExit()
        {
            base.OnExit();

            base.characterBody.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;
            base.characterMotor.airControl = this.previousAirControl;

            base.characterMotor.velocity *= 0.9f;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            base.StartAimMode(0.5f, false);

            if (base.fixedAge >= DodgeSlash.duration && base.isAuthority)
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