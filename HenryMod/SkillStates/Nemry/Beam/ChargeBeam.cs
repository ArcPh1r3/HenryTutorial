using EntityStates;
using HenryMod.SkillStates.BaseStates;
using RoR2;
using UnityEngine;

namespace HenryMod.SkillStates.Nemry.Beam
{
    public class ChargeBeam : BaseNemrySkillState
    {
        public static float baseChargeDuration = 3.4f;

        private float chargeDuration;
        private bool finishedCharge;
        private ChildLocator childLocator;
        private Animator animator;
        private Transform modelBaseTransform;
        private uint chargePlayID;
        private GameObject chargeEffectInstance;
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

            //base.PlayAnimation("FullBody, Override", "Charge", "Charge.playbackRate", this.chargeDuration);
            this.chargePlayID = Util.PlaySound("NemryChargeBeam", base.gameObject);

            Transform muzzleTransform = base.FindModelChild("Muzzle");
            if (muzzleTransform)
            {
                this.chargeEffectInstance = UnityEngine.Object.Instantiate<GameObject>(EntityStates.LemurianBruiserMonster.ChargeMegaFireball.chargeEffectPrefab, muzzleTransform.position, muzzleTransform.rotation);
                this.chargeEffectInstance.transform.parent = muzzleTransform;
                this.chargeEffectInstance.transform.localScale *= 0.5f;
                this.chargeEffectInstance.GetComponent<ScaleParticleSystemDuration>().newDuration = this.chargeDuration;

                this.chargeEffectInstance.transform.Find("FlameBillboards, Local").gameObject.SetActive(false);
                this.chargeEffectInstance.transform.Find("SmokeBillboard").gameObject.SetActive(false);
            }
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
            }

            if (base.isAuthority && (base.fixedAge >= 1.25f * this.chargeDuration || !base.inputBank.skill2.down && base.fixedAge >= this.chargeDuration))
            {
                this.outer.SetNextState(new FireBeam());
            }
        }

        protected float CalcCharge()
        {
            return Mathf.Clamp01(base.fixedAge / this.chargeDuration);
        }

        public override void OnExit()
        {
            base.OnExit();

            AkSoundEngine.StopPlayingID(this.chargePlayID);
            if (this.chargeEffectInstance) EntityState.Destroy(this.chargeEffectInstance);

            if (base.cameraTargetParams) base.cameraTargetParams.aimMode = CameraTargetParams.AimType.Aura;
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }
    }
}