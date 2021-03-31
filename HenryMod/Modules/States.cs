using R2API;
using HenryMod.SkillStates;
using HenryMod.SkillStates.BaseStates;
using HenryMod.SkillStates.Stinger;
using HenryMod.SkillStates.Emotes;
using HenryMod.SkillStates.Bazooka;
using HenryMod.SkillStates.Henry.Shotgun;
using System.Collections.Generic;
using System;
using MonoMod.RuntimeDetour;
using EntityStates;
using RoR2;
using System.Reflection;
using HenryMod.SkillStates.Henry;

namespace HenryMod.Modules
{
    public static class States
    {
        internal static List<Type> entityStates = new List<Type>();

        private static Hook set_stateTypeHook;
        private static Hook set_typeNameHook;
        private static readonly BindingFlags allFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic;
        private delegate void set_stateTypeDelegate(ref SerializableEntityStateType self, Type value);
        private delegate void set_typeNameDelegate(ref SerializableEntityStateType self, String value);

        internal static void RegisterStates()
        {
            // fixing a vanilla bug- ignore this
            Type type = typeof(SerializableEntityStateType);
            HookConfig cfg = default;
            cfg.Priority = Int32.MinValue;
            set_stateTypeHook = new Hook(type.GetMethod("set_stateType", allFlags), new set_stateTypeDelegate(SetStateTypeHook), cfg);
            set_typeNameHook = new Hook(type.GetMethod("set_typeName", allFlags), new set_typeNameDelegate(SetTypeName), cfg);
            //

            entityStates.Add(typeof(BaseHenrySkillState));

            entityStates.Add(typeof(HenryMain));

            entityStates.Add(typeof(BaseEmote));
            entityStates.Add(typeof(Rest));
            entityStates.Add(typeof(Dance));

            entityStates.Add(typeof(BaseMeleeAttack));
            entityStates.Add(typeof(SlashCombo));
            entityStates.Add(typeof(PunchCombo));

            entityStates.Add(typeof(Shoot));
            entityStates.Add(typeof(ShootAlt));
            entityStates.Add(typeof(ShootUzi));
            entityStates.Add(typeof(UziIdle));

            entityStates.Add(typeof(Roll));

            entityStates.Add(typeof(BaseShotgunBlast));
            entityStates.Add(typeof(ShotgunBlastDown));
            entityStates.Add(typeof(ShotgunBlastUp));
            entityStates.Add(typeof(ShotgunBlastEntry));

            entityStates.Add(typeof(ThrowBomb));

            entityStates.Add(typeof(StingerEntry));
            entityStates.Add(typeof(Stinger));
            entityStates.Add(typeof(DashPunch));
            entityStates.Add(typeof(Uppercut));
            entityStates.Add(typeof(AirSlam));

            entityStates.Add(typeof(BazookaEnter));
            entityStates.Add(typeof(BazookaExit));
            entityStates.Add(typeof(BazookaCharge));
            entityStates.Add(typeof(BazookaFire));

            entityStates.Add(typeof(SkillStates.Bazooka.Scepter.BazookaCharge));
            entityStates.Add(typeof(SkillStates.Bazooka.Scepter.BazookaExit));
            entityStates.Add(typeof(SkillStates.Bazooka.Scepter.BazookaCharge));
            entityStates.Add(typeof(SkillStates.Bazooka.Scepter.BazookaFire));

            entityStates.Add(typeof(SkillStates.MrGreen.CloneDodge));
            entityStates.Add(typeof(SkillStates.MrGreen.CloneFakeDeath));
            entityStates.Add(typeof(SkillStates.MrGreen.CloneRespawnState));
            entityStates.Add(typeof(SkillStates.MrGreen.CloneSpawnState));
            //entityStates.Add(typeof(SkillStates.MrGreen.CloneThrow));
            entityStates.Add(typeof(SkillStates.MrGreen.Dash));
            entityStates.Add(typeof(SkillStates.MrGreen.PeoplesElbow));
            entityStates.Add(typeof(SkillStates.MrGreen.Resurrect));

            entityStates.Add(typeof(SkillStates.Nemry.DodgeSlash));
            entityStates.Add(typeof(SkillStates.Nemry.SpawnState));
            entityStates.Add(typeof(SkillStates.Nemry.MainState));
            entityStates.Add(typeof(SkillStates.Nemry.VoidBlast));
            entityStates.Add(typeof(SkillStates.Nemry.WeaponSwap));
            entityStates.Add(typeof(SkillStates.Nemry.SlashCombo));
            entityStates.Add(typeof(SkillStates.Nemry.ShootGun));

            entityStates.Add(typeof(SkillStates.Nemry.ChargeSlash.ChargeEntry));
            entityStates.Add(typeof(SkillStates.Nemry.ChargeSlash.StartCharge));
            entityStates.Add(typeof(SkillStates.Nemry.ChargeSlash.ChargeRelease));
            entityStates.Add(typeof(SkillStates.Nemry.ChargeSlash.Lunge));
            entityStates.Add(typeof(SkillStates.Nemry.ChargeSlash.LungeExit));
            entityStates.Add(typeof(SkillStates.Nemry.ChargeSlash.Uppercut));
            entityStates.Add(typeof(SkillStates.Nemry.ChargeSlash.Downslash));

            entityStates.Add(typeof(SkillStates.Nemry.Torrent.TorrentEntry));
            entityStates.Add(typeof(SkillStates.Nemry.Torrent.TorrentEntry));
            entityStates.Add(typeof(SkillStates.Nemry.Torrent.TorrentAir));

            entityStates.Add(typeof(SkillStates.Nemry.Burst));
        }

        // ignore this
        private static void SetStateTypeHook(ref this SerializableEntityStateType self, Type value)
        {
            self._typeName = value.AssemblyQualifiedName;
        }

        private static void SetTypeName(ref this SerializableEntityStateType self, String value)
        {
            Type t = GetTypeFromName(value);
            if (t != null)
            {
                self.SetStateTypeHook(t);
            }
        }

        private static Type GetTypeFromName(String name)
        {
            Type[] types = EntityStateCatalog.stateIndexToType;
            return Type.GetType(name);
        }
    }
}