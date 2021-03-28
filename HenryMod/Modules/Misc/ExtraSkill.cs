using RoR2;

namespace HenryMod.Modules.Misc
{
    public struct ExtraSkill
    {
        public int Value { get; }

        public static readonly ExtraSkill WeaponSwapSkill = new ExtraSkill(11);

        private ExtraSkill(int value)
        {
            Value = value;
        }

        public static implicit operator SkillSlot(ExtraSkill extraSkillSlot)
        {
            return (SkillSlot)extraSkillSlot.Value;
        }

        public static implicit operator ExtraSkill(SkillSlot skillSlot)
        {
            return new ExtraSkill((int)skillSlot);
        }
    }
}