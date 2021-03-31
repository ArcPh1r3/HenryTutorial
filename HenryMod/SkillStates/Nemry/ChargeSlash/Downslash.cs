using EntityStates;
using HenryMod.SkillStates.BaseStates;
using RoR2;
using UnityEngine;

namespace HenryMod.SkillStates.Nemry.ChargeSlash
{
    public class Downslash : BaseMeleeAttack
    {
        public static float maxDamageCoefficient = 12f;
        public static float minDamageCoefficient = 6f;

        public float charge;

        public override void OnEnter()
        {
            this.hitboxName = "Sword";

            this.damageType = DamageType.Generic;
            this.damageCoefficient = Util.Remap(this.charge, 0f, 1f, Downslash.minDamageCoefficient, Downslash.maxDamageCoefficient);
            this.procCoefficient = 1f;
            this.pushForce = 200f;
            this.bonusForce = Vector3.up * -3200f;
            this.baseDuration = 1f;
            this.attackStartTime = 0.225f;
            this.attackEndTime = 0.4f;
            this.baseEarlyExitTime = 0f;
            this.hitStopDuration = 0.03f;
            this.attackRecoil = 1.75f;
            this.hitHopVelocity = 16f;

            this.swingSoundString = "NemrySwordSwing";
            this.hitSoundString = "";
            this.muzzleString = "SwingDown";
            this.swingEffectPrefab = Modules.Assets.nemSwordHeavySwingEffect;
            this.hitEffectPrefab = Modules.Assets.nemSwordHitImpactEffect;

            this.impactSound = Modules.Assets.nemSwordHitSoundEvent.index;

            base.OnEnter();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.characterMotor.velocity.y < 0f && this.stopwatch <= this.attackEndTime) base.characterMotor.velocity.y = 0f;
        }

        protected override void PlayAttackAnimation()
        {
            base.PlayCrossfade("FullBody, Override", "Downslash", "Downslash.playbackRate", this.duration, 0.05f);
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
            if (this.stopwatch < this.duration * 0.8f) return InterruptPriority.PrioritySkill;
            else return InterruptPriority.Any;
        }
    }
}