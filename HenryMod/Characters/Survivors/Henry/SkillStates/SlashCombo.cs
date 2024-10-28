using HenryMod.Modules.BaseStates;
using RoR2.Skills;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace HenryMod.Survivors.Henry.SkillStates
{
    public class SlashCombo : BaseMeleeAttack, SteppedSkillDef.IStepSetter
    {
        public int swingIndex;

        //used by the steppedskilldef to increment your combo whenever this state is entered
        public void SetStep(int i)
        {
            swingIndex = i;
        }

        public override void OnEnter()
        {
            //mouse over variables for detailed explanations
            hitBoxGroupName = "SwordGroup";

            damageType = DamageType.Generic;
            damageCoefficient = HenryContent.StaticValues.swordDamageCoefficient;
            procCoefficient = 1f;
            pushForce = 300f;
            bonusForce = Vector3.zero;
            baseDuration = 1f;

            //0-1 multiplier of baseduration, used to time when the hitbox is out (usually based on the run time of the animation)
            attackStartPercentTime = 0.2f;
            attackEndPercentTime = 0.4f;

            earlyExitPercentTime = 0.6f;

            hitStopDuration = 0.012f;
            attackRecoil = 0.5f;
            hitHopVelocity = 4f;

            swingSoundString = "HenrySwordSwing";
            playbackRateParam = "Slash.playbackRate";
            muzzleString = swingIndex == 0 ? "SwingLeft" : "SwingRight";
            swingEffectPrefab = HenryContent.Assets.swordSwingEffect;
            hitEffectPrefab = HenryContent.Assets.swordHitImpactEffect;

            impactSound = HenryContent.Assets.swordHitSoundEvent.index;

            base.OnEnter();

            PlayAttackAnimation();
        }

        protected override void PlayAttackAnimation()
        {
            //play a adifferent animation based on what step of the combo you are currently in.
            if (swingIndex == 0)
            {
                PlayCrossfade("Gesture, Override", "Slash1", playbackRateParam, duration, 0.1f * duration);
            }
            if (swingIndex == 1)
            {
                PlayCrossfade("Gesture, Override", "Slash2", playbackRateParam, duration, 0.1f * duration);
            }
            //as a challenge, see if you can rewrite this code to be one line.
        }

        protected override void PlaySwingEffect()
        {
            base.PlaySwingEffect();
        }

        protected override void OnHitEnemyAuthority()
        {
            base.OnHitEnemyAuthority();
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        //add these functions for steppedskilldefs
        //bit advanced so don't worry about this, it's for networking.
        //long story short this syncs a value from authority (current player) to all other clients, so the swingIndex is the same for all machines
        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(swingIndex);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            swingIndex = reader.ReadInt32();
        }
    }
}