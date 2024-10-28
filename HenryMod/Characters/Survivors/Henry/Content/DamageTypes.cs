using R2API;

namespace HenryMod.Survivors.Henry.HenryContent
{
    public class DamageTypes
    {
        public static DamageAPI.ModdedDamageType ComboFinisherDebuffDamage;

        public static void Init()
        {
            ComboFinisherDebuffDamage = DamageAPI.ReserveDamageType();
        }
    }
}
