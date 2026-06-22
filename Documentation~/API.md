# API

Namespace: `Deucarian.Projectiles`

- IDs: `ProjectileDefinitionId`, `ProjectileInstanceId`, `ProjectileSpawnHandle`
- Definitions: `ProjectileDefinition`
- Requests: `ProjectileLaunchRequest`, `ProjectileImpactRequest`
- Results: `ProjectileLaunchResult`, `ProjectileImpactResult`, `ProjectileTickResult`, `ProjectileExpiryEvent`
- Snapshots: `ProjectileRecordSnapshot`, `ProjectileSnapshot`
- Runtime: `ProjectileRuntime`
- Adapters: `IProjectileSpawner`, `IProjectileNavigator`, `IProjectileDamageRequestFactory`
- Provided integration: `WorldNavigationProjectileNavigator`, `ProjectileDamageRequestFactory`

`ProjectileRuntime` owns active projectile lifecycle only. It does not discover collisions. A physics, tower lane, idle battle, or donor adapter reports impacts by calling `ReportImpact`.

`ReportImpact` creates a `DamageResolutionRequest`; Combat remains responsible for damage math and health mutation through `CombatDamageResolver.Resolve`.
