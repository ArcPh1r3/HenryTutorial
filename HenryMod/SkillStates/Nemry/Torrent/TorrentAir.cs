using EntityStates;
using HenryMod.Modules.Components;
using HenryMod.SkillStates.BaseStates;
using RoR2;
using UnityEngine;

namespace HenryMod.SkillStates.Nemry.Torrent
{
    public class TorrentAir : BaseNemrySkillState
    {
        public static GameObject bulletTracerEffectPrefab = Modules.Assets.energyTracer;

        public static float damageCoefficient = 2.1f;
        public static float baseFireInterval = 0.09f;
        public static float bulletRecoil = 0.5f;
        public static float hopHeight = 22f;
        public static float procCoefficient = 0.9f;
        public static float bulletForce = 150f;
        public static float baseStartDuration = 0.5f;

        private float startDuration;
        private float fireInterval;
        private float fireStopwatch;
        private string muzzleString;
        private int storedJumpCount;

        public override void OnEnter()
        {
            this.storedJumpCount = base.characterMotor.jumpCount;
            base.OnEnter();
            base.characterBody.SetAimTimer(2f);
            this.fireInterval = TorrentAir.baseFireInterval / this.attackSpeedStat;
            this.startDuration = TorrentAir.baseStartDuration / this.attackSpeedStat;
            this.muzzleString = "Muzzle";

            base.characterMotor.velocity *= 0.25f;
            base.SmallHop(base.characterMotor, TorrentAir.hopHeight);

            base.PlayAnimation("FullBody, Override", "AirTorrent", "Torrent.playbackRate", this.startDuration);
            //Util.PlaySound("AatroxRainstorm", base.gameObject);
        }

        public override void OnExit()
        {
            base.OnExit();

            if (base.characterMotor.isGrounded)
            {
                base.PlayAnimation("FullBody, Override", "BufferEmpty");
            }
            else
            {
                base.PlayAnimation("FullBody, Override", "AirTorrentExit", "Torrent.playbackRate", 0.4f / this.attackSpeedStat);
            }
        }

        private void FireBullet()
        {
            base.AddRecoil(-2f * TorrentAir.bulletRecoil, -3f * TorrentAir.bulletRecoil, -1f * TorrentAir.bulletRecoil, 1f * TorrentAir.bulletRecoil);
            base.characterBody.AddSpreadBloom(0.33f * TorrentAir.bulletRecoil);

            EffectManager.SimpleMuzzleFlash(EntityStates.Engi.EngiWeapon.FireGrenades.effectPrefab, base.gameObject, this.muzzleString, false);
            
            Util.PlaySound("NemryShootEnergy", base.gameObject);

            if (base.fixedAge >= this.startDuration) base.PlayAnimation("FullBody, Override", "AirTorrentLoop", "Torrent.playbackRate", this.fireInterval);

            if (base.isAuthority)
            {
                float damage = TorrentAir.damageCoefficient * this.damageStat;

                Vector3 bulletPosition = base.GetModelChildLocator().FindChild(this.muzzleString).position;
                Vector3 aimVector = base.GetModelChildLocator().FindChild(this.muzzleString).forward;

                if (base.fixedAge >= this.startDuration)
                {
                    aimVector = Vector3.down;
                }

                Ray aimRay = base.GetAimRay();
                new BulletAttack
                {
                    bulletCount = 1U,
                    aimVector = aimVector,
                    origin = bulletPosition,
                    damage = damage,
                    damageColorIndex = DamageColorIndex.Default,
                    damageType = DamageType.Generic,
                    falloffModel = BulletAttack.FalloffModel.None,
                    maxDistance = 256f,
                    force = TorrentAir.bulletForce,
                    hitMask = LayerIndex.CommonMasks.bullet,
                    minSpread = 0f,
                    maxSpread = 5f,
                    isCrit = base.RollCrit(),
                    owner = base.gameObject,
                    muzzleName = this.muzzleString,
                    smartCollision = false,
                    procChainMask = default(ProcChainMask),
                    procCoefficient = procCoefficient,
                    radius = 0.5f,
                    sniper = false,
                    stopperMask = LayerIndex.CommonMasks.bullet,
                    weapon = null,
                    tracerEffectPrefab = TorrentAir.bulletTracerEffectPrefab,
                    spreadPitchScale = 0.25f,
                    spreadYawScale = 0.25f,
                    queryTriggerInteraction = QueryTriggerInteraction.UseGlobal,
                    hitEffectPrefab = EntityStates.ClayBruiser.Weapon.MinigunFire.bulletHitEffectPrefab,
                    HitEffectNormal = EntityStates.ClayBruiser.Weapon.MinigunFire.bulletHitEffectNormal
                }.Fire();
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            base.StartAimMode(0.2f, false);
            this.fireStopwatch -= Time.fixedDeltaTime;
            base.characterBody.isSprinting = false;
            base.characterMotor.jumpCount = base.characterBody.maxJumpCount;

            if (this.fireStopwatch <= 0f)
            {
                if (this.SpendEnergy(1))
                {
                    if (base.fixedAge >= this.startDuration) this.fireStopwatch = this.fireInterval;
                    else this.fireStopwatch = 0.5f * this.fireInterval;
                    this.FireBullet();
                }
                else
                {
                    this.outer.SetNextStateToMain();
                    return;
                }
            }

            if (base.characterMotor)
            {
                if (base.characterMotor.velocity.y < -5f)
                {
                    base.characterMotor.velocity.y = -5f;
                }
            }

            if (!base.inputBank.skill2.down && base.fixedAge >= 10f * this.fireInterval && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }

            if (base.characterMotor.isGrounded && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }
    }
}