using EntityStates;
using HenryMod.Modules.Components;
using HenryMod.SkillStates.BaseStates;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace HenryMod.SkillStates.Bazooka.Scepter
{
    public class BazookaCharge : BaseHenrySkillState
    {
        public static float baseChargeDuration = 1.6f;
        public static float minChargeDuration = 0.2f;
        public static float minBloomRadius = 0.1f;
        public static float maxBloomRadius = 2f;

        private float duration;
        private uint chargePlayID;
        private GameObject chargeEffectInstance;
        private GameObject secondaryChargeEffectInstance;

        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = BazookaCharge.baseChargeDuration / this.attackSpeedStat;

            this.chargePlayID = Util.PlayAttackSpeedSound("HenryBazookaCharge", base.gameObject, this.attackSpeedStat);

            Transform muzzleTransform = base.FindModelChild("BazookaMuzzle");
            if (muzzleTransform)
            {
                this.chargeEffectInstance = UnityEngine.Object.Instantiate<GameObject>(EntityStates.LemurianBruiserMonster.ChargeMegaFireball.chargeEffectPrefab, muzzleTransform.position, muzzleTransform.rotation);
                this.chargeEffectInstance.transform.parent = muzzleTransform;
                this.chargeEffectInstance.transform.localScale *= 0.5f;
                this.chargeEffectInstance.GetComponent<ScaleParticleSystemDuration>().newDuration = this.duration;

                this.chargeEffectInstance.transform.Find("FlameBillboards, Local").gameObject.SetActive(false);
                this.chargeEffectInstance.transform.Find("SmokeBillboard").gameObject.SetActive(false);

                this.secondaryChargeEffectInstance = UnityEngine.Object.Instantiate<GameObject>(EntityStates.LemurianBruiserMonster.ChargeMegaFireball.chargeEffectPrefab, muzzleTransform.position, muzzleTransform.rotation);
                this.secondaryChargeEffectInstance.transform.parent = base.FindModelChild("BazookaMuzzleScepter");
                this.secondaryChargeEffectInstance.transform.localScale *= 0.5f;
                this.secondaryChargeEffectInstance.GetComponent<ScaleParticleSystemDuration>().newDuration = this.duration;

                this.secondaryChargeEffectInstance.transform.Find("FlameBillboards, Local").gameObject.SetActive(false);
                this.secondaryChargeEffectInstance.transform.Find("SmokeBillboard").gameObject.SetActive(false);
            }
        }

        private float CalcCharge()
        {
            return Mathf.Clamp01(base.fixedAge / this.duration);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            float charge = this.CalcCharge();

            if (base.isAuthority && ((!base.IsKeyDownAuthority() && base.fixedAge >= BazookaCharge.minChargeDuration) || base.fixedAge >= this.duration))
            {
                BazookaFire nextState = new BazookaFire()
                {
                    charge = charge
                };
                this.outer.SetNextState(nextState);
                return;
            }
        }

        public override void OnExit()
        {
            base.OnExit();

            AkSoundEngine.StopPlayingID(this.chargePlayID);

            if (this.chargeEffectInstance) EntityState.Destroy(this.chargeEffectInstance);
            if (this.secondaryChargeEffectInstance) EntityState.Destroy(this.secondaryChargeEffectInstance);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }
    }
}