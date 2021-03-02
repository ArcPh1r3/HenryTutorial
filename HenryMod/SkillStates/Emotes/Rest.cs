namespace HenryMod.SkillStates.Emotes
{
    public class Rest : BaseEmote
    {
        public override void OnEnter()
        {
            this.animString = "RestEmote";
            this.animDuration = 0.75f;
            base.OnEnter();
        }
    }
}