using RoR2;
using System.Linq;
using UnityEngine;

namespace HenryMod.Modules.Components
{
    public class HenryTracker : MonoBehaviour
    {
        public float maxTrackingDistance = 40f;
        public float maxTrackingAngle = 20f;
        public float trackerUpdateFrequency = 10f;

        private HurtBox trackingTarget;
        private CharacterBody characterBody;
        private TeamComponent teamComponent;
        private InputBankTest inputBank;
        private float trackerUpdateStopwatch;
        private Indicator indicator;
        private readonly BullseyeSearch search = new BullseyeSearch();

        private void Awake()
        {
            this.indicator = new Indicator(base.gameObject, Resources.Load<GameObject>("Prefabs/HuntressTrackingIndicator"));
        }

        private void Start()
        {
            this.characterBody = base.GetComponent<CharacterBody>();
            this.inputBank = base.GetComponent<InputBankTest>();
            this.teamComponent = base.GetComponent<TeamComponent>();
        }

        public HurtBox GetTrackingTarget()
        {
            return this.trackingTarget;
        }

        private void OnEnable()
        {
            this.indicator.active = true;
        }

        private void OnDisable()
        {
            this.indicator.active = false;
        }

        private void OnDestroy()
        {
            this.indicator.active = false;
        }

        private void FixedUpdate()
        {
            this.trackerUpdateStopwatch += Time.fixedDeltaTime;

            if (this.trackerUpdateStopwatch >= 1f / this.trackerUpdateFrequency)
            {
                this.trackerUpdateStopwatch -= 1f / this.trackerUpdateFrequency;

                Ray aimRay = new Ray(this.inputBank.aimOrigin, this.inputBank.aimDirection);
                this.SearchForTarget(aimRay);

                this.indicator.targetTransform = (this.trackingTarget ? this.trackingTarget.transform : null);
            }
        }

        private void SearchForTarget(Ray aimRay)
        {
            this.search.teamMaskFilter = TeamMask.GetUnprotectedTeams(this.teamComponent.teamIndex);
            this.search.filterByLoS = true;
            this.search.searchOrigin = aimRay.origin;
            this.search.searchDirection = aimRay.direction;
            this.search.sortMode = BullseyeSearch.SortMode.Distance;
            this.search.maxDistanceFilter = this.maxTrackingDistance;
            this.search.maxAngleFilter = this.maxTrackingAngle;
            this.search.RefreshCandidates();
            this.search.FilterOutGameObject(base.gameObject);

            this.trackingTarget = this.search.GetResults().FirstOrDefault<HurtBox>();
        }
    }
}