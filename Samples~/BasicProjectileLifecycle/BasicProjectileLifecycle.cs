using Deucarian.Attacks;
using Deucarian.Combat;
using Deucarian.GameplayFoundation;
using Deucarian.Projectiles;
using Deucarian.WorldNavigation;
using Deucarian.WorldSpawning;
using UnityEngine;

public static class BasicProjectileLifecycle
{
    public static void Run(IProjectileSpawner spawner, IProjectileNavigator navigator)
    {
        var physical = new DamageTypeId("physical");
        var catalog = new CombatCatalog(new[] { new DamageTypeDefinition(physical) });
        var definition = new ProjectileDefinition(new ProjectileDefinitionId("arrow"), new WorldSpawnableId("arrow.prefab"), physical, 10, 60, 8);
        var runtime = new ProjectileRuntime(catalog, new[] { definition }, spawner, navigator);
        var source = new AttackSourceSnapshot(new AttackSourceId("tower"), new CombatantId("tower.combatant"));
        ProjectileLaunchResult launch = runtime.Launch(new ProjectileLaunchRequest(definition.Id, source.Id, new AttackDefinitionId("basic-shot"), source, Vector3.zero, Vector3.forward * 8));
        if (!launch.Succeeded) return;

        var target = new HealthState(new CombatantId("enemy"), 100, 100);
        ProjectileImpactResult impact = runtime.ReportImpact(new ProjectileImpactRequest(launch.ProjectileId, target.Id, target));
        if (impact.Succeeded) CombatDamageResolver.Resolve(impact.DamageRequest);
    }
}
