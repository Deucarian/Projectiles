# Changelog

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
