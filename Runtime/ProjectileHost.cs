using System;
using Deucarian.Combat.Unity;
using Deucarian.Projectiles.Unity;
using Deucarian.WorldNavigation;
using Deucarian.WorldSpawning;
using UnityEngine;
namespace Deucarian.Projectiles
{
    /// <summary>Owns projectile state and ticks. The referenced hosts own movement and pooled objects.</summary>
    [DefaultExecutionOrder(-900), DisallowMultipleComponent]
    public sealed class ProjectileHost : MonoBehaviour
    {
        [SerializeField] private WorldSpawnHost spawning;
        [SerializeField] private WorldNavigationHost navigation;
        [SerializeField] private ProjectileDefinitionCatalog definitions;
        [SerializeField] private CombatDefinitionCatalog combatDefinitions;
        private ProjectileRuntime runtime;
        public ProjectileRuntime Runtime => runtime ?? throw new InvalidOperationException("Enable ProjectileHost with its spawning, navigation and definition catalogs configured before using it.");
        private void Awake()
        {
            if (spawning == null || navigation == null) throw new InvalidOperationException("Assign a WorldSpawnHost and WorldNavigationHost to ProjectileHost.");
            runtime = new ProjectileRuntime((combatDefinitions != null ? combatDefinitions : CombatDefinitionCatalog.LoadProject()).CreateCatalog(),
                (definitions != null ? definitions : ProjectileDefinitionCatalog.LoadProject()).CreateRuntimeDefinitions(),
                new WorldSpawnHostProjectileSpawner(spawning), new WorldNavigationProjectileNavigator(navigation.Service));
        }
        private void FixedUpdate() => runtime?.Tick(1);
        public void Clear()
        { if (runtime != null) foreach (var projectile in runtime.CreateSnapshot().Projectiles) runtime.Cleanup(projectile.Id); }
        private void OnDisable() => Clear();
        private void OnDestroy() { Clear(); runtime = null; }
    }
}
