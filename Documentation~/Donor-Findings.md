# Donor Findings

Donor project: `C:\Repositories\JorisHoef\Codex-Attempted-Vampire-Project\Codex-Attempted-Vampire-Project`

Representative files inspected:

- `Assets/Game/Code/Runtime/Weapons/ProjectileWeaponRuntime.cs`
- `Assets/Game/Code/Runtime/Combat/ProjectileActor.cs`
- `Assets/Game/Code/Runtime/Utilities/GameObjectPoolService.cs`
- weapon definitions and projectile prefab assets under `Assets/Game/Data/Weapons` and `Assets/Game/Prefabs`

## Clean Mappings

- Weapon source identity maps to `AttackSourceId` and `AttackDefinitionId`.
- Projectile prefab identity maps to `ProjectileDefinition.SpawnableId`.
- Lifetime, speed, damage element, base damage, and pierce count map to projectile definitions plus Combat.
- Per-projectile hit memory maps to runtime duplicate-target rejection.

## Adapter Required

- Donor physics overlap, enemy registry scans, chain/fork/return behavior, VFX/audio, and owner callbacks stay outside this package.
- Donor direct `Instantiate`/`Destroy` should be replaced by a World Spawning adapter when migrated.
- Donor per-projectile `Update` movement should be replaced by centralized World Navigation ticking.

## Discard

- Do not bring donor weapon cooldown, radial pattern, hitscan conversion, UI/resource hooks, or owner feedback callbacks into Projectiles.

The Phase 1K API is not survivor-game-specific; Idle Auto Defense and classic Tower Defense can use the same launch/impact contract with different targeting adapters.
