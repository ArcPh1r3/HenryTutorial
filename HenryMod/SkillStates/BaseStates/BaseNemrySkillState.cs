using EntityStates;
using HenryMod.Modules.Components;
using RoR2;

namespace HenryMod.SkillStates.BaseStates
{
    public class BaseNemrySkillState : BaseSkillState
    {
        protected NemryEnergyComponent energyComponent;
        protected HenryTracker tracker;

        public override void OnEnter()
        {
            this.energyComponent = base.GetComponent<NemryEnergyComponent>();
            this.tracker = base.GetComponent<HenryTracker>();
            base.OnEnter();
        }

        protected bool AddEnergy(float amount)
        {
            if (this.energyComponent) return this.energyComponent.AddEnergy(amount);
            else return false;
        }

        protected bool SpendEnergy(float amount)
        {
            return this.SpendEnergy(amount, SkillSlot.None);
        }

        protected bool SpendEnergy(float amount, SkillSlot skillSlot)
        {
            if (this.energyComponent) return this.energyComponent.SpendEnergy(amount, skillSlot);
            else return false;
        }
    }
}
