using EntityStates;
using HenryMod.Modules.Components;
using HenryMod.SkillStates.BaseStates;
using RoR2;
using UnityEngine;

namespace HenryMod.SkillStates.Nemry.Torrent
{
    public class TorrentGround : BaseNemrySkillState
    {
        public static GameObject bulletTracerEffectPrefab = Modules.Assets.energyTracer;

        public static float damageCoefficient = 1.2f;
        public static float procCoefficient = 0.8f;
        public static float baseFireInterval = 0.09f;
        public static float bulletRecoil = 0.45f;
        public static float baseStartDuration = 0.4f;
        public static float bulletForce = 50f;

        private float startDuration;
        private float fireInterval;
        private float fireStopwatch;
        private string muzzleString;
        private int startingJumpCount;
        private GameObject spinningWeaponEffect;

        public override void OnEnter()
        {
            base.OnEnter();
            base.characterBody.SetAimTimer(2f);
            this.fireInterval = TorrentGround.baseFireInterval / this.attackSpeedStat;
            this.startDuration = TorrentGround.baseStartDuration / this.attackSpeedStat;
            this.muzzleString = "Muzzle";
            this.startingJumpCount = base.characterMotor.jumpCount;
            this.spinningWeaponEffect = base.FindModelChild("SpinningWeaponEffect").gameObject;

            this.spinningWeaponEffect.SetActive(true);

            base.PlayAnimation("FullBody, Override", "GroundTorrent", "Torrent.playbackRate", this.startDuration);
            Util.PlaySound("HenryBazookaEquip", base.gameObject);
        }

        public override void OnExit()
        {
            base.OnExit();
            base.PlayAnimation("FullBody, Override", "BufferEmpty");
            base.PlayAnimation("Gesture, Override", "GroundTorrentExit", "Torrent.playbackRate", 0.5f);
            Util.PlaySound("HenryBazookaUnequip", base.gameObject);

            base.characterMotor.jumpCount = this.startingJumpCount;
        }

        private void FireBullet()
        {
            this.fireInterval = TorrentGround.baseFireInterval / this.attackSpeedStat;
            base.AddRecoil(-2f * TorrentGround.bulletRecoil, -3f * TorrentGround.bulletRecoil, -1f * TorrentGround.bulletRecoil, 1f * TorrentGround.bulletRecoil);
            base.characterBody.AddSpreadBloom(0.33f * TorrentGround.bulletRecoil);

            EffectManager.SimpleMuzzleFlash(EntityStates.Engi.EngiWeapon.FireGrenades.effectPrefab, base.gameObject, this.muzzleString, false);
            Util.PlaySound("NemryShootEnergy", base.gameObject);
            base.PlayAnimation("FullBody, Override", "GroundTorrentLoop", "Torrent.playbackRate", this.fireInterval);

            bool isAuthority = base.isAuthority;
            if (isAuthority)
            {
                float damage = TorrentGround.damageCoefficient * this.damageStat;
                Ray aimRay = base.GetAimRay();
                new BulletAttack
                {
                    bulletCount = 1U,
                    aimVector = aimRay.direction,
                    origin = aimRay.origin,
                    damage = damage,
                    damageColorIndex = DamageColorIndex.Default,
                    damageType = DamageType.Generic,
                    falloffModel = BulletAttack.FalloffModel.DefaultBullet,
                    maxDistance = 256f,
                    force = TorrentGround.bulletForce,
                    hitMask = LayerIndex.CommonMasks.bullet,
                    minSpread = 0f,
                    maxSpread = 20f,
                    isCrit = base.RollCrit(),
                    owner = base.gameObject,
                    muzzleName = this.muzzleString,
                    smartCollision = false,
                    procChainMask = default(ProcChainMask),
                    procCoefficient = TorrentGround.procCoefficient,
                    radius = 0.5f,
                    sniper = false,
                    stopperMask = LayerIndex.CommonMasks.bullet,
                    weapon = null,
                    tracerEffectPrefab = TorrentGround.bulletTracerEffectPrefab,
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
            base.characterMotor.velocity = Vector3.zero;
            base.characterMotor.moveDirection = Vector3.zero;
            base.characterMotor.jumpCount = base.characterBody.maxJumpCount;

            if (this.fireStopwatch <= 0f && base.fixedAge >= this.startDuration)
            {
                this.spinningWeaponEffect.SetActive(false);

                if (this.SpendEnergy(1))
                {
                    this.fireStopwatch = this.fireInterval;
                    this.FireBullet();
                }
                else
                {
                    this.outer.SetNextStateToMain();
                    return;
                }
            }

            if (!base.inputBank.skill2.down && base.fixedAge >= this.fireInterval && base.isAuthority)
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