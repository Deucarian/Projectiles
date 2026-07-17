# Basic Projectile Lifecycle

This sample composes caller-supplied spawning and navigation adapters with a projectile definition, launches one projectile, reports a manual impact, and resolves the emitted Combat damage request.

Open `BasicProjectileLifecycle.unity` for an importable sample scene, then use `BasicProjectileLifecycle.Run` with adapters backed by your game's world implementation. Physics hit discovery and presentation intentionally remain caller-owned.
