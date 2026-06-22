# Deucarian Projectiles

`com.deucarian.projectiles` provides generic projectile launch, movement, lifetime, manual impact reporting, Combat request creation, and cleanup.

The package intentionally does not own weapons, cooldowns, targeting, physics hit discovery, damage math, rewards, persistence, UI, VFX, audio, pathfinding, placement, encounters, or ECS. Games or adapters report impacts into the runtime.

## Runtime Dependencies

- `com.deucarian.gameplay-foundation`
- `com.deucarian.combat`
- `com.deucarian.attacks`
- `com.deucarian.world-navigation`
- `com.deucarian.world-spawning`

Projectiles uses World Spawning directly through generic `WorldSpawnRequest` values. It still has no Encounters dependency.

## Minimal Flow

1. Create `ProjectileDefinition` entries.
2. Provide `IProjectileSpawner` and `IProjectileNavigator`.
3. Construct `ProjectileRuntime`.
4. Call `Launch`.
5. Tick lifetime with `Tick`.
6. Report physics or game-rule hits with `ReportImpact`.
7. Resolve the returned `DamageResolutionRequest` through Combat.

See `Samples~/BasicProjectileLifecycle`.
