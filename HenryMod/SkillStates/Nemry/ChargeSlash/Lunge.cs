using EntityStates;
using RoR2;
using UnityEngine;
using System;
using HenryMod.SkillStates.BaseStates;

namespace HenryMod.SkillStates.Nemry.ChargeSlash
{
    public class Lunge : BaseNemrySkillState
    {
        public float charge;

        public static float dashSpeed = 120f;
        public static float hopForce = 10f;

        public static float maxDamageCoefficient = 12f;
        public static float minDamageCoefficient = 6f;
        public static float procCoefficient = 1f;
        public static float pushForce = 200f;

        public static GameObject hitEffectPrefab = Modules.Assets.nemSwordHitImpactEffect;
        public static NetworkSoundEventDef impactSound = Modules.Assets.nemSwordHitSoundEvent;

        private HurtBox target;
        private bool targetIsValid;
        private OverlapAttack attack;
        private Vector3 storedPosition;

        public override void OnEnter()
        {
            base.OnEnter();
            this.target = this.tracker.GetTrackingTarget();

            if (base.characterBody) base.characterBody.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;

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

            float damage = Util.Remap(this.charge, 0f, 1f, Lunge.minDamageCoefficient, Lunge.maxDamageCoefficient);

            this.attack = new OverlapAttack();
            this.attack.damageType = DamageType.Generic;
            this.attack.attacker = base.gameObject;
            this.attack.inflictor = base.gameObject;
            this.attack.teamIndex = base.GetTeam();
            this.attack.damage = damage * this.damageStat;
            this.attack.procCoefficient = Lunge.procCoefficient;
            this.attack.hitEffectPrefab = Lunge.hitEffectPrefab;
            this.attack.hitBoxGroup = hitBoxGroup;
            this.attack.isCrit = base.RollCrit();
            this.attack.impactSound = Lunge.impactSound.index;
            this.attack.pushAwayForce = Lunge.pushForce * 0.2f;

            Util.PlaySound("HenryLunge", base.gameObject);
            base.PlayAnimation("FullBody, Override", "Lunge");

            if (base.isGrounded)
            {
                EffectManager.SimpleEffect(Modules.Assets.dustEffect, base.characterBody.footPosition, base.transform.rotation, false);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (this.target) this.storedPosition = this.target.transform.position;

            if (base.isAuthority && this.targetIsValid)
            {
                Vector3 velocity = (this.storedPosition - base.transform.position).normalized * Lunge.dashSpeed;

                base.characterMotor.velocity = velocity;
                base.characterDirection.forward = base.characterMotor.velocity.normalized;

                // don't get locked in a Lunge forever (idk what would cause this but it is entirely possible)
                if (base.fixedAge >= 0.8f)
                {
                    this.outer.SetNextStateToMain();
                    return;
                }

                this.attack.forceVector = base.characterMotor.velocity.normalized * Lunge.pushForce;

                if (this.attack.Fire())
                {
                    this.outer.SetNextState(new LungeExit());
                    return;
                }
            }
            else
            {
                base.skillLocator.secondary.AddOneStock();
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