﻿using EntityStates;
using RoR2;
using UnityEngine;
using HenryMod.Modules.Components;
using System;

namespace HenryMod.SkillStates.Stinger
{
    public class Stinger : BaseSkillState
    {
        public static float dashSpeed = 80f;
        public static float hopForce = 10f;

        public static float damageCoefficient = Modules.StaticValues.stingerDamageCoefficient;
        public static float procCoefficient = 1f;
        public static float pushForce = 1000f;

        public static GameObject hitEffectPrefab = Modules.Assets.swordHitImpactEffect;
        public static NetworkSoundEventDef impactSound = Modules.Assets.swordHitSoundEvent;

        private HenryTracker tracker;
        private HurtBox target;
        private bool targetIsValid;
        private OverlapAttack attack;
        private Vector3 storedPosition;

        public override void OnEnter()
        {
            base.OnEnter();
            this.tracker = base.GetComponent<HenryTracker>();
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

            this.attack = new OverlapAttack();
            this.attack.damageType = DamageType.Generic;
            this.attack.attacker = base.gameObject;
            this.attack.inflictor = base.gameObject;
            this.attack.teamIndex = base.GetTeam();
            this.attack.damage = Stinger.damageCoefficient * this.damageStat;
            this.attack.procCoefficient = Stinger.procCoefficient;
            this.attack.hitEffectPrefab = Stinger.hitEffectPrefab;
            this.attack.hitBoxGroup = hitBoxGroup;
            this.attack.isCrit = base.RollCrit();
            this.attack.impactSound = Stinger.impactSound.index;
            this.attack.pushAwayForce = Stinger.pushForce * 0.2f;

            Util.PlaySound("HenryStinger", base.gameObject);
            base.PlayAnimation("FullBody, Override", "Stinger");

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
                Vector3 velocity = (this.storedPosition - base.transform.position).normalized * Stinger.dashSpeed;

                base.characterMotor.velocity = velocity;
                base.characterDirection.forward = base.characterMotor.velocity.normalized;

                // don't get locked in a stinger forever (idk what would cause this but it is entirely possible)
                if (base.fixedAge >= 0.8f)
                {
                    this.outer.SetNextStateToMain();
                    return;
                }

                this.attack.forceVector = base.characterMotor.velocity.normalized * Stinger.pushForce;

                if (this.attack.Fire())
                {
                    this.outer.SetNextState(new StingerExit());
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