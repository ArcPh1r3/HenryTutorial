using EntityStates;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace HenryMod.Survivors.Henry.SkillStates
{
    //simplest form of throwing a projectile. From here, you can change any and all aspects of these as you see fit.
    //If your projectile is a primary, has gravity, or wants to throw a spread, GenericProjectileBaseState has some useful features that you might like
        //see ThrowBombGeneric for an example
    public class ThrowBombSimple : BaseSkillState
    {
        public static float BaseDuration = 0.65f;
        public static float DamageCoefficient = 16;

        private float duration;

        public override void OnEnter()
        {
            base.OnEnter();

            duration = BaseDuration / attackSpeedStat;

            PlayAnimation("Gesture, Override", "ThrowBomb", "ThrowBomb.playbackRate", duration);

            Ray aimray = GetAimRay();

            FireProjectileInfo projectileInfo = new FireProjectileInfo
            {
                projectilePrefab = HenryContent.CharacterAssets.bombProjectilePrefab,
                position = aimray.origin,
                rotation = Util.QuaternionSafeLookRotation(aimray.direction),
                owner = gameObject,
                damage = damageStat * DamageCoefficient,
                force = 100f,
                crit = RollCrit(),
                damageTypeOverride = DamageTypeCombo.GenericSpecial,
                //optional:
                target = null, //used for certain projectile components. 
                useSpeedOverride = true, //as name implies overrides the speed usually used by ProjectileSimple component.
                speedOverride = 100, //only works if useSpeedOverride is true
            };
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(isAuthority && fixedAge > duration)
            {
                outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}