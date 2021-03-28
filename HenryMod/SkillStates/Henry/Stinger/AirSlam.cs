using EntityStates;
using HenryMod.SkillStates.BaseStates;
using RoR2;
using UnityEngine;

namespace HenryMod.SkillStates.Stinger
{
    public class AirSlam : BaseMeleeAttack
    {
        public override void OnEnter()
        {
            this.hitboxName = "Punch";

            this.damageType = DamageType.BypassArmor;
            this.damageCoefficient = 6f;
            this.procCoefficient = 1f;
            this.pushForce = 200f;
            this.bonusForce = Vector3.up * -3200f;
            this.baseDuration = 1f;
            this.attackStartTime = 0.25f;
            this.attackEndTime = 0.4f;
            this.baseEarlyExitTime = 0f;
            this.hitStopDuration = 0.03f;
            this.attackRecoil = 1.75f;
            this.hitHopVelocity = 16f;

            this.swingSoundString = "HenryPunchSwing";
            this.hitSoundString = "";
            this.muzzleString = "SwingDown";
            this.swingEffectPrefab = Modules.Assets.punchSwingEffect;
            this.hitEffectPrefab = Modules.Assets.punchImpactEffect;

            this.impactSound = Modules.Assets.punchHitSoundEvent.index;

            base.OnEnter();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.characterMotor.velocity.y < 0f && this.stopwatch <= this.attackEndTime) base.characterMotor.velocity.y = 0f;
        }

        protected override void PlayAttackAnimation()
        {
            base.PlayCrossfade("FullBody, Override", "AirSlam", "AirSlam.playbackRate", this.duration, 0.05f);
        }

        protected override void PlaySwingEffect()
        {
            base.PlaySwingEffect();

            base.SmallHop(base.characterMotor, 0.5f * this.hitHopVelocity);
        }

        protected override void OnHitEnemyAuthority()
        {
            base.OnHitEnemyAuthority();
        }

        protected override void SetNextState()
        {
            this.outer.SetNextStateToMain();
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}