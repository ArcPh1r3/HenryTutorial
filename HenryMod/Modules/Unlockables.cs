using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using RoR2.Achievements;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace HenryMod.Modules
{
    internal static class Unlockables
    {
        private static readonly HashSet<string> usedRewardIds = new HashSet<string>();
        public static List<AchievementDef> achievementDefs = new List<AchievementDef>();
        public static List<UnlockableDef> unlockableDefs = new List<UnlockableDef>();
        private static readonly List<(AchievementDef achDef, UnlockableDef unlockableDef, string unlockableName)> moddedUnlocks = new List<(AchievementDef achDef, UnlockableDef unlockableDef, string unlockableName)>();

        private static bool addingUnlockables;
        public static bool ableToAdd { get; private set; } = false;

        public static UnlockableDef CreateNewUnlockable(UnlockableInfo unlockableInfo)
        {
            UnlockableDef newUnlockableDef = ScriptableObject.CreateInstance<UnlockableDef>();

            newUnlockableDef.nameToken = unlockableInfo.Name;
            newUnlockableDef.cachedName = unlockableInfo.Name;
            newUnlockableDef.getHowToUnlockString = unlockableInfo.HowToUnlockString;
            newUnlockableDef.getUnlockedString = unlockableInfo.UnlockedString;
            newUnlockableDef.sortScore = unlockableInfo.SortScore;

            return newUnlockableDef;
        }

        [Obsolete("The bool parameter serverTracked is redundant. Instead, pass in a BaseServerAchievement type if it is server tracked, or nothing if it's not")]
        public static UnlockableDef AddUnlockable<TUnlockable>(bool serverTracked) where TUnlockable : BaseAchievement, IModdedUnlockableDataProvider, new()
        {
            return AddUnlockable<TUnlockable>(null);
        }
        public static UnlockableDef AddUnlockable<TUnlockable>(Type serverTrackerType = null) where TUnlockable : BaseAchievement, IModdedUnlockableDataProvider, new()
        {
            TUnlockable instance = new TUnlockable();

            string unlockableIdentifier = instance.UnlockableIdentifier;

            if (!usedRewardIds.Add(unlockableIdentifier)) throw new InvalidOperationException($"The unlockable identifier '{unlockableIdentifier}' is already used by another mod or the base game.");

            AchievementDef achievementDef = new AchievementDef
            {
                identifier = instance.AchievementIdentifier,
                unlockableRewardIdentifier = instance.UnlockableIdentifier,
                prerequisiteAchievementIdentifier = instance.PrerequisiteUnlockableIdentifier,
                nameToken = instance.AchievementNameToken,
                descriptionToken = instance.AchievementDescToken,
                achievedIcon = instance.Sprite,
                type = instance.GetType(),
                serverTrackerType = serverTrackerType,
            };

            UnlockableDef unlockableDef = CreateNewUnlockable(new UnlockableInfo
            {
                Name = instance.UnlockableIdentifier,
                HowToUnlockString = instance.GetHowToUnlock,
                UnlockedString = instance.GetUnlocked,
                SortScore = 200
            });

            unlockableDefs.Add(unlockableDef);
            achievementDefs.Add(achievementDef);

            moddedUnlocks.Add((achievementDef, unlockableDef, instance.UnlockableIdentifier));

            if (!addingUnlockables)
            {
                addingUnlockables = true;
                IL.RoR2.AchievementManager.CollectAchievementDefs += CollectAchievementDefs;
                IL.RoR2.UnlockableCatalog.Init += Init_Il;
            }

            return unlockableDef;
        }

        public static ILCursor CallDel_<TDelegate>(this ILCursor cursor, TDelegate target, out int index)
where TDelegate : Delegate
        {
            index = cursor.EmitDelegate(target);
            return cursor;
        }
        public static ILCursor CallDel_<TDelegate>(this ILCursor cursor, TDelegate target)
            where TDelegate : Delegate => cursor.CallDel_(target, out _);

        private static void Init_Il(ILContext il) => new ILCursor(il)
    .GotoNext(MoveType.AfterLabel, x => x.MatchCallOrCallvirt(typeof(UnlockableCatalog), nameof(UnlockableCatalog.SetUnlockableDefs)))
    .CallDel_(ArrayHelper.AppendDel(unlockableDefs));

        private static void CollectAchievementDefs(ILContext il)
        {
            var f1 = typeof(AchievementManager).GetField("achievementIdentifiers", BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic);
            if (f1 is null) throw new NullReferenceException($"Could not find field in {nameof(AchievementManager)}");
            var cursor = new ILCursor(il);
            _ = cursor.GotoNext(MoveType.After,
                x => x.MatchEndfinally(),
                x => x.MatchLdloc(1)
            );

            void EmittedDelegate(List<AchievementDef> list, Dictionary<string, AchievementDef> map, List<string> identifiers)
            {
                ableToAdd = false;
                for (int i = 0; i < moddedUnlocks.Count; ++i)
                {
                    var (ach, unl, unstr) = moddedUnlocks[i];
                    if (ach is null) continue;
                    identifiers.Add(ach.identifier);
                    list.Add(ach);
                    map.Add(ach.identifier, ach);
                }
            }

            _ = cursor.Emit(OpCodes.Ldarg_0);
            _ = cursor.Emit(OpCodes.Ldsfld, f1);
            _ = cursor.EmitDelegate<Action<List<AchievementDef>, Dictionary<string, AchievementDef>, List<string>>>(EmittedDelegate);
            _ = cursor.Emit(OpCodes.Ldloc_1);
        }

        public struct UnlockableInfo
        {
            public string Name;
            public Func<string> HowToUnlockString;
            public Func<string> UnlockedString;
            public int SortScore;
        }
    }

    public interface IModdedUnlockableDataProvider
    {
        string AchievementIdentifier { get; }
        string UnlockableIdentifier { get; }
        string AchievementNameToken { get; }
        string PrerequisiteUnlockableIdentifier { get; }
        string UnlockableNameToken { get; }
        string AchievementDescToken { get; }
        Sprite Sprite { get; }
        Func<string> GetHowToUnlock { get; }
        Func<string> GetUnlocked { get; }
    }

    //fuck your internal i'm a slut
    public abstract class ModdedUnlockable : BaseAchievement, IModdedUnlockableDataProvider
    {
        #region Implementation
        public void Revoke()
        {
            if (userProfile.HasAchievement(AchievementIdentifier))
            {
                userProfile.RevokeAchievement(AchievementIdentifier);
            }

            userProfile.RevokeUnlockable(UnlockableCatalog.GetUnlockableDef(UnlockableIdentifier));
        }
        #endregion

        #region Contract
        public abstract string AchievementIdentifier { get; }
        public abstract string UnlockableIdentifier { get; }
        public abstract string AchievementNameToken { get; }
        public abstract string PrerequisiteUnlockableIdentifier { get; }
        public abstract string UnlockableNameToken { get; }
        public abstract string AchievementDescToken { get; }
        public abstract Sprite Sprite { get; }
        public abstract Func<string> GetHowToUnlock { get; }
        public abstract Func<string> GetUnlocked { get; }
        #endregion

        #region Virtuals
        public override void OnGranted() => base.OnGranted();
        public override void OnInstall()
        {
            base.OnInstall();
        }
        public override void OnUninstall()
        {
            base.OnUninstall();
        }
        public override float ProgressForAchievement() => base.ProgressForAchievement();
        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return base.LookUpRequiredBodyIndex();
        }
        public override void OnBodyRequirementBroken() => base.OnBodyRequirementBroken();
        public override void OnBodyRequirementMet() => base.OnBodyRequirementMet();
        public override bool wantsBodyCallbacks { get => base.wantsBodyCallbacks; }
        #endregion
    }

    public static class ArrayHelper
    {
        public static T[] Append<T>(ref T[] array, List<T> list)
        {
            var orig = array.Length;
            var added = list.Count;
            Array.Resize(ref array, orig + added);
            list.CopyTo(array, orig);
            return array;
        }

        public static Func<T[], T[]> AppendDel<T>(List<T> list) => (r) => Append(ref r, list);
    }
}