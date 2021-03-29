using EntityStates;

namespace HenryMod.SkillStates.MrGreen
{
    public class CloneSpawnState : BaseState
    {
        public static float duration = 0.4f;

        public override void OnEnter()
        {
            base.OnEnter();
            base.PlayAnimation("Body", "Spawn");
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.isAuthority && base.fixedAge >= CloneSpawnState.duration)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }
    }
}