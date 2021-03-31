using EntityStates;
using HenryMod.SkillStates.BaseStates;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace HenryMod.SkillStates.Nemry.Beam
{
    public class FireBeam : BaseNemrySkillState
    {
        public static float damageCoefficient = 0.4f;
        public static float force = 1000f;
        public static float minSpread = 0f;
        public static float maxSpread = 0f;
        public static float bulletRadius = 4f;
        public static uint bulletCount = 1;
        public static float fireFrequency = 0.4f;
        public static float maxDistance = 256f;
        public static float procCoefficientPerTick = 0.25f;

        private float fireStopwatch;
        private float stopwatch;
        private Ray aimRay;
        private Transform modelTransform;
        private GameObject beamEffectInstance;
        private ChildLocator laserChildLocator;
        private ChildLocator childLocator;
        protected Transform muzzleTransform;

        public override void OnEnter()
        {
            base.OnEnter();
            this.aimRay = base.GetAimRay();
            base.characterBody.SetAimTimer(15f);
            this.aimRay = base.GetAimRay();
            this.childLocator = base.GetModelChildLocator();
            this.modelTransform = base.GetModelTransform();

            if (this.childLocator)
            {
                this.muzzleTransform = this.childLocator.FindChild("Muzzle");
                if (this.muzzleTransform)
                {
                    /*this.laserEffect = UnityEngine.Object.Instantiate<GameObject>(, this.muzzleTransform.position, this.muzzleTransform.rotation);
                    this.laserEffect.transform.parent = this.muzzleTransform;
                    this.laserChildLocator = this.laserEffect.GetComponent<ChildLocator>();
                    this.laserEffectEnd = this.laserChildLocator.FindChild("LaserEnd");*/
                }
            }

            Util.PlaySound("NemryFireBeam", base.gameObject);
        }

        public override void OnExit()
        {
            if (this.beamEffectInstance) EntityState.Destroy(this.beamEffectInstance);
            base.characterBody.SetAimTimer(0.2f);

            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            this.fireStopwatch += Time.fixedDeltaTime;
            this.stopwatch += Time.fixedDeltaTime;
            base.inputBank.aimDirection = this.aimRay.direction;

            bool fired = false;
            if (this.fireStopwatch > this.attackSpeedStat / FireBeam.fireFrequency)
            {
                this.FireBullet(this.modelTransform, this.aimRay, "Muzzle", FireBeam.maxDistance);
                this.fireStopwatch -= 1f / FireBeam.fireFrequency;
                fired = this.SpendEnergy(5f, SkillSlot.Special);
            }

            if (base.isAuthority && !fired)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }

        private void FireBullet(Transform modelTransform, Ray aimRay, string targetMuzzle, float maxDistance)
        {
            /*if (this.effectPrefab)
            {
                EffectManager.SimpleMuzzleFlash(this.effectPrefab, base.gameObject, targetMuzzle, false);
            }*/
            if (base.isAuthority)
            {
                new BulletAttack
                {
                    owner = base.gameObject,
                    weapon = base.gameObject,
                    origin = aimRay.origin,
                    aimVector = aimRay.direction,
                    minSpread = FireBeam.minSpread,
                    maxSpread = FireBeam.maxSpread,
                    bulletCount = FireBeam.bulletCount,
                    damage = FireBeam.damageCoefficient * this.damageStat,
                    force = FireBeam.force,
                    muzzleName = targetMuzzle,
                    hitEffectPrefab = null,
                    isCrit = base.RollCrit(),
                    procCoefficient = FireBeam.procCoefficientPerTick,
                    HitEffectNormal = false,
                    radius = FireBeam.bulletRadius,
                    maxDistance = maxDistance
                }.Fire();
            }
        }
    }
}