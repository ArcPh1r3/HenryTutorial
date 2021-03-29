using EntityStates;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace HenryMod.SkillStates.Nemry
{
    public class VoidBlast : BaseSkillState
    {
        public static float damageCoefficient = 4.5f;
        public static float procCoefficient = 1f;
        public static float baseDuration = 0.8f;
        public static float throwForce = 80f;

        private float duration;
        private float fireTime;
        private bool hasFired;
        private Animator animator;

        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = ThrowBomb.baseDuration / this.attackSpeedStat;
            this.fireTime = 0.35f * this.duration;
            base.characterBody.SetAimTimer(2f);
            this.animator = base.GetModelAnimator();

            base.PlayAnimation("Gesture, Override", "VoidBlast", "VoidBlast.playbackRate", this.duration);
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
                //EffectManager.SimpleMuzzleFlash(EntityStates.Commando.CommandoWeapon.FirePistol.effectPrefab, base.gameObject, this.muzzleString, false);
                Util.PlaySound("HenryBombThrow", base.gameObject);

                if (base.isAuthority)
                {
                    Ray aimRay = base.GetAimRay();

                    ProjectileManager.instance.FireProjectile(Modules.Projectiles.voidBlastPrefab,
                        aimRay.origin,
                        Util.QuaternionSafeLookRotation(aimRay.direction),
                        base.gameObject,
                        VoidBlast.damageCoefficient * this.damageStat,
                        -2000f,
                        base.RollCrit(),
                        DamageColorIndex.Default,
                        null,
                        VoidBlast.throwForce);
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
            return InterruptPriority.PrioritySkill;
        }
    }
}