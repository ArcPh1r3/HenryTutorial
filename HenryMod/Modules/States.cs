using R2API;
using HenryMod.SkillStates;
using HenryMod.SkillStates.BaseStates;
using HenryMod.SkillStates.Stinger;

namespace HenryMod.Modules
{
    public static class States
    {
        public static void RegisterStates()
        {
            LoadoutAPI.AddSkill(typeof(HenryMain));

            LoadoutAPI.AddSkill(typeof(BaseMeleeAttack));
            LoadoutAPI.AddSkill(typeof(SlashCombo));
            LoadoutAPI.AddSkill(typeof(PunchCombo));

            LoadoutAPI.AddSkill(typeof(Shoot));

            LoadoutAPI.AddSkill(typeof(Roll));

            LoadoutAPI.AddSkill(typeof(ThrowBomb));

            LoadoutAPI.AddSkill(typeof(StingerEntry));
            LoadoutAPI.AddSkill(typeof(Stinger));
            LoadoutAPI.AddSkill(typeof(DashPunch));
        }
    }
}