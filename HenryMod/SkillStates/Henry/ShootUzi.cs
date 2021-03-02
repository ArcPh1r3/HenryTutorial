using EntityStates;
using RoR2;
using UnityEngine;

namespace HenryMod.SkillStates
{
    public class ShootUzi : BaseSkillState
    {
        public static float damageCoefficient = 0.6f;
        public static float procCoefficient = 0.5f;
        public static float baseDuration = 0.1f;
        public static float force = 100f;
        public static float recoil = 1.5f;
        public static float range = 256f;
        public static GameObject tracerEffectPrefab = Resources.Load<GameObject>("Prefabs/Effects/Tracers/TracerCommandoDefault");

        private float duration;
        private Animator animator;
        private string muzzleString;

        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = ShootUzi.baseDuration / this.attackSpeedStat;
            base.characterBody.SetAimTimer(2f);
            this.animator = base.GetModelAnimator();
            this.muzzleString = "Muzzle";

            base.PlayAnimation("LeftArm, Override", "ShootUzi", "ShootGun.playbackRate", 5f * this.duration);

            this.Fire();
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        private void Fire()
        {
            base.characterBody.AddSpreadBloom(0.5f);
            EffectManager.SimpleMuzzleFlash(EntityStates.Commando.CommandoWeapon.FirePistol.effectPrefab, base.gameObject, this.muzzleString, false);
            Util.PlaySound("HenryShootUzi", base.gameObject);

            if (base.isAuthority)
            {
                Ray aimRay = base.GetAimRay();
                base.AddRecoil(-1f * ShootUzi.recoil, -2f * ShootUzi.recoil, -0.5f * ShootUzi.recoil, 0.5f * ShootUzi.recoil);

                new BulletAttack
                {
                    bulletCount = 1,
                    aimVector = aimRay.direction,
                    origin = aimRay.origin,
                    damage = ShootUzi.damageCoefficient * this.damageStat,
                    damageColorIndex = DamageColorIndex.Default,
                    damageType = DamageType.Generic,
                    falloffModel = BulletAttack.FalloffModel.DefaultBullet,
                    maxDistance = ShootUzi.range,
                    force = ShootUzi.force,
                    hitMask = LayerIndex.CommonMasks.bullet,
                    minSpread = 0f,
                    maxSpread = 8f,
                    isCrit = base.RollCrit(),
                    owner = base.gameObject,
                    muzzleName = muzzleString,
                    smartCollision = false,
                    procChainMask = default(ProcChainMask),
                    procCoefficient = procCoefficient,
                    radius = 0.75f,
                    sniper = false,
                    stopperMask = LayerIndex.CommonMasks.bullet,
                    weapon = null,
                    tracerEffectPrefab = ShootUzi.tracerEffectPrefab,
                    spreadPitchScale = 0.5f,
                    spreadYawScale = 0.5f,
                    queryTriggerInteraction = QueryTriggerInteraction.UseGlobal,
                    hitEffectPrefab = EntityStates.Commando.CommandoWeapon.FirePistol.hitEffectPrefab,
                }.Fire();
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

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