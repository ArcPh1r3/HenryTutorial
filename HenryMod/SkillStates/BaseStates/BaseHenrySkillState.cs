using EntityStates;
using HenryMod.Modules.Components;

namespace HenryMod.SkillStates.BaseStates
{
    public class BaseHenrySkillState : BaseSkillState
    {
        protected HenryController henryController;

        public override void OnEnter()
        {
            this.henryController = base.GetComponent<HenryController>();
            base.OnEnter();
        }
    }
}