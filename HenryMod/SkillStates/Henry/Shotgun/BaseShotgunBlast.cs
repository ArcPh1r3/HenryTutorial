using EntityStates;
using HenryMod.SkillStates.BaseStates;
using RoR2;
using UnityEngine;

namespace HenryMod.SkillStates.Henry.Shotgun
{
    public class BaseShotgunBlast : BaseHenrySkillState
    {
        public static float baseDuration = 0.5f;
        public static uint bulletCount = Modules.StaticValues.shotgunBulletCount;
        public static float damageCoefficient = Modules.StaticValues.shotgunDamageCoefficient;
        public static float procCoefficient = Modules.StaticValues.shotgunProcCoefficient;
        public static float bulletForce = 150f;
        public static float bulletRange = 128f;
        public static float basePushForce = 36f;
        public static float recoil = 2f;

        protected string animString;
        protected Vector3 aimDirection;
        protected Vector3 pushForce;

        private bool hasFired;
        private float fireTime;

        public override void OnEnter()
        {
            base.OnEnter();
            this.fireTime = BaseShotgunBlast.baseDuration * 0.3f;
            base.StartAimMode(0.5f, true);

            base.characterBody.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
        }

        protected void Fire()
        {
            this.hasFired = true;
            base.PlayAnimation("FullBody, Override", this.animString);

            base.characterBody.AddSpreadBloom(2f);
            EffectManager.SimpleMuzzleFlash(EntityStates.Commando.CommandoWeapon.FirePistol2.muzzleEffectPrefab, base.gameObject, "ShotgunMuzzle", false);
            Util.PlaySound("HenryShootShotgun", base.gameObject);

            if (base.isAuthority)
            {
                base.characterMotor.velocity = this.pushForce;
                //base.characterMotor.rootMotion = this.pushForce;
                base.AddRecoil(-1f * BaseShotgunBlast.recoil, -2f * BaseShotgunBlast.recoil, -0.5f * BaseShotgunBlast.recoil, 0.5f * BaseShotgunBlast.recoil);

                new BulletAttack
                {
                    aimVector = this.aimDirection,
                    origin = base.GetAimRay().origin,
                    damage = BaseShotgunBlast.damageCoefficient * this.damageStat,
                    damageColorIndex = DamageColorIndex.Default,
                    damageType = DamageType.Generic,
                    falloffModel = BulletAttack.FalloffModel.DefaultBullet,
                    maxDistance = BaseShotgunBlast.bulletRange,
                    force = BaseShotgunBlast.bulletForce,
                    hitMask = LayerIndex.CommonMasks.bullet,
                    isCrit = base.RollCrit(),
                    owner = base.gameObject,
                    muzzleName = "ShotgunMuzzle",
                    smartCollision = false,
                    procChainMask = default(ProcChainMask),
                    procCoefficient = BaseShotgunBlast.procCoefficient,
                    radius = 0.85f,
                    sniper = false,
                    stopperMask = LayerIndex.CommonMasks.bullet,
                    weapon = null,
                    tracerEffectPrefab = Modules.Assets.shotgunTracer,
                    spreadPitchScale = 0.5f,
                    spreadYawScale = 0.5f,
                    queryTriggerInteraction = QueryTriggerInteraction.UseGlobal,
                    hitEffectPrefab = EntityStates.ClayBruiser.Weapon.MinigunFire.bulletHitEffectPrefab,
                    HitEffectNormal = EntityStates.ClayBruiser.Weapon.MinigunFire.bulletHitEffectNormal,
                    bulletCount = BaseShotgunBlast.bulletCount,
                    maxSpread = 35f,
                    minSpread = 0f
                }.Fire();
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.fixedAge >= this.fireTime && !this.hasFired) this.Fire();

            if (base.isAuthority && base.fixedAge >= BaseShotgunBlast.baseDuration)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override void OnExit()
        {
            base.OnExit();

            base.characterBody.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}