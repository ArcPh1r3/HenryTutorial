using HenryMod.SkillStates.BaseStates;
using RoR2;
using UnityEngine;

namespace HenryMod.SkillStates.Nemry
{
    public class SpearStab : BaseMeleeAttack
    {
        public override void OnEnter()
        {
            this.hitboxName = "Spear";

            this.damageType = DamageType.ApplyMercExpose;
            this.damageCoefficient = 1.6f;
            this.procCoefficient = 1f;
            this.pushForce = 250f;
            this.bonusForce = Vector3.zero;
            this.baseDuration = 0.8f;
            this.attackStartTime = 0.25f;
            this.attackEndTime = 0.35f;
            this.baseEarlyExitTime = 0.2f;
            this.hitStopDuration = 0.025f;
            this.attackRecoil = 1.25f;
            this.hitHopVelocity = 6f;

            this.swingSoundString = "HenrySwordSwing";
            this.hitSoundString = "";
            this.muzzleString = "SwingCenter";
            this.swingEffectPrefab = Modules.Assets.spearSwingEffect;
            this.hitEffectPrefab = Modules.Assets.swordHitImpactEffect;

            this.impactSound = Modules.Assets.swordHitSoundEvent.index;

            base.OnEnter();
            base.characterBody.isSprinting = false;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (this.stopwatch <= (this.duration * this.attackEndTime)) base.characterBody.isSprinting = false;
        }

        protected override void PlayAttackAnimation()
        {
            base.PlayCrossfade("Gesture, Override", "Stab", "Stab.playbackRate", this.duration, 0.05f);
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
            this.outer.SetNextState(new SpearStab());
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}