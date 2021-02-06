using HenryMod.SkillStates.BaseStates;
using RoR2;
using UnityEngine;

namespace HenryMod.SkillStates
{
    public class SlashCombo : BaseMeleeAttack
    {
        public override void OnEnter()
        {
            this.hitboxName = "Sword";

            this.damageType = DamageType.Generic;
            this.damageCoefficient = 3.5f;
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

            this.swingSoundString = EntityStates.Merc.GroundLight.comboAttackSoundString;
            this.hitSoundString = EntityStates.Merc.GroundLight.hitSoundString;
            this.muzzleString = "SwingCenter";
            this.swingEffectPrefab = EntityStates.Merc.GroundLight.comboSwingEffectPrefab;
            this.hitEffectPrefab = EntityStates.Merc.GroundLight.comboHitEffectPrefab;

            base.OnEnter();
        }

        protected override void PlayAttackAnimation()
        {
            base.PlayAttackAnimation();
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
            this.outer.SetNextState(new SlashCombo());
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}