using System;
using HenryMod.Modules.Characters;
using UnityEngine;

//todo windows change namespace
namespace HenryMod.Modules.Survivors
{
    internal class HenryEnemy : EnemyBase<HenryEnemy>
    {
        const string HENRY_PREFIX = HenryPlugin.DEVELOPER_PREFIX + "_HENRYMONSTER_";

        public override string assetBundleName => "myassetbundle";

        public override string bodyName => "HenryMonsterBody";

        public override string modelPrefabName => "mdlHenry";

        public override BodyInfo bodyInfo => new BodyInfo
        {
            bodyName = instance.bodyName,
            bodyNameToken = HENRY_PREFIX + "NAME",
            subtitleNameToken = HENRY_PREFIX + "SUBTITLE",

            characterPortrait = instance.assetBundle.LoadAsset<Texture>("texHenryIcon"),
            bodyColor = Color.white,

            crosshair = Modules.Assets.LoadCrosshair("Standard"),
            podPrefab = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/SurvivorPod"),

            maxHealth = 110f,
            healthRegen = 1.5f,
            armor = 0f,

            jumpCount = 1,
        };

        public override CustomRendererInfo[] customRendererInfos => null;

        public override void InitializeCharacter()
        {
            characterModelObject = R2API.PrefabAPI.InstantiateClone(HenrySurvivor.instance.characterModelObject, "mdlHenryMonster", false);
            base.InitializeCharacter();

            //add to css for testing
            Modules.Content.CreateSurvivor(bodyPrefab, characterModelObject, Color.red, HENRY_PREFIX);
        }

        public override void InitializeCharacterMaster()
        {
            throw new NotImplementedException();
        }

        public override void InitializeEntityStateMachines()
        {
            throw new NotImplementedException();
        }

        public override void InitializeSkills()
        {
            throw new NotImplementedException();
        }

        public override void InitializeSkins()
        {
            throw new NotImplementedException();
        }
    }
}
