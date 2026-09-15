# Changelog

## [0.3.0] - 2026-09-11

- Add typed reusable definition authoring and/or scoped Inspector components that share the existing C# service behavior.
- Include a playable Definition Workflow sample with configured hosts, short callers and usage documentation.
- Align declared package dependencies with the definition-authoring development wave.


## 0.2.1 - 2026-07-17

- Aligned package metadata and the playable sample with the portfolio contract; direct Deucarian dependencies now use the coordinated patch versions.

## 0.2.0

- Added direct World Spawning integration through `WorldSpawnProjectileSpawner`.
- Changed projectile spawnable identity from generic `ContentId` to `WorldSpawnableId`.
- Updated benchmarks to use `GC.GetAllocatedBytesForCurrentThread` instead of Mono heap deltas.

## 0.1.0

- Added projectile identity, definitions, launch requests, runtime records, snapshots, lifetime expiry, manual cleanup, and deterministic processing order.
- Added spawn and navigation adapter contracts.
- Added World Navigation transform adapter.
- Added manual impact reporting and Combat damage request creation.
- Added EditMode tests and validation documentation.
