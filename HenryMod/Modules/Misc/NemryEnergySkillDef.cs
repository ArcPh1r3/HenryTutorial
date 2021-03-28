using JetBrains.Annotations;
using RoR2;
using RoR2.Skills;
using HenryMod.Modules.Components;

namespace HenryMod.Modules.Misc
{
    public class NemryEnergySkillDef : SkillDef
    {
        public float cost;

        public override SkillDef.BaseSkillInstanceData OnAssigned([NotNull] GenericSkill skillSlot)
        {
            return new NemryEnergySkillDef.InstanceData
            {
                energyComponent = skillSlot.GetComponent<NemryEnergyComponent>()
            };
        }

        private static bool HasSufficientEnergy([NotNull] GenericSkill skillSlot)
        {
            NemryEnergyComponent energyComponent = ((NemryEnergySkillDef.InstanceData)skillSlot.skillInstanceData).energyComponent;
            return (energyComponent != null) ? (energyComponent.currentEnergy >= skillSlot.rechargeStock) : false;
        }

        public override bool CanExecute([NotNull] GenericSkill skillSlot)
        {
            return NemryEnergySkillDef.HasSufficientEnergy(skillSlot) && base.CanExecute(skillSlot);
        }

        public override bool IsReady([NotNull] GenericSkill skillSlot)
        {
            return base.IsReady(skillSlot) && NemryEnergySkillDef.HasSufficientEnergy(skillSlot);
        }

        protected class InstanceData : SkillDef.BaseSkillInstanceData
        {
            public NemryEnergyComponent energyComponent;
        }
    }
}