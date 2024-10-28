using HenryMod.Survivors.Henry.SkillStates;

namespace HenryMod.Survivors.Henry.HenryContent
{
    public static class States
    {
        public static void Init()
        {
            Modules.Content.AddEntityState(typeof(SlashCombo));

            Modules.Content.AddEntityState(typeof(Shoot));

            Modules.Content.AddEntityState(typeof(Roll));

            Modules.Content.AddEntityState(typeof(ThrowBomb));
        }
    }
}
