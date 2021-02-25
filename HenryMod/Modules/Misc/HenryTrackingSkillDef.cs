using JetBrains.Annotations;
using RoR2;
using RoR2.Skills;
using HenryMod.Modules.Components;

namespace HenryMod.Modules.Misc
{
    public class HenryTrackingSkillDef : SkillDef
    {
        public override SkillDef.BaseSkillInstanceData OnAssigned([NotNull] GenericSkill skillSlot)
        {
            return new HenryTrackingSkillDef.InstanceData
            {
                henryTracker = skillSlot.GetComponent<HenryTracker>()
            };
        }

        private static bool HasTarget([NotNull] GenericSkill skillSlot)
        {
            HenryTracker henryTracker = ((HenryTrackingSkillDef.InstanceData)skillSlot.skillInstanceData).henryTracker;
            return (henryTracker != null) ? henryTracker.GetTrackingTarget() : null;
        }

        public override bool CanExecute([NotNull] GenericSkill skillSlot)
        {
            return HenryTrackingSkillDef.HasTarget(skillSlot) && base.CanExecute(skillSlot);
        }

        public override bool IsReady([NotNull] GenericSkill skillSlot)
        {
            return base.IsReady(skillSlot) && HenryTrackingSkillDef.HasTarget(skillSlot);
        }

        protected class InstanceData : SkillDef.BaseSkillInstanceData
        {
            public HenryTracker henryTracker;
        }
    }
}