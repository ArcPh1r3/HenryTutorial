using EntityStates;
using RoR2;
using System.Linq;
using UnityEngine;

namespace HenryMod.SkillStates.MrGreen
{
    public class Dash : BaseState
    {
        private Vector3 dashVector = Vector3.zero;
        public float duration = 0.2f;
        public float speedCoefficient = 8f;

        public override void OnEnter()
        {
            base.OnEnter();
            base.characterBody.isSprinting = true;
            this.dashVector = ((base.inputBank.moveVector == Vector3.zero) ? base.characterDirection.forward : base.inputBank.moveVector).normalized;

            Util.PlaySound("HenryStinger", base.gameObject);
            base.PlayAnimation("Gesture, Override", "BufferEmpty");
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            base.characterMotor.velocity = Vector3.zero;
            base.characterMotor.rootMotion = this.dashVector * (this.moveSpeedStat * this.speedCoefficient * Time.fixedDeltaTime) * Mathf.Cos(base.fixedAge / (this.duration * 1.3f) * 1.57079637f);

            if (base.isAuthority)
            {
                if (base.characterDirection)
                {
                    base.characterDirection.forward = this.dashVector;
                }
            }

            this.SearchForAllies();

            if (base.isAuthority && base.fixedAge >= this.duration)
            {
                this.outer.SetNextStateToMain();
            }
        }

        private void SearchForAllies()
        {
            Ray aimRay = base.GetAimRay();

            BullseyeSearch search = new BullseyeSearch
            {
                teamMaskFilter = TeamMask.none,
                filterByLoS = false,
                searchOrigin = base.transform.position,
                searchDirection = Random.onUnitSphere,
                sortMode = BullseyeSearch.SortMode.Distance,
                maxDistanceFilter = 3f,
                maxAngleFilter = 360f
            };

            search.teamMaskFilter.AddTeam(base.GetTeam());

            search.RefreshCandidates();
            search.FilterOutGameObject(base.gameObject);

            HurtBox target = search.GetResults().FirstOrDefault<HurtBox>();
            if (target)
            {
                if (target.healthComponent && target.healthComponent.body)
                {
                    this.outer.SetNextState(new PeoplesElbow());
                    return;
                }
            }
        }

        public override void OnExit()
        {
            base.characterMotor.velocity *= 0.1f;
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }
    }
}