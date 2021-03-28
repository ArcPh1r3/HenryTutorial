using EntityStates;
using HenryMod.Modules.Components;
using HenryMod.SkillStates.BaseStates;
using RoR2;
using UnityEngine;

namespace HenryMod.SkillStates.Nemry.Stab
{
    public class StabEntry : BaseMeleeAttack
    {
        private bool hasHit;

        public override void OnEnter()
        {
            this.hitboxName = "Sword";

            this.damageType = DamageType.Generic;
            this.damageCoefficient = 16f;
            this.procCoefficient = 1f;
            this.pushForce = -500f;
            this.baseDuration = 0.6f;
            this.attackStartTime = 0.8f;
            this.attackEndTime = 0.95f;
            this.baseEarlyExitTime = 0f;
            this.hitStopDuration = 0.03f;
            this.attackRecoil = 2.5f;
            this.hitHopVelocity = 8f;

            this.swingSoundString = "NemrySwordSwing";
            this.hitSoundString = "";
            this.muzzleString = "SwingDown";
            this.swingEffectPrefab = Modules.Assets.nemSwordSwingEffect;
            this.hitEffectPrefab = Modules.Assets.nemSwordHitImpactEffect;

            this.impactSound = Modules.Assets.nemSwordHitSoundEvent.index;

            base.OnEnter();

            NemryEnergyComponent energyComponent = base.GetComponent<NemryEnergyComponent>();
            if (energyComponent) energyComponent.SpendEnergy(100f, SkillSlot.Special);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.characterMotor.velocity.y < 0f && this.stopwatch <= this.attackEndTime) base.characterMotor.velocity.y = 0f;
        }

        protected override void PlayAttackAnimation()
        {
            base.PlayCrossfade("FullBody, Override", "StabEntry", "Stab.playbackRate", this.duration, 0.05f);
        }

        protected override void PlaySwingEffect()
        {
            base.PlaySwingEffect();
        }

        protected override void OnHitEnemyAuthority()
        {
            base.OnHitEnemyAuthority();

            if (!this.hasHit)
            {
                this.hasHit = true;
                this.outer.SetNextState(new StabSuccess());
                return;
            }
        }

        protected override void SetNextState()
        {
            this.outer.SetNextState(new StabFail());
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