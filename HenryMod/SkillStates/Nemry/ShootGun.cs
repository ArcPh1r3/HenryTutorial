using EntityStates;
using HenryMod.SkillStates.BaseStates;
using RoR2;
using UnityEngine;

namespace HenryMod.SkillStates.Nemry
{
    public class ShootGun : BaseNemrySkillState
    {
        public static float damageCoefficient = 0.9f;
        public static float boostedDamageCoefficient = 2.5f;
        public static float procCoefficient = 1f;
        public static float baseDuration = 0.6f;
        public static float force = 200f;
        public static float recoil = 1.5f;
        public static float range = 256f;
        public static float energyCost = 5f;
        public static GameObject tracerEffectPrefab = Resources.Load<GameObject>("Prefabs/Effects/Tracers/TracerEngiTurret");

        private float duration;
        private float fireTime;
        private bool hasFired;
        private string muzzleString;
        private bool isBoosted;

        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = Shoot.baseDuration / this.attackSpeedStat;
            this.fireTime = 0.2f * this.duration;
            base.characterBody.SetAimTimer(2f);
            this.muzzleString = "Muzzle";
            this.isBoosted = false;

            if (this.energyComponent.SpendEnergy(ShootGun.energyCost, SkillSlot.Primary))
            {
                this.isBoosted = true;
            }

            base.PlayAnimation("Gesture, Override", "ShootGun", "ShootGun.playbackRate", 1.5f * this.duration);
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        private void Fire()
        {
            if (!this.hasFired)
            {
                this.hasFired = true;

                base.characterBody.AddSpreadBloom(1.5f);
                EffectManager.SimpleMuzzleFlash(Modules.Assets.muzzleFlashEnergy, base.gameObject, this.muzzleString, false);

                string soundString = "NemryShootGun";
                if (this.isBoosted) soundString = "NemryShootEnergy";
                Util.PlaySound(soundString, base.gameObject);

                if (base.isAuthority)
                {
                    Ray aimRay = base.GetAimRay();
                    base.AddRecoil(-1f * ShootGun.recoil, -2f * ShootGun.recoil, -0.5f * ShootGun.recoil, 0.5f * ShootGun.recoil);

                    LayerMask bulletStopperMask = LayerIndex.CommonMasks.bullet;
                    float damageCoeff = ShootGun.damageCoefficient;
                    BulletAttack.FalloffModel falloffModel = BulletAttack.FalloffModel.DefaultBullet;
                    GameObject tracerPrefab = ShootGun.tracerEffectPrefab;

                    if (this.isBoosted)
                    {
                        damageCoeff = ShootGun.boostedDamageCoefficient;
                        bulletStopperMask = LayerIndex.world.mask;
                        falloffModel = BulletAttack.FalloffModel.None;
                        tracerPrefab = Modules.Assets.energyTracer;
                    }

                    new BulletAttack
                    {
                        bulletCount = 1,
                        aimVector = aimRay.direction,
                        origin = aimRay.origin,
                        damage = damageCoeff * this.damageStat,
                        damageColorIndex = DamageColorIndex.Default,
                        damageType = DamageType.Generic,
                        falloffModel = falloffModel,
                        maxDistance = ShootGun.range,
                        force = ShootGun.force,
                        hitMask = LayerIndex.CommonMasks.bullet,
                        minSpread = 0f,
                        maxSpread = 0f,
                        isCrit = base.RollCrit(),
                        owner = base.gameObject,
                        muzzleName = muzzleString,
                        smartCollision = false,
                        procChainMask = default(ProcChainMask),
                        procCoefficient = procCoefficient,
                        radius = 0.75f,
                        sniper = false,
                        stopperMask = bulletStopperMask,
                        weapon = null,
                        tracerEffectPrefab = tracerPrefab,
                        spreadPitchScale = 0f,
                        spreadYawScale = 0f,
                        queryTriggerInteraction = QueryTriggerInteraction.UseGlobal,
                        hitEffectPrefab = EntityStates.Commando.CommandoWeapon.FirePistol2.hitEffectPrefab,
                    }.Fire();
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.fixedAge >= this.fireTime)
            {
                this.Fire();
            }

            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}