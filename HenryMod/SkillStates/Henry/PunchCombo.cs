﻿using HenryMod.SkillStates.BaseStates;
using RoR2;
using UnityEngine;

namespace HenryMod.SkillStates
{
    public class PunchCombo : BaseMeleeAttack
    {
        public override void OnEnter()
        {
            this.hitboxName = "Punch";

            this.damageType = DamageType.BypassArmor;
            this.damageCoefficient = Modules.StaticValues.boxingGlovesDamageCoefficient;
            this.procCoefficient = 1f;
            this.pushForce = 500f;
            this.bonusForce = Vector3.zero;
            this.baseDuration = 0.8f;
            this.attackStartTime = 0.2f;
            this.attackEndTime = 0.4f;
            this.baseEarlyExitTime = 0.3f;
            this.hitStopDuration = 0.015f;
            this.attackRecoil = 0.75f;
            this.hitHopVelocity = 6f;

            this.swingSoundString = "HenryPunchSwing";
            this.hitSoundString = "";
            this.muzzleString = swingIndex % 2 == 0 ? "SwingLeft" : "SwingRight";
            this.swingEffectPrefab = Modules.Assets.punchSwingEffect;
            this.hitEffectPrefab = Modules.Assets.punchImpactEffect;

            this.impactSound = Modules.Assets.punchHitSoundEvent.index;

            base.OnEnter();
        }

        protected override void PlayAttackAnimation()
        {
            if (this.animator.GetBool("isGrounded"))
            {
                if (!this.animator.GetBool("isMoving")) base.PlayCrossfade("FullBody, Override", "Punch" + (1 + swingIndex), "Punch.playbackRate", this.duration, 0.05f);
            }
            else
            {
                base.PlayCrossfade("FullBody, Override", "Punch" + (1 + swingIndex), "Punch.playbackRate", this.duration, 0.05f);
            }

            base.PlayCrossfade("Gesture, Override", "Punch" + (1 + swingIndex), "Punch.playbackRate", this.duration, 0.05f);
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