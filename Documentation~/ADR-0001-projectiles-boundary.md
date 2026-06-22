# ADR 0001: Projectiles Boundary

## Decision

Projectiles owns projectile identity, definitions, launch requests, runtime state, lifetime expiry, manual impact signaling, Combat request creation, despawn cleanup, hit-count policy, deterministic ordering, snapshots, and adapter points for spawn/movement/hit discovery.

Projectiles does not own weapons, cooldowns, target discovery, physics overlap/raycast logic, tower placement, pathfinding, damage math, status effects, encounters, rewards, persistence, UI, VFX, audio, or ECS.

## Dependencies

Runtime depends on Gameplay Foundation, Attacks, Combat, and World Navigation. It does not depend on Defense Games, Encounters, Progression, Persistence, UI packages, Core State, service locators, global mutable state, or Entities.

World Spawning integration is represented through `IProjectileSpawner`. The current World Spawning public API consumes Encounters `SpawnRequest`, so a direct runtime adapter would introduce an Encounters dependency. That boundary is left to game/adapter assemblies.

## Consequences

The same API can support survivor projectiles, Idle Auto Defense shots, and classic tower-defense bolts because it models launch, travel, impact, and cleanup without owning targeting or weapon cadence.
