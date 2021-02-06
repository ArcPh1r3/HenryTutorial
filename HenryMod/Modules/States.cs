using R2API;
using HenryMod.SkillStates;
using HenryMod.SkillStates.BaseStates;

namespace HenryMod.Modules
{
    public static class States
    {
        public static void RegisterStates()
        {
            LoadoutAPI.AddSkill(typeof(BaseMeleeAttack));
            LoadoutAPI.AddSkill(typeof(SlashCombo));
            LoadoutAPI.AddSkill(typeof(Shoot));
            LoadoutAPI.AddSkill(typeof(Roll));
            LoadoutAPI.AddSkill(typeof(ThrowBomb));
        }
    }
}