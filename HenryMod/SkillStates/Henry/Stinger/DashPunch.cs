using EntityStates;
using HenryMod.Modules.Components;
using RoR2;
using System;
using UnityEngine;

namespace HenryMod.SkillStates.Stinger
{
    public class DashPunch : BaseSkillState
    {
        public static float dashSpeed = 100f;
        public static float hopForce = 16f;

        public static float damageCoefficient = 5f;
        public static float procCoefficient = 1f;
        public static float pushForce = 2500f;

        public static GameObject hitEffectPrefab = Modules.Assets.punchImpactEffect;
        public static NetworkSoundEventDef impactSound = Modules.Assets.punchHitSoundEvent;

        private HenryTracker tracker;
        private HurtBox target;
        private bool targetIsValid;
        private OverlapAttack attack;

        public override void OnEnter()
        {
            base.OnEnter();
            this.tracker = base.GetComponent<HenryTracker>();
            this.target = this.tracker.GetTrackingTarget();

            if (base.characterBody) base.characterBody.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
            if (base.isGrounded) base.SmallHop(base.characterMotor, 10f);

            if (this.target && this.target.healthComponent && this.target.healthComponent.alive)
            {
                this.targetIsValid = true;
            }

            HitBoxGroup hitBoxGroup = null;
            Transform modelTransform = base.GetModelTransform();

            if (modelTransform)
            {
                hitBoxGroup = Array.Find<HitBoxGroup>(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == "Sword");
            }

            this.attack = new OverlapAttack();
            this.attack.damageType = DamageType.Generic;
            this.attack.attacker = base.gameObject;
            this.attack.inflictor = base.gameObject;
            this.attack.teamIndex = base.GetTeam();
            this.attack.damage = DashPunch.damageCoefficient * this.damageStat;
            this.attack.procCoefficient = DashPunch.procCoefficient;
            this.attack.hitEffectPrefab = DashPunch.hitEffectPrefab;
            this.attack.hitBoxGroup = hitBoxGroup;
            this.attack.isCrit = base.RollCrit();
            this.attack.impactSound = DashPunch.impactSound.index;
            this.attack.pushAwayForce = DashPunch.pushForce * 0.2f;

            Util.PlaySound("HenryStinger", base.gameObject);
            base.PlayAnimation("FullBody, Override", "DashPunch");

            if (base.isGrounded)
            {
                EffectManager.SimpleEffect(Modules.Assets.dustEffect, base.characterBody.footPosition, base.transform.rotation, false);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.isAuthority && this.targetIsValid)
            {
                if (!this.target)
                {
                    this.outer.SetNextStateToMain();
                    return;
                }

                Vector3 velocity = (this.target.transform.position - base.transform.position).normalized * DashPunch.dashSpeed;

                base.characterMotor.velocity = velocity;
                base.characterDirection.forward = base.characterMotor.velocity.normalized;

                if (base.fixedAge >= 0.8f)
                {
                    this.outer.SetNextStateToMain();
                    return;
                }

                this.attack.forceVector = base.characterMotor.velocity.normalized * DashPunch.pushForce;

                if (this.attack.Fire())
                {
                    this.outer.SetNextState(new StingerExit());
                    return;
                }
            }
            else
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            base.characterBody.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;
            base.characterMotor.velocity *= 0.1f;

            base.PlayAnimation("FullBody, Override", "BufferEmpty");
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }
    }
}