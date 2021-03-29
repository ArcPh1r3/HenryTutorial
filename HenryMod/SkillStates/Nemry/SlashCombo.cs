using HenryMod.Modules.Components;
using HenryMod.SkillStates.BaseStates;
using RoR2;
using UnityEngine;

namespace HenryMod.SkillStates.Nemry
{
    public class SlashCombo : BaseMeleeAttack
    {
        private NemryEnergyComponent energyComponent;

        public override void OnEnter()
        {
            this.hitboxName = "Sword";

            this.damageType = DamageType.Generic;
            this.damageCoefficient = 2.8f;
            this.procCoefficient = 1f;
            this.pushForce = 300f;
            this.bonusForce = Vector3.zero;
            this.baseDuration = 1f;
            this.attackStartTime = 0.2f;
            this.attackEndTime = 0.4f;
            this.baseEarlyExitTime = 0.4f;
            this.hitStopDuration = 0.012f;
            this.attackRecoil = 0.5f;
            this.hitHopVelocity = 4f;

            this.swingSoundString = "NemrySwordSwing";
            this.hitSoundString = "";
            this.muzzleString = swingIndex % 2 == 0 ? "SwingLeft" : "SwingRight";
            this.swingEffectPrefab = Modules.Assets.nemSwordSwingEffect;
            this.hitEffectPrefab = Modules.Assets.nemSwordHitImpactEffect;

            this.impactSound = Modules.Assets.nemSwordHitSoundEvent.index;

            base.OnEnter();

            this.energyComponent = base.GetComponent<NemryEnergyComponent>();
        }

        protected override void PlayAttackAnimation()
        {
            if (this.animator.GetBool("isGrounded"))
            {
                if (!this.animator.GetBool("isMoving")) base.PlayCrossfade("FullBody, Override", "Slash" + (1 + swingIndex), "Slash.playbackRate", this.duration, 0.05f);
            }
            else
            {
                base.PlayCrossfade("FullBody, Override", "Slash" + (1 + swingIndex), "Slash.playbackRate", this.duration, 0.05f);
            }

            base.PlayCrossfade("Gesture, Override", "Slash" + (1 + swingIndex), "Slash.playbackRate", this.duration, 0.05f);
        }

        protected override void PlaySwingEffect()
        {
            base.PlaySwingEffect();
        }

        protected override void OnHitEnemyAuthority()
        {
            base.OnHitEnemyAuthority();

            this.energyComponent.AddEnergy(20f);
        }

        protected override void SetNextState()
        {
            int index = this.swingIndex + 1;
            if (index == 3) index = 1;

            this.outer.SetNextState(new SlashCombo
            {
                swingIndex = index
            });
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}
