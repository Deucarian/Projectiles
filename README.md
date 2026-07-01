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

## Install

Stable:

```json
"com.deucarian.projectiles": "https://github.com/Deucarian/Projectiles.git#main"
```

Development:

```json
"com.deucarian.projectiles": "https://github.com/Deucarian/Projectiles.git#develop"
```

Use `#main` for stable package consumption and `#develop` when testing active package work.

## When To Use This

Use this package when you need Generic projectile launch, movement, lifetime, impact, and cleanup foundations for Deucarian games.

Do not use this package to take ownership of capabilities outside its `AGENTS.md` boundary. Reusable behavior should stay with the package that owns that capability in the Package Registry governance docs.

## Quick Start

1. Install the package through Deucarian Package Installer or Unity Package Manager using the URL above.
2. Let Unity finish resolving packages and compiling assemblies.
3. Import the `Basic Projectile Lifecycle` sample if you want a working reference scene or setup.
4. Start from the package README sections above and the public runtime/editor APIs in this repository.

## Validation

Run the shared package validator from this repository root:

```powershell
python C:/Repositories/Package-Registry/Tools/deucarian_package_validator.py --registry-root C:/Repositories/Package-Registry --repository-root . --config deucarian-package.json
```

Documentation-only updates should still pass:

```powershell
git diff --check
```

## Troubleshooting

- Package does not resolve: confirm the stable or development Git URL matches the Package Registry entry and that required Deucarian dependencies are installed.
- Unity compile errors after install: let Package Manager finish resolving dependencies, then check asmdef references against `package.json` dependencies.
- Behavior appears to belong in another package: consult `AGENTS.md` and the Package Registry governance docs before moving or duplicating code.

## License

MIT. See `LICENSE.md`.
