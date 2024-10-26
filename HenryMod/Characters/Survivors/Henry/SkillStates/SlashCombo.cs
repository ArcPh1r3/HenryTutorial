using HenryMod.Characters.Survivors.Henry.Content;
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
            hitboxGroupName = "SwordGroup";

            damageType = DamageType.Generic;
            damageCoefficient = HenryContent.swordDamageCoefficient;
            procCoefficient = 1f;
            pushForce = 300f;
            bonusForce = Vector3.zero;
            baseDuration = 1f;

            //0-1 multiplier of baseduration, used to time when the hitbox is out (usually based on the run time of the animation)
            //for example, if attackStartPercentTime is 0.5, the attack will start hitting halfway through the ability. if baseduration is 3 seconds, the attack will start happening at 1.5 seconds
            attackStartPercentTime = 0.2f;
            attackEndPercentTime = 0.4f;

            //this is the point at which the attack can be interrupted by itself, continuing a combo
            earlyExitPercentTime = 0.6f;

            hitStopDuration = 0.012f;
            attackRecoil = 0.5f;
            hitHopVelocity = 4f;

            swingSoundString = "HenrySwordSwing";
            hitSoundString = "";
            muzzleString = swingIndex % 2 == 0 ? "SwingLeft" : "SwingRight";
            playbackRateParam = "Slash.playbackRate";
            swingEffectPrefab = HenryAssets.swordSwingEffect;
            hitEffectPrefab = HenryAssets.swordHitImpactEffect;

            impactSound = HenryAssets.swordHitSoundEvent.index;

            base.OnEnter();
        }

        protected override void PlayAttackAnimation()
        {
            //play a adifferent animation based on what step of the combo you are currently in.
            switch (swingIndex)
            {
                case 0:
                    PlayCrossfade("Gesture, Override", "Slash1", playbackRateParam, duration, 0.1f * duration);
                    break;
                case 1:
                    PlayCrossfade("Gesture, Override", "Slash1", playbackRateParam, duration, 0.1f * duration);
                    break;
            }
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

        //bit advanced so don't worry about this, it's for networking. all you need to know is you have to do this for steppedskilldefs
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