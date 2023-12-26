using HenryMod.SkillStates;

//todo windows change namespace
namespace HenryMod.Characters.Survivors.Henry.Content {
    public static class HenryStates {
        public static void Init() {
            Modules.Content.AddEntityState(typeof(SlashCombo));

            Modules.Content.AddEntityState(typeof(Shoot));

            Modules.Content.AddEntityState(typeof(Roll));

            Modules.Content.AddEntityState(typeof(ThrowBomb));
        }
    }
}
