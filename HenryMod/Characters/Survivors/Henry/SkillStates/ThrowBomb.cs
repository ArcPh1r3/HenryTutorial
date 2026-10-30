using EntityStates;
using HenryMod.Survivors.Henry;
using RoR2;
using RoR2.Projectile;

namespace HenryMod.Survivors.Henry.SkillStates
{
    //GenericProjectileBaseState is mainly useful for primaries, where the projectile being used is generic and repeatable.
    //in other cases, it is likely useful to know how to create your own projectile. see ThrowBombSimple
    public class ThrowBomb : GenericProjectileBaseState
    {
        public static float BaseDuration = 0.65f;
        //delays for projectiles feel absolute ass so only do this if you know what you're doing, otherwise it's best to keep it at 0
        public static float BaseDelayDuration = 0.0f;

        public static float DamageCoefficient = 16f;

        public override void OnEnter()
        {
            projectilePrefab = HenryContent.CharacterAssets.bombProjectilePrefab;
            //base.effectPrefab = Modules.Assets.SomeMuzzleEffect;
            //targetmuzzle = "muzzleThrow"

            attackSoundString = "HenryBombThrow";

            baseDuration = BaseDuration;
            baseDelayBeforeFiringProjectile = BaseDelayDuration;

            damageCoefficient = DamageCoefficient;
            //proc coefficient is set on the components of the projectile prefab
            force = 80f;

            //when your projectile has gravity. it is useful to give it a bit of an bump upwards in aim angle, so that it doesn't immediately start dropping, and you'll hit your crosshair at more ranges.
            //base.projectilePitchBonus = 0;

            //if you're throwing multiple or want to have random spread, use these
            //base.minSpread = 0;
            //base.maxSpread = 0
            recoilAmplitude = 0.1f;
            bloom = 10;

            base.OnEnter();
        }

        public override void ModifyProjectileInfo(ref FireProjectileInfo fireProjectileInfo)
        {
            base.ModifyProjectileInfo(ref fireProjectileInfo);
            fireProjectileInfo.damageTypeOverride = DamageTypeCombo.GenericSpecial;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }

        public override void PlayAnimation(float duration)
        {

            if (GetModelAnimator())
            {
                PlayAnimation("Gesture, Override", "ThrowBomb", "ThrowBomb.playbackRate", this.duration);
            }
        }
    }
}