using JetBrains.Annotations;
using RoR2;
using RoR2.Skills;
using HenryMod.Modules.Components;

namespace HenryMod.Modules.Misc
{
    public class TrackingEnergySkillDef : SkillDef
    {
        public float cost;

        public override SkillDef.BaseSkillInstanceData OnAssigned([NotNull] GenericSkill skillSlot)
        {
            return new TrackingEnergySkillDef.InstanceData
            {
                energyComponent = skillSlot.GetComponent<NemryEnergyComponent>(),
                henryTracker = skillSlot.GetComponent<HenryTracker>()
            };
        }

        private static bool HasSufficientEnergy([NotNull] GenericSkill skillSlot)
        {
            NemryEnergyComponent energyComponent = ((TrackingEnergySkillDef.InstanceData)skillSlot.skillInstanceData).energyComponent;
            return (energyComponent != null) ? (energyComponent.currentEnergy >= skillSlot.rechargeStock) : false;
        }

        private static bool HasTarget([NotNull] GenericSkill skillSlot)
        {
            HenryTracker henryTracker = ((TrackingEnergySkillDef.InstanceData)skillSlot.skillInstanceData).henryTracker;
            return (henryTracker != null) ? henryTracker.GetTrackingTarget() : null;
        }

        public override bool CanExecute([NotNull] GenericSkill skillSlot)
        {
            return TrackingEnergySkillDef.HasSufficientEnergy(skillSlot) && TrackingEnergySkillDef.HasTarget(skillSlot) && base.CanExecute(skillSlot);
        }

        public override bool IsReady([NotNull] GenericSkill skillSlot)
        {
            return base.IsReady(skillSlot) && TrackingEnergySkillDef.HasSufficientEnergy(skillSlot) && TrackingEnergySkillDef.HasTarget(skillSlot);
        }

        protected class InstanceData : SkillDef.BaseSkillInstanceData
        {
            public NemryEnergyComponent energyComponent;
            public HenryTracker henryTracker;
        }
    }
}