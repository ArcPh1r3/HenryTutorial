using RoR2;
using HenryMod.Modules.Characters;

//todo windows change namespace
namespace HenryMod.Modules.Survivors
{
    internal abstract class EnemyBase<T> : CharacterBase<T> where T : EnemyBase<T>, new()
    {
        //public directorinfo
        public override void InitializeCharacter()
        {
            base.InitializeCharacter();

            InitializeEnemy();
        }

        protected virtual void InitializeEnemy()
        {
            DirectorCard nip = new DirectorCard();
        }
    }
}
