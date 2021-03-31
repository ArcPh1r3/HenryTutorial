using HenryMod.SkillStates.BaseStates;
using RoR2;
using UnityEngine;

namespace HenryMod.SkillStates
{
    public class FistBarrage : BaseMeleeAttack
    {
        public override void OnEnter()
        {
            this.hitboxName = "Punch";

            this.damageType = DamageType.BypassArmor;
            this.damageCoefficient = Modules.StaticValues.boxingGlovesDamageCoefficient;
            this.procCoefficient = 1f;
            this.pushForce = 100f;
            this.bonusForce = Vector3.zero;
            this.baseDuration = 0.8f;
            this.attackStartTime = 0.2f;
            this.attackEndTime = 0.4f;
            this.baseEarlyExitTime = 0.3f;
            this.hitStopDuration = 0.015f;
            this.attackRecoil = 0.75f;
            this.hitHopVelocity = 2f;

            this.swingSoundString = "HenryPunchSwing";
            this.hitSoundString = "";
            this.muzzleString = "SwingCenter";
            this.swingEffectPrefab = Modules.Assets.fistBarrageEffect;
            this.hitEffectPrefab = Modules.Assets.punchImpactEffect;

            this.impactSound = Modules.Assets.punchHitSoundEvent.index;

            base.OnEnter();
        }

        protected override void PlayAttackAnimation()
        {
            if (this.animator.GetBool("isGrounded"))
            {
                if (!this.animator.GetBool("isMoving")) base.PlayAnimation("FullBody, Override", "FastPunch" + (1 + swingIndex), "Punch.playbackRate", this.duration);
            }
            else
            {
                base.PlayAnimation("FullBody, Override", "FastPunch" + (1 + swingIndex), "FastPunch.playbackRate", this.duration);
            }

            base.PlayAnimation("Gesture, Override", "FastPunch" + (1 + swingIndex), "FastPunch.playbackRate", this.duration);
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
            int index = this.swingIndex + 1;
            if (index == 3) index = 1;

            if (this.attackSpeedStat >= 5f)
            {
                this.outer.SetNextState(new FistBarrage
                {
                    swingIndex = index
                });
            }
            else
            {
                this.outer.SetNextState(new PunchCombo
                {
                    swingIndex = index
                });
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}