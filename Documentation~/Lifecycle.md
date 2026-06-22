# Lifecycle

1. A weapon, tower, trap, or idle battle system selects targets and creates a `ProjectileLaunchRequest`.
2. `ProjectileRuntime.Launch` validates the definition, asks `IProjectileSpawner` for an object, then asks `IProjectileNavigator` to move it.
3. `ProjectileRuntime.Tick` decrements fixed lifetime ticks in launch order.
4. Hit discovery remains outside the package. Physics or deterministic lane logic calls `ReportImpact`.
5. `ReportImpact` rejects duplicate targets for the same projectile, creates a Combat request, and expires the projectile when `MaxImpacts` is reached.
6. Expiry and manual cleanup stop navigation and despawn through adapters.
