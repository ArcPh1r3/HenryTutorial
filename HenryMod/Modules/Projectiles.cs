using R2API;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace HenryMod.Modules
{
    internal static class Projectiles
    {
        internal static GameObject bombPrefab;
        internal static GameObject bazookaRocketPrefab;

        internal static GameObject voidBlastPrefab;

        internal static void RegisterProjectiles()
        {
            // only separating into separate methods for my sanity
            CreateBomb();
            CreateBazookaRocket();

            CreateVoidBlast();

            Modules.Prefabs.projectilePrefabs.Add(bombPrefab);
            Modules.Prefabs.projectilePrefabs.Add(bazookaRocketPrefab);
            Modules.Prefabs.projectilePrefabs.Add(voidBlastPrefab);
        }

        private static void CreateVoidBlast()
        {
            voidBlastPrefab = CloneProjectilePrefab("CommandoGrenadeProjectile", "NemryVoidBlastProjectile");

            ProjectileImpactExplosion bombImpactExplosion = voidBlastPrefab.GetComponent<ProjectileImpactExplosion>();
            InitializeImpactExplosion(bombImpactExplosion);

            bombImpactExplosion.blastRadius = 8f;
            bombImpactExplosion.destroyOnEnemy = true;
            bombImpactExplosion.lifetime = 12f;
            bombImpactExplosion.impactEffect = Resources.Load<GameObject>("Prefabs/Effects/NullifierExplosion");
            //bombImpactExplosion.lifetimeExpiredSound = Modules.Assets.CreateNetworkSoundEventDef("HenryBombExplosion");
            bombImpactExplosion.timerAfterImpact = true;
            bombImpactExplosion.lifetimeAfterImpact = 0.1f;

            ProjectileDamage bombDamage = voidBlastPrefab.GetComponent<ProjectileDamage>();
            bombDamage.damageType = DamageType.Nullify;

            ProjectileController bombController = voidBlastPrefab.GetComponent<ProjectileController>();
            bombController.ghostPrefab = Resources.Load<GameObject>("Prefabs/ProjectileGhosts/NullifierPreBombGhost");
            bombController.startSound = "";

            voidBlastPrefab.GetComponent<Rigidbody>().useGravity = false;
        }

        private static void CreateBomb()
        {
            bombPrefab = CloneProjectilePrefab("CommandoGrenadeProjectile", "HenryBombProjectile");

            ProjectileImpactExplosion bombImpactExplosion = bombPrefab.GetComponent<ProjectileImpactExplosion>();
            InitializeImpactExplosion(bombImpactExplosion);

            bombImpactExplosion.blastRadius = 16f;
            bombImpactExplosion.destroyOnEnemy = true;
            bombImpactExplosion.lifetime = 12f;
            bombImpactExplosion.impactEffect = Modules.Assets.bombExplosionEffect;
            //bombImpactExplosion.lifetimeExpiredSound = Modules.Assets.CreateNetworkSoundEventDef("HenryBombExplosion");
            bombImpactExplosion.timerAfterImpact = true;
            bombImpactExplosion.lifetimeAfterImpact = 0.1f;

            ProjectileController bombController = bombPrefab.GetComponent<ProjectileController>();
            bombController.ghostPrefab = CreateGhostPrefab("HenryBombGhost");
            bombController.startSound = "";
        }

        private static void CreateBazookaRocket()
        {
            bazookaRocketPrefab = CloneProjectilePrefab("CommandoGrenadeProjectile", "HenryBazookaRocketProjectile");
            bazookaRocketPrefab.AddComponent<Modules.Components.BazookaRotation>();
            bazookaRocketPrefab.transform.localScale *= 2f;

            ProjectileImpactExplosion bazookaImpactExplosion = bazookaRocketPrefab.GetComponent<ProjectileImpactExplosion>();
            InitializeImpactExplosion(bazookaImpactExplosion);

            bazookaImpactExplosion.blastRadius = 8f;
            bazookaImpactExplosion.destroyOnEnemy = true;
            bazookaImpactExplosion.lifetime = 12f;
            bazookaImpactExplosion.impactEffect = Modules.Assets.bazookaExplosionEffect;
            //bazookaImpactExplosion.lifetimeExpiredSound = Modules.Assets.CreateNetworkSoundEventDef("HenryBazookaExplosion");
            bazookaImpactExplosion.timerAfterImpact = true;
            bazookaImpactExplosion.lifetimeAfterImpact = 0f;

            ProjectileController bazookaController = bazookaRocketPrefab.GetComponent<ProjectileController>();

            GameObject bazookaGhost = CreateGhostPrefab("HenryBazookaRocketGhost");
            bazookaGhost.GetComponentInChildren<ParticleSystem>().gameObject.AddComponent<Modules.Components.DetachOnDestroy>();

            bazookaController.ghostPrefab = bazookaGhost;
            bazookaController.startSound = "";
        }

        private static void InitializeImpactExplosion(ProjectileImpactExplosion projectileImpactExplosion)
        {
            projectileImpactExplosion.blastDamageCoefficient = 1f;
            projectileImpactExplosion.blastProcCoefficient = 1f;
            projectileImpactExplosion.blastRadius = 1f;
            projectileImpactExplosion.bonusBlastForce = Vector3.zero;
            projectileImpactExplosion.childrenCount = 0;
            projectileImpactExplosion.childrenDamageCoefficient = 0f;
            projectileImpactExplosion.childrenProjectilePrefab = null;
            projectileImpactExplosion.destroyOnEnemy = false;
            projectileImpactExplosion.destroyOnWorld = false;
            projectileImpactExplosion.explosionSoundString = "";
            projectileImpactExplosion.falloffModel = RoR2.BlastAttack.FalloffModel.None;
            projectileImpactExplosion.fireChildren = false;
            projectileImpactExplosion.impactEffect = null;
            projectileImpactExplosion.lifetime = 0f;
            projectileImpactExplosion.lifetimeAfterImpact = 0f;
            projectileImpactExplosion.lifetimeExpiredSoundString = "";
            projectileImpactExplosion.lifetimeRandomOffset = 0f;
            projectileImpactExplosion.offsetForLifetimeExpiredSound = 0f;
            projectileImpactExplosion.timerAfterImpact = false;

            projectileImpactExplosion.GetComponent<ProjectileDamage>().damageType = DamageType.Generic;
        }

        private static GameObject CreateGhostPrefab(string ghostName)
        {
            GameObject ghostPrefab = Modules.Assets.mainAssetBundle.LoadAsset<GameObject>(ghostName);
            if (!ghostPrefab.GetComponent<NetworkIdentity>()) ghostPrefab.AddComponent<NetworkIdentity>();
            if (!ghostPrefab.GetComponent<ProjectileGhostController>()) ghostPrefab.AddComponent<ProjectileGhostController>();

            Modules.Assets.ConvertAllRenderersToHopooShader(ghostPrefab);

            return ghostPrefab;
        }

        private static GameObject CloneProjectilePrefab(string prefabName, string newPrefabName)
        {
            GameObject newPrefab = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/Projectiles/" + prefabName), newPrefabName);
            return newPrefab;
        }
    }
}