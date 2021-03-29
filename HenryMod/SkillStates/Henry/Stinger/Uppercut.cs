using EntityStates;
using HenryMod.SkillStates.BaseStates;
using RoR2;
using UnityEngine;

namespace HenryMod.SkillStates.Stinger
{
    public class Uppercut : BaseMeleeAttack
    {
        public override void OnEnter()
        {
            this.hitboxName = "Punch";

            this.damageType = DamageType.Stun1s;
            this.damageCoefficient = 4.5f;
            this.procCoefficient = 1f;
            this.pushForce = 200f;
            this.bonusForce = Vector3.up * 3200f;
            this.baseDuration = 1f;
            this.attackStartTime = 0.225f;
            this.attackEndTime = 0.4f;
            this.baseEarlyExitTime = 0f;
            this.hitStopDuration = 0f;
            this.attackRecoil = 1.75f;
            this.hitHopVelocity = 0f;

            this.swingSoundString = "HenryPunchSwing";
            this.hitSoundString = "";
            this.muzzleString = "SwingUp";
            this.swingEffectPrefab = Modules.Assets.punchSwingEffect;
            this.hitEffectPrefab = Modules.Assets.punchImpactEffect;

            this.impactSound = Modules.Assets.punchHitSoundEvent.index;

            base.OnEnter();

            EffectManager.SimpleEffect(Modules.Assets.dustEffect, base.characterBody.footPosition, base.transform.rotation, false);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (!this.inHitPause)
            {
                if (base.characterMotor && base.characterDirection)
                {
                    Vector3 velocity = base.characterDirection.forward * (0.5f * this.moveSpeedStat) * Mathf.Lerp(EntityStates.Merc.Uppercut.moveSpeedBonusCoefficient, 0f, this.stopwatch / (0.8f * this.duration));
                    velocity.y = EntityStates.Merc.Uppercut.yVelocityCurve.Evaluate(this.stopwatch / (0.8f * this.duration));
                    base.characterMotor.velocity = velocity;
                }
            }
        }

        protected override void PlayAttackAnimation()
        {
            base.PlayCrossfade("FullBody, Override", "Uppercut", "Uppercut.playbackRate", this.duration, 0.05f);
        }

        protected override void PlaySwingEffect()
        {
            base.PlaySwingEffect();
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