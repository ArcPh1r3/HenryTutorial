using EntityStates;
using RoR2;
using RoR2.Audio;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace HenryMod.Modules.BaseStates
{
    public abstract class BaseMeleeAttack : BaseSkillState
    {
        /// <summary>
        /// the name of your HitBoxGroup as setup with Prefabs.SetupHitBoxGroup in your survivor creation
        /// </summary>
        public string hitBoxGroupName = "SwordGroup";

        public DamageType damageType = DamageType.Generic;
        public float damageCoefficient = 3.5f;
        public float procCoefficient = 1f;
        public float pushForce = 300f;
        public Vector3 bonusForce = Vector3.zero;
        public float baseDuration = 1f;
        /// <summary>
        /// 0-1 multiplier of baseduration, used to time when the hitbox is out (usually based on the run time of the animation)
        /// <para>for example, if attackStartPercentTime is 0.5, the attack will start hitting halfway through the ability. if baseduration is 3 seconds, the attack will start happening at 1.5 seconds</para>
        /// </summary>
        public float attackStartPercentTime = 0.2f;

        /// <summary>
        /// 0-1 multiplier of baseduration, used to time when the hitbox stops (usually based on the run time of the animation)
        /// </summary>
        public float attackEndPercentTime = 0.4f;
        /// <summary>
        /// 0-1 multiplier of baseduration. This is the point at which the attack can be interrupted by itself, continuing a combo
        /// </summary>
        public float earlyExitPercentTime = 0.4f;

        public float hitStopDuration = 0.012f;
        /// <summary>
        /// camera recoil
        /// </summary>
        public float attackRecoil = 0.75f;
        /// <summary>
        /// when you land hits in the air
        /// </summary>
        public float hitHopVelocity = 4f;

        public string swingSoundString = "";
        /// <summary>
        /// this is an entry in your childlocator for where the effect will be spawned
        /// </summary>
        public string muzzleString = "SwingCenter";
        /// <summary>
        /// used to control the speed of your animations, this is paused during hitstop
        /// </summary>
        public string playbackRateParam;
        public GameObject swingEffectPrefab;
        public GameObject hitEffectPrefab;
        public NetworkSoundEventIndex impactSound = NetworkSoundEventIndex.Invalid;

        /// <summary>
        /// the actual damaging part of your melee attack. We call overlapAttack.Fire() every frame to see if it hits something
        /// <para>It's all set up in BaseMeleeAttack.OnEnter, but you can access it to do anything else you want, namely add modded damagetypes</para>
        /// </summary>
        protected OverlapAttack overlapAttack;
        protected float duration;
        protected Animator animator;
        protected float stopwatch;
        protected bool inHitPause;
        protected bool hasFired;

        private float hitPauseTimer;
        private bool hasHopped;
        private HitStopCachedState hitStopCachedState;
        private Vector3 storedVelocity;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            animator = GetModelAnimator();
            StartAimMode(0.5f + duration, false);

            overlapAttack = new OverlapAttack();
            overlapAttack.damageType = damageType;
            overlapAttack.attacker = gameObject;
            overlapAttack.inflictor = gameObject;
            overlapAttack.teamIndex = GetTeam();
            overlapAttack.damage = damageCoefficient * damageStat;
            overlapAttack.procCoefficient = procCoefficient;
            overlapAttack.hitEffectPrefab = hitEffectPrefab;
            overlapAttack.forceVector = bonusForce;
            overlapAttack.pushAwayForce = pushForce;
            overlapAttack.hitBoxGroup = FindHitBoxGroup(hitBoxGroupName);
            overlapAttack.isCrit = RollCrit();
            overlapAttack.impactSound = impactSound;
        }

        protected abstract void PlayAttackAnimation();

        public override void OnExit()
        {
            if (inHitPause)
            {
                RemoveHitstop();
            }
            base.OnExit();
        }

        protected virtual void PlaySwingEffect()
        {
            EffectManager.SimpleMuzzleFlash(swingEffectPrefab, gameObject, muzzleString, false);
        }

        protected virtual void OnHitEnemyAuthority()
        {
            //Util.PlaySound(hitSoundString, gameObject);

            if (!hasHopped)
            {
                if (characterMotor && !characterMotor.isGrounded && hitHopVelocity > 0f)
                {
                    SmallHop(characterMotor, hitHopVelocity);
                }

                hasHopped = true;
            }

            ApplyHitstop();
        }

        protected void ApplyHitstop()
        {
            if (!inHitPause && hitStopDuration > 0f)
            {
                storedVelocity = characterMotor.velocity;
                hitStopCachedState = CreateHitStopCachedState(characterMotor, animator, playbackRateParam);
                hitPauseTimer = hitStopDuration / attackSpeedStat;
                inHitPause = true;
            }
        }

        private void FireAttack()
        {
            if (isAuthority)
            {
                if (overlapAttack.Fire())
                {
                    OnHitEnemyAuthority();
                }
            }
        }

        private void EnterAttack()
        {
            hasFired = true;
            Util.PlayAttackSpeedSound(swingSoundString, gameObject, attackSpeedStat);

            PlaySwingEffect();

            if (isAuthority)
            {
                AddRecoil(-1f * attackRecoil, -2f * attackRecoil, -0.5f * attackRecoil, 0.5f * attackRecoil);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            hitPauseTimer -= Time.deltaTime;

            if (hitPauseTimer <= 0f && inHitPause)
            {
                RemoveHitstop();
            }

            if (!inHitPause)
            {
                stopwatch += Time.deltaTime;
            }
            else
            {
                if (characterMotor) characterMotor.velocity = Vector3.zero;
                if (animator) animator.SetFloat(playbackRateParam, 0f);
            }

            bool fireStarted = stopwatch >= duration * attackStartPercentTime;
            bool fireEnded = stopwatch >= duration * attackEndPercentTime;

            //to guarantee attack comes out if at high attack speed the stopwatch skips past the firing duration between frames
            if (fireStarted && !fireEnded || fireStarted && fireEnded && !hasFired)
            {
                if (!hasFired)
                {
                    EnterAttack();
                }
                FireAttack();
            }

            if (stopwatch >= duration && isAuthority)
            {
                outer.SetNextStateToMain();
                return;
            }
        }

        private void RemoveHitstop()
        {
            ConsumeHitStopCachedState(hitStopCachedState, characterMotor, animator);
            inHitPause = false;
            characterMotor.velocity = storedVelocity;
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            if (stopwatch >= duration * earlyExitPercentTime)
            {
                return InterruptPriority.Any;
            }
            return InterruptPriority.Skill;
        }
    }
}