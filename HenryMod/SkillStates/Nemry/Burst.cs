using EntityStates;
using HenryMod.SkillStates.BaseStates;
using RoR2;
using UnityEngine;

namespace HenryMod.SkillStates.Nemry
{
    public class Burst : BaseNemrySkillState
    {
        public static float baseDuration = 0.2f;
        public static float damageCoefficient = 6.8f;
        public static float procCoefficient = 1f;
        public static float blastForce = 2500f;
        public static float pushForce = 100f;
        public static float recoil = 5f;

        protected Vector3 aimDirection;

        public override void OnEnter()
        {
            base.OnEnter();
            base.StartAimMode(0.5f, true);

            base.characterBody.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;

            this.Fire();
            this.SpendEnergy(20, SkillSlot.Utility);
            base.PlayAnimation("Gesture, Override", "ShootGun", "ShootGun.playbackRate", Burst.baseDuration);
        }

        protected void Fire()
        {
            Ray aimRay = base.GetAimRay();
            this.aimDirection = aimRay.direction;

            //base.PlayAnimation("FullBody, Override", this.animString);

            base.characterBody.AddSpreadBloom(2f);
            EffectManager.SimpleMuzzleFlash(EntityStates.Commando.CommandoWeapon.FirePistol2.muzzleEffectPrefab, base.gameObject, "Muzzle", false);
            Util.PlaySound("HenryShootShotgun", base.gameObject);

            if (base.isAuthority)
            {
                base.characterMotor.velocity = -this.aimDirection * Burst.pushForce;
                //base.characterMotor.rootMotion = this.pushForce;
                base.AddRecoil(-1f * Burst.recoil, -2f * Burst.recoil, -0.5f * Burst.recoil, 0.5f * Burst.recoil);

                BlastAttack blastAttack = new BlastAttack();
                blastAttack.radius = 10f;
                blastAttack.procCoefficient = 1f;
                blastAttack.position = aimRay.origin + 2 * aimRay.direction;
                blastAttack.attacker = base.gameObject;
                blastAttack.crit = base.RollCrit();
                blastAttack.baseDamage = base.characterBody.damage * Burst.damageCoefficient;
                blastAttack.falloffModel = BlastAttack.FalloffModel.None;
                blastAttack.baseForce = Burst.blastForce;
                blastAttack.teamIndex = TeamComponent.GetObjectTeam(blastAttack.attacker);
                blastAttack.damageType = DamageType.Stun1s;
                blastAttack.attackerFiltering = AttackerFiltering.NeverHit;
                BlastAttack.Result result = blastAttack.Fire();

                EffectData effectData = new EffectData();
                effectData.origin = aimRay.origin + 2 * aimRay.direction;
                effectData.scale = 8;

                EffectManager.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/FusionCellExplosion"), effectData, true);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.isAuthority && base.fixedAge >= Burst.baseDuration)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override void OnExit()
        {
            base.OnExit();

            base.characterMotor.velocity *= 0.2f;

            base.characterBody.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}