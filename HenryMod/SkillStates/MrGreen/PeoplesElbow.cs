using EntityStates;
using RoR2;
using UnityEngine;

namespace HenryMod.SkillStates.MrGreen
{
    public class PeoplesElbow : BaseSkillState
    {
        public static float jumpDuration = 0.8f;
        public static float dropForce = 50f;

        public static float slamRadius = 8f;
        public static float slamDamageCoefficient = 8f;
        public static float slamProcCoefficient = 1f;
        public static float slamForce = 1000f;

        private bool hasDropped;
        private Vector3 flyVector = Vector3.zero;
        private Transform modelTransform;
        private Transform slamIndicatorInstance;
        private Transform slamCenterIndicatorInstance;
        private Ray downRay;

        public override void OnEnter()
        {
            base.OnEnter();
            this.modelTransform = base.GetModelTransform();
            this.flyVector = Vector3.up;
            this.hasDropped = false;
            base.characterMotor.jumpCount = base.characterBody.maxJumpCount;

            //base.PlayAnimation("FullBody, Override", "PeoplesElbow", "HighJump.playbackRate", PeoplesElbow.jumpDuration);
            Util.PlayAttackSpeedSound(EntityStates.Croco.Leap.leapSoundString, base.gameObject, 1.5f);

            base.characterMotor.Motor.ForceUnground();
            base.characterMotor.velocity = Vector3.zero;

            base.characterBody.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;

            base.gameObject.layer = LayerIndex.fakeActor.intVal;
            base.characterMotor.Motor.RebuildCollidableLayers();
        }

        public override void Update()
        {
            base.Update();

            if (this.slamIndicatorInstance) this.UpdateSlamIndicator();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (!this.hasDropped)
            {
                base.characterMotor.rootMotion += this.flyVector * ((1f * this.moveSpeedStat) * EntityStates.Mage.FlyUpState.speedCoefficientCurve.Evaluate(base.fixedAge / PeoplesElbow.jumpDuration) * Time.fixedDeltaTime);
                base.characterMotor.velocity.y = 0f;
            }

            if (base.fixedAge >= (0.25f * PeoplesElbow.jumpDuration) && !this.slamIndicatorInstance)
            {
                this.CreateIndicator();
            }

            if (base.fixedAge >= PeoplesElbow.jumpDuration && !this.hasDropped)
            {
                this.StartDrop();
            }

            if (this.hasDropped && base.isAuthority && !base.characterMotor.disableAirControlUntilCollision)
            {
                this.LandingImpact();
                this.outer.SetNextStateToMain();
            }
        }

        private void StartDrop()
        {
            this.hasDropped = true;

            base.characterMotor.disableAirControlUntilCollision = true;
            base.characterMotor.velocity.y = -PeoplesElbow.dropForce;

            base.PlayAnimation("FullBody, Override", "PeoplesElbowSlam", "HighJump.playbackRate", 0.2f);
        }

        private void CreateIndicator()
        {
            if (EntityStates.Huntress.ArrowRain.areaIndicatorPrefab)
            {
                this.downRay = new Ray
                {
                    direction = Vector3.down,
                    origin = base.transform.position
                };

                this.slamIndicatorInstance = UnityEngine.Object.Instantiate<GameObject>(EntityStates.Huntress.ArrowRain.areaIndicatorPrefab).transform;
                this.slamIndicatorInstance.localScale = Vector3.one * PeoplesElbow.slamRadius;

                this.slamCenterIndicatorInstance = UnityEngine.Object.Instantiate<GameObject>(EntityStates.Huntress.ArrowRain.areaIndicatorPrefab).transform;
                this.slamCenterIndicatorInstance.localScale = (Vector3.one * PeoplesElbow.slamRadius) / 3f;
            }
        }

        private void LandingImpact()
        {
            base.characterMotor.velocity *= 0.1f;

            BlastAttack blastAttack = new BlastAttack();
            blastAttack.radius = PeoplesElbow.slamRadius;
            blastAttack.procCoefficient = PeoplesElbow.slamProcCoefficient;
            blastAttack.position = base.characterBody.footPosition;
            blastAttack.attacker = base.gameObject;
            blastAttack.crit = base.RollCrit();
            blastAttack.baseDamage = base.characterBody.damage * PeoplesElbow.slamDamageCoefficient;
            blastAttack.falloffModel = BlastAttack.FalloffModel.SweetSpot;
            blastAttack.baseForce = PeoplesElbow.slamForce;
            blastAttack.teamIndex = TeamComponent.GetObjectTeam(blastAttack.attacker);
            blastAttack.damageType = DamageType.Stun1s;
            blastAttack.attackerFiltering = AttackerFiltering.NeverHit;
            blastAttack.Fire();

            EffectManager.SpawnEffect(EntityStates.LemurianBruiserMonster.SpawnState.spawnEffectPrefab, new EffectData
            {
                origin = base.characterBody.footPosition,
                scale = 4f
            }, true);
        }

        private void UpdateSlamIndicator()
        {
            if (this.slamIndicatorInstance)
            {
                float maxDistance = 250f;

                this.downRay = new Ray
                {
                    direction = Vector3.down,
                    origin = base.transform.position
                };

                RaycastHit raycastHit;
                if (Physics.Raycast(this.downRay, out raycastHit, maxDistance, LayerIndex.world.mask))
                {
                    this.slamIndicatorInstance.transform.position = raycastHit.point;
                    this.slamIndicatorInstance.transform.up = raycastHit.normal;

                    this.slamCenterIndicatorInstance.transform.position = raycastHit.point;
                    this.slamCenterIndicatorInstance.transform.up = raycastHit.normal;
                }
            }
        }

        public override void OnExit()
        {
            base.OnExit();

            if (this.slamIndicatorInstance) EntityState.Destroy(this.slamIndicatorInstance.gameObject);
            if (this.slamCenterIndicatorInstance) EntityState.Destroy(this.slamCenterIndicatorInstance.gameObject);

            base.PlayAnimation("FullBody, Override", "BufferEmpty");

            base.characterBody.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;

            base.gameObject.layer = LayerIndex.defaultLayer.intVal;
            base.characterMotor.Motor.RebuildCollidableLayers();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }
    }
}