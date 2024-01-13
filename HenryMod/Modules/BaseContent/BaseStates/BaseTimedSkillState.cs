using EntityStates;
using System;

namespace HenryMod.Modules.BaseStates
{
    //see example skills below
    public class BaseTimedSkillState : BaseSkillState
    {
        //total duration of the move
        public float TimedBaseDuration;

        //time relative to duration that the skill starts
        //for example, set 0.5 and the "cast" will happen halfway through the skill
        public float TimedBaseCastStartTime;
        public float TimedBaseCastEndTime;

        protected float duration;
        protected float castStartPercentTime;
        protected float castEndPercentTime;
        protected bool hasFired;
        protected bool isFiring;
        protected bool hasExited;

        //initialize your time values here
        protected virtual void InitDurationValues(float baseDuration, float castStartPercentTime, float castEndPercentTime = 1)
        {
            TimedBaseDuration = baseDuration;
            TimedBaseCastStartTime = castStartPercentTime;
            TimedBaseCastEndTime = castEndPercentTime;

            duration = TimedBaseDuration / attackSpeedStat;
            this.castStartPercentTime = castStartPercentTime * duration;
            this.castEndPercentTime = castEndPercentTime * duration;
        }

        protected virtual void OnCastEnter() { }
        protected virtual void OnCastFixedUpdate() { }
        protected virtual void OnCastUpdate() { }
        protected virtual void OnCastExit() { }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            //wait start duration and fire
            if (!hasFired && fixedAge > castStartPercentTime)
            {
                hasFired = true;
                OnCastEnter();
            }

            bool fireStarted = fixedAge >= castStartPercentTime;
            bool fireEnded = fixedAge >= castEndPercentTime;
            isFiring = false;

            //to guarantee attack comes out if at high attack speed the fixedage skips past the endtime
            if (fireStarted && !fireEnded || fireStarted && fireEnded && !hasFired)
            {
                isFiring = true;
                OnCastFixedUpdate();
            }

            if (fireEnded && !hasExited)
            {
                hasExited = true;
                OnCastExit();
            }

            if (fixedAge > duration)
            {
                outer.SetNextStateToMain();
                return;
            }
        }

        public override void Update()
        {
            base.Update();
            if (isFiring)
            {
                OnCastUpdate();
            }
        }
    }

    public class ExampleTimedSkillState : BaseTimedSkillState
    {
        public static float SkillBaseDuration = 1.5f;
        public static float SkillStartTime = 0.2f;
        public static float SkillEndTime = 0.9f;

        public override void OnEnter()
        {
            base.OnEnter();

            InitDurationValues(SkillBaseDuration, SkillStartTime, SkillEndTime);
        }

        protected override void OnCastEnter()
        {
            //perform my skill after 0.3 seconds of windup
        }

        protected override void OnCastFixedUpdate()
        {
            //perform some continuous action after the windup, which will end .15 seconds before the full duration
        }

        protected override void OnCastExit()
        {
            //probably play an animation at the end of the action
        }
    }

    public class ExampleDelayedSkillState : BaseTimedSkillState
    {
        public static float SkillBaseDuration = 1.5f;
        public static float SkillStartTime = 0.2f;

        public override void OnEnter()
        {
            base.OnEnter();

            InitDurationValues(SkillBaseDuration, SkillStartTime);
        }

        protected override void OnCastEnter()
        {
            //perform my skill after 0.3 seconds of windup
        }
    }
}