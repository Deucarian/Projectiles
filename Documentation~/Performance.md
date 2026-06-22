# Performance

Measured in `C:\Repositories\Deucarian\Projectiles-TestProject` with Unity `6000.3.5f1`.

Benchmark setup: fake pooled prefab represented by one empty `GameObject`; operation per cycle was launch, manual impact, and cleanup by hit-limit expiry. Allocation method is `GC.GetAllocatedBytesForCurrentThread` in Unity EditMode batch.

| Cycles | Elapsed | Allocated | Spawned | Despawned |
| ---: | ---: | ---: | ---: | ---: |
| 1,000 | 25 ms | 0 bytes | 1,000 | 1,000 |
| 5,000 | 105 ms | 0 bytes | 5,000 | 5,000 |
| 10,000 | 201 ms | 0 bytes | 10,000 | 10,000 |

These values are more reliable than Mono heap deltas, but still reflect Unity EditMode batch behavior and the fake adapter path. They are not a blanket allocation-free claim for all production adapters.
