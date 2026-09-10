# Deucarian Projectiles Agent Notes

Package ID: `com.deucarian.projectiles`
Repository: `Deucarian/Projectiles`

Follow the canonical Deucarian governance docs in [Package Registry](https://github.com/Deucarian/Package-Registry/blob/main/ARCHITECTURE.md), especially capability ownership and dependency rules.

## Ownership

This package owns:

- Generic projectile definitions, launch requests, projectile runtime instances, movement/lifetime ticking, manual impact reporting, cleanup events, Combat damage request creation, and projectile diagnostics snapshots.

Registered capabilities:
- None.

This package must not own:

- Weapon slots/cooldowns, target acquisition, attack orchestration, physics hit discovery, damage resolution rules, rewards, progression, persistence, UI, VFX/audio, pathfinding, encounter scheduling, tower placement, ECS/DOTS, or product-specific projectile behavior.

## Dependencies

Allowed dependency shape:

- May depend on lower gameplay simulation packages that provide IDs, Combat damage request types, attack intent context, world movement, and world spawning.

Required dependencies and why:

- `com.deucarian.gameplay-foundation`: shared gameplay IDs and deterministic primitives.
- `com.deucarian.combat`: damage request types emitted after projectile impact.
- `com.deucarian.attacks`: attack intent/source context used by projectile launches.
- `com.deucarian.world-navigation`: supplied movement adapters for projectile travel.
- `com.deucarian.world-spawning`: generic spawn requests and spawned instance identity for projectile bodies.

Optional/version-defined dependencies:

- None.

Architecture exceptions:

- None.

## Policies

- Keep this package focused on projectile lifecycle and generic adapters.
- Do not add hard dependencies on Defense Games, Auto Defense, Weapon Systems, Run Upgrades, Progression, Persistence, UI, Game Content Authoring, or template packages.
- Higher-level weapons, targeting, physics hit detection, and genre-specific projectile rules belong in owning packages or caller adapters.
- Logging: Do not introduce direct Unity Debug calls.
- Unity object lifetime: Use Common only if production code directly owns transient Unity object cleanup.
- Testing: Test fixture teardown may use Unity `DestroyImmediate` directly.

## Validation

Run the shared validator before committing:

```powershell
python C:/Repositories/Package-Registry/Tools/deucarian_package_validator.py --registry-root C:/Repositories/Package-Registry --repository-root . --config deucarian-package.json
```

Also run existing repository tests when changing code or asmdefs. Documentation-only updates should still run `git diff --check`.

## Codex Guidance

- Inspect current files before changing anything.
- Work on `develop`; do not edit or merge `main` unless the task is promotion-only.
- Do not edit `Library/PackageCache`.
- Do not guess package versions or dependency versions.
- Do not add package dependencies casually; update asmdefs, `package.json`, `deucarian-package.json`, Package Registry, Package Installer fallback, and Bootstrap fallback together when a dependency is truly required.
- Do not create local copies of shared helpers.
- Keep commits focused and report exactly what changed and what was validated.
