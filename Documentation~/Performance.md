# Performance

Measured in `C:\Repositories\Deucarian\Projectiles-TestProject` with Unity `6000.3.5f1`.

Benchmark setup: fake pooled prefab represented by one empty `GameObject`; operation per cycle was launch, navigation start, manual impact, and cleanup by hit-limit expiry.

| Cycles | Elapsed | Mono Delta | Spawned | Despawned |
| ---: | ---: | ---: | ---: | ---: |
| 1,000 | 27 ms | -126,976 bytes | 1,000 | 1,000 |
| 5,000 | 113 ms | 1,572,864 bytes | 5,000 | 5,000 |
| 10,000 | 244 ms | 884,736 bytes | 10,000 | 10,000 |

The zero-tick steady-state allocation test after warm-up passed with less than 64 KiB mono delta for 256 representative evaluations. The benchmark still allocates because it creates test `GameObject` instances and test combat target IDs.
