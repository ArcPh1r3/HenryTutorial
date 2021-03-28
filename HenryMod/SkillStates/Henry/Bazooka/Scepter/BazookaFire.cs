using EntityStates;
using HenryMod.Modules.Components;
using HenryMod.SkillStates.BaseStates;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace HenryMod.SkillStates.Bazooka.Scepter
{
    public class BazookaFire : BaseHenrySkillState
    {
        public float charge;

        public static float baseDuration = 0.7f;
        public static float minSpeed = 20f;
        public static float maxSpeed = 160f;
        public static float minDamageCoefficient = 6f;
        public static float maxDamageCoefficient = 14f;
        public static float minRecoil = 0.5f;
        public static float maxRecoil = 5f;

        private float duration;
        private float speed;
        private float damageCoefficient;
        private float recoil;
        private float fireTime;
        private bool hasFired;

        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = BazookaFire.baseDuration / this.attackSpeedStat;
            this.speed = Util.Remap(this.charge, 0f, 1f, BazookaFire.minSpeed, BazookaFire.maxSpeed);
            this.damageCoefficient = Util.Remap(this.charge, 0f, 1f, BazookaFire.minDamageCoefficient, BazookaFire.maxDamageCoefficient);
            this.recoil = Util.Remap(this.charge, 0f, 1f, BazookaFire.minRecoil, BazookaFire.maxRecoil);
            this.fireTime = this.duration * 0.15f;
            this.hasFired = false;

            if (this.charge >= 0.8f) base.PlayAnimation("Bazooka, Override", "BazookaFireChargedScepter", "Bazooka.playbackRate", 0.8f);
            else base.PlayAnimation("Bazooka, Override", "BazookaFireScepter", "Bazooka.playbackRate", 1f);

            this.Fire("BazookaMuzzle");
            this.hasFired = false;
        }

        private void Fire(string muzzleString)
        {
            if (!this.hasFired)
            {
                this.hasFired = true;
                EffectManager.SimpleMuzzleFlash(Modules.Assets.bazookaMuzzleFlash, base.gameObject, muzzleString, false);
                Util.PlaySound("HenryBazookaFire", base.gameObject);

                if (base.isAuthority)
                {
                    base.AddRecoil(-1f * this.recoil, -2f * this.recoil, -0.5f * this.recoil, 0.5f * this.recoil);

                    Ray aimRay = base.GetAimRay();

                    ProjectileManager.instance.FireProjectile(Modules.Projectiles.bazookaRocketPrefab,
                        aimRay.origin,
                        Util.QuaternionSafeLookRotation(aimRay.direction),
                        base.gameObject,
                        this.damageCoefficient * this.damageStat,
                        2000f,
                        base.RollCrit(),
                        DamageColorIndex.Default,
                        null,
                        this.speed);
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.fixedAge >= this.fireTime && !this.hasFired)
            {
                this.Fire("BazookaMuzzleScepter");
            }

            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}