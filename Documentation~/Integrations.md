# Integrations

## Attacks

Attacks can feed `AttackSourceId`, `AttackDefinitionId`, and `AttackSourceSnapshot` into `ProjectileLaunchRequest`. No Attacks API change was required in Phase 1K.

## Combat

`ProjectileDamageRequestFactory` maps projectile definition damage and source multiplier into `DamageResolutionRequest`. Combat resolves armor, resistance, criticals, shield, health, statuses, and death.

## World Navigation

`WorldNavigationProjectileNavigator` registers the spawned object transform and issues destination or path movement commands.

## World Spawning

Use `WorldSpawnProjectileSpawner` to launch projectile objects through `WorldSpawnService` with generic `WorldSpawnRequest` values. Projectiles does not require Encounters.

## Defense Games

Defense Games should decide tower cadence, lane target, and placement. It can translate a tower fire decision into a projectile launch request.
