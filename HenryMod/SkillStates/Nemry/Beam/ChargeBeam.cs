using EntityStates;
using HenryMod.SkillStates.BaseStates;
using RoR2;
using UnityEngine;

namespace HenryMod.SkillStates.Nemry.Beam
{
    public class ChargeBeam : BaseNemrySkillState
    {
        public static float baseChargeDuration = 2f;

        private float chargeDuration;
        private bool finishedCharge;
        private ChildLocator childLocator;
        private Animator animator;
        private Transform modelBaseTransform;
        //private uint chargePlayID;
        //private ParticleSystem swordVFX;
        private bool zoomin;

        public override void OnEnter()
        {
            base.OnEnter();
            this.chargeDuration = ChargeBeam.baseChargeDuration / this.attackSpeedStat;
            this.childLocator = base.GetModelChildLocator();
            this.modelBaseTransform = base.GetModelBaseTransform();
            this.animator = base.GetModelAnimator();
            this.zoomin = false;
            base.characterBody.hideCrosshair = true;

            foreach (EntityStateMachine i in base.gameObject.GetComponents<EntityStateMachine>())
            {
                if (i)
                {
                    if (i.customName == "Weapon" || i.customName == "Slide")
                    {
                        i.SetNextStateToMain();
                    }
                }
            }

            this.SpendEnergy(100f, SkillSlot.Special);

            //base.PlayAnimation("FullBody, Override", "Charge", "Charge.playbackRate", this.chargeDuration);

            if (base.cameraTargetParams) base.cameraTargetParams.aimMode = CameraTargetParams.AimType.OverTheShoulder;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            base.characterMotor.velocity = Vector3.zero;
            float charge = this.CalcCharge();

            if (charge >= 0.6f && !this.zoomin)
            {
                this.zoomin = true;
                if (base.cameraTargetParams) base.cameraTargetParams.aimMode = CameraTargetParams.AimType.Aura;
            }

            if (charge >= 1f && !this.finishedCharge)
            {
                this.finishedCharge = true;

                //AkSoundEngine.StopPlayingID(this.chargePlayID);
                //Util.PlaySound("NemmandoDecisiveStrikeReady", base.gameObject);

                if (base.cameraTargetParams) base.cameraTargetParams.aimMode = CameraTargetParams.AimType.Aura;
            }

            if (base.isAuthority && base.inputBank.skill3.down && base.skillLocator.utility.CanExecute())
            {
                base.skillLocator.utility.ExecuteIfReady();
            }

            if (base.isAuthority && (base.fixedAge >= 1.25f * this.chargeDuration || !base.inputBank.skill2.down && base.fixedAge >= this.chargeDuration))
            {
                //ChargeRelease nextState = new ChargeRelease();
                //nextState.charge = charge;
                //this.outer.SetNextState(nextState);
            }
        }

        protected float CalcCharge()
        {
            return Mathf.Clamp01(base.fixedAge / this.chargeDuration);
        }

        public override void OnExit()
        {
            base.OnExit();

            base.PlayAnimation("FullBody, Override", "BufferEmpty");
            //AkSoundEngine.StopPlayingID(this.chargePlayID);

            if (base.cameraTargetParams) base.cameraTargetParams.aimMode = CameraTargetParams.AimType.Aura;
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }
    }
}