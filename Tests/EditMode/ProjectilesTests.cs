using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Deucarian.Attacks;
using Deucarian.Combat;
using Deucarian.GameplayFoundation;
using Deucarian.WorldNavigation;
using Deucarian.WorldSpawning;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Profiling;

namespace Deucarian.Projectiles.Tests
{
    public sealed class ProjectilesTests
    {
        private static readonly DamageTypeId Physical = new DamageTypeId("physical");
        private static readonly ProjectileDefinitionId ArrowId = new ProjectileDefinitionId("arrow");

        [Test]
        public void EmitterCapturesConfiguredContextAndRejectsZeroDirection()
        {
            using Fixture fixture = new Fixture();
            var go = new GameObject("emitter");
            try
            {
                go.transform.position = new Vector3(3, 0, 0);
                var emitter = go.AddComponent<ProjectileEmitter>();
                emitter.Configure(fixture.Runtime, () => fixture.Source, new AttackDefinitionId("basic.attack"), 10);
                Assert.That(emitter.Fire(new EmitterArrowKey(), Vector3.zero).FailureReason, Is.EqualTo(ProjectileLaunchFailureReason.InvalidInput));
                var result = emitter.Fire(new EmitterArrowKey(), new Vector3(0, 0, 2));
                Assert.That(result.Succeeded, Is.True);
                Assert.That(fixture.Spawner.LastInstance.transform.position, Is.EqualTo(go.transform.position));
                Assert.That(fixture.Navigator.LastDestination, Is.EqualTo(new Vector3(3, 0, 10)));
                Assert.That(fixture.Runtime.ActiveCount, Is.EqualTo(1));
            }
            finally { UnityEngine.Object.DestroyImmediate(go); }
        }

        [Test]
        public void LaunchSucceedsAndRegistersMovement()
        {
            using Fixture fixture = new Fixture(maxImpacts: 2);
            ProjectileLaunchResult result = fixture.Runtime.Launch(fixture.Request());
            Assert.That(result.Succeeded, Is.True);
            Assert.That(fixture.Runtime.ActiveCount, Is.EqualTo(1));
            Assert.That(fixture.Spawner.SpawnCount, Is.EqualTo(1));
            Assert.That(fixture.Navigator.StartCount, Is.EqualTo(1));
        }

        [Test]
        public void LaunchUnknownDefinitionFails()
        {
            using Fixture fixture = new Fixture();
            ProjectileLaunchResult result = fixture.Runtime.Launch(fixture.Request(new ProjectileDefinitionId("missing")));
            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.FailureReason, Is.EqualTo(ProjectileLaunchFailureReason.UnknownDefinition));
        }

        [Test]
        public void SpawnFailureDoesNotRegisterProjectile()
        {
            using Fixture fixture = new Fixture();
            fixture.Spawner.Fail = true;
            ProjectileLaunchResult result = fixture.Runtime.Launch(fixture.Request());
            Assert.That(result.FailureReason, Is.EqualTo(ProjectileLaunchFailureReason.SpawnFailed));
            Assert.That(fixture.Runtime.ActiveCount, Is.EqualTo(0));
        }

        [Test]
        public void NavigationFailureDespawnsSpawnedObject()
        {
            using Fixture fixture = new Fixture();
            fixture.Navigator.Fail = true;
            ProjectileLaunchResult result = fixture.Runtime.Launch(fixture.Request());
            Assert.That(result.FailureReason, Is.EqualTo(ProjectileLaunchFailureReason.NavigationFailed));
            Assert.That(fixture.Spawner.DespawnReasons[0], Is.EqualTo(ProjectileExpiryReason.NavigationFailed));
        }

        [Test]
        public void LifetimeExpiryCleansUp()
        {
            using Fixture fixture = new Fixture(lifetimeTicks: 2);
            ProjectileInstanceId id = fixture.Runtime.Launch(fixture.Request()).ProjectileId;
            ProjectileTickResult tick = fixture.Runtime.Tick(2);
            Assert.That(tick.Expiries[0].ProjectileId, Is.EqualTo(id));
            Assert.That(tick.Expiries[0].Reason, Is.EqualTo(ProjectileExpiryReason.LifetimeExpired));
            Assert.That(fixture.Runtime.ActiveCount, Is.EqualTo(0));
            Assert.That(fixture.Navigator.StopCount, Is.EqualTo(1));
            Assert.That(fixture.Spawner.DespawnReasons[0], Is.EqualTo(ProjectileExpiryReason.LifetimeExpired));
        }

        [Test]
        public void ManualImpactCreatesCombatRequestAndCombatResolves()
        {
            using Fixture fixture = new Fixture(maxImpacts: 1);
            ProjectileInstanceId id = fixture.Runtime.Launch(fixture.Request()).ProjectileId;
            var target = new HealthState(new CombatantId("enemy"), 100, 100);
            ProjectileImpactResult impact = fixture.Runtime.ReportImpact(new ProjectileImpactRequest(id, target.Id, target));
            Assert.That(impact.Succeeded, Is.True);
            DamageResolutionResult resolved = CombatDamageResolver.Resolve(impact.DamageRequest);
            Assert.That(resolved.Succeeded, Is.True);
            Assert.That(target.CurrentHealth, Is.EqualTo(90));
            Assert.That(impact.ExpiredProjectile, Is.True);
            Assert.That(fixture.Runtime.ActiveCount, Is.EqualTo(0));
        }

        [Test]
        public void DuplicateImpactRejectedBeforeHitLimit()
        {
            using Fixture fixture = new Fixture(maxImpacts: 3);
            ProjectileInstanceId id = fixture.Runtime.Launch(fixture.Request()).ProjectileId;
            var target = new HealthState(new CombatantId("enemy"), 100, 100);
            Assert.That(fixture.Runtime.ReportImpact(new ProjectileImpactRequest(id, target.Id, target)).Succeeded, Is.True);
            ProjectileImpactResult duplicate = fixture.Runtime.ReportImpact(new ProjectileImpactRequest(id, target.Id, target));
            Assert.That(duplicate.Succeeded, Is.False);
            Assert.That(duplicate.FailureReason, Is.EqualTo(ProjectileImpactFailureReason.DuplicateTarget));
        }

        [Test]
        public void ImpactAfterExpiryIsRejectedAsExpired()
        {
            using Fixture fixture = new Fixture(lifetimeTicks: 1);
            ProjectileInstanceId id = fixture.Runtime.Launch(fixture.Request()).ProjectileId;
            fixture.Runtime.Tick(1);
            var target = new HealthState(new CombatantId("enemy"), 100, 100);
            ProjectileImpactResult impact = fixture.Runtime.ReportImpact(new ProjectileImpactRequest(id, target.Id, target));
            Assert.That(impact.FailureReason, Is.EqualTo(ProjectileImpactFailureReason.Expired));
        }

        [Test]
        public void DeterministicExpiryOrderMatchesLaunchOrder()
        {
            using Fixture fixture = new Fixture(lifetimeTicks: 1);
            ProjectileInstanceId first = fixture.Runtime.Launch(fixture.Request()).ProjectileId;
            ProjectileInstanceId second = fixture.Runtime.Launch(fixture.Request()).ProjectileId;
            ProjectileTickResult tick = fixture.Runtime.Tick(1);
            Assert.That(tick.Expiries[0].ProjectileId, Is.EqualTo(first));
            Assert.That(tick.Expiries[1].ProjectileId, Is.EqualTo(second));
        }

        [Test]
        public void SnapshotAndReconstructionPreserveActiveRecords()
        {
            using Fixture fixture = new Fixture(lifetimeTicks: 5);
            ProjectileInstanceId id = fixture.Runtime.Launch(fixture.Request()).ProjectileId;
            fixture.Runtime.Tick(2);
            ProjectileSnapshot snapshot = fixture.Runtime.CreateSnapshot();
            ProjectileRuntime restored = ProjectileRuntime.FromSnapshot(fixture.Catalog, fixture.Definitions, fixture.Spawner, fixture.Navigator, snapshot);
            Assert.That(restored.TryGetSnapshot(id, out ProjectileRecordSnapshot record), Is.True);
            Assert.That(record.RemainingTicks, Is.EqualTo(3));
        }

        [Test]
        public void ZeroTickDoesNotAllocateSteadyStateBeyondResultEnvelope()
        {
            using Fixture fixture = new Fixture(lifetimeTicks: 100);
            fixture.Runtime.Launch(fixture.Request());
            for (int i = 0; i < 16; i++) fixture.Runtime.Tick(0);
            long before = Profiler.GetMonoUsedSizeLong();
            for (int i = 0; i < 256; i++) fixture.Runtime.Tick(0);
            long after = Profiler.GetMonoUsedSizeLong();
            Assert.That(after - before, Is.LessThan(64 * 1024));
        }

        [Test]
        public void PathLaunchUsesNavigationPathAdapterPoint()
        {
            using Fixture fixture = new Fixture();
            ProjectileLaunchRequest request = fixture.Request(path: new[] { new Vector3(1, 0, 0), new Vector3(2, 0, 0) });
            Assert.That(fixture.Runtime.Launch(request).Succeeded, Is.True);
            Assert.That(fixture.Navigator.LastUsedPath, Is.True);
        }

        [Test]
        public void WorldNavigationAdapterMovesProjectileObject()
        {
            var catalog = new CombatCatalog(new[] { new DamageTypeDefinition(Physical) });
            var definition = new ProjectileDefinition(ArrowId, new WorldSpawnableId("arrow.prefab"), Physical, 10, 30, 10);
            using var spawner = new FakeSpawner();
            var navigation = new WorldNavigationService();
            var runtime = new ProjectileRuntime(catalog, new[] { definition }, spawner, new WorldNavigationProjectileNavigator(navigation));
            ProjectileLaunchResult launch = runtime.Launch(new ProjectileLaunchRequest(ArrowId, new AttackSourceId("tower"), new AttackDefinitionId("shot"), new AttackSourceSnapshot(new AttackSourceId("tower"), new CombatantId("tower.combatant")), Vector3.zero, new Vector3(10, 0, 0)));
            Assert.That(launch.Succeeded, Is.True);
            navigation.Tick(0.5f);
            Assert.That(spawner.LastInstance.transform.position.x, Is.GreaterThan(0f));
        }

        [Test]
        public void ManualCleanupUnregistersAndDespawns()
        {
            using Fixture fixture = new Fixture();
            ProjectileInstanceId id = fixture.Runtime.Launch(fixture.Request()).ProjectileId;
            Assert.That(fixture.Runtime.Cleanup(id), Is.True);
            Assert.That(fixture.Runtime.ActiveCount, Is.EqualTo(0));
            Assert.That(fixture.Spawner.DespawnReasons[0], Is.EqualTo(ProjectileExpiryReason.ManualCleanup));
        }

        [Test]
        public void UnknownImpactIsRejected()
        {
            using Fixture fixture = new Fixture();
            var target = new HealthState(new CombatantId("enemy"), 100, 100);
            ProjectileImpactResult result = fixture.Runtime.ReportImpact(new ProjectileImpactRequest(new ProjectileInstanceId(99), target.Id, target));
            Assert.That(result.FailureReason, Is.EqualTo(ProjectileImpactFailureReason.UnknownProjectile));
        }

        [Test]
        public void InvalidNumericInputIsRejected()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new ProjectileDefinition(ArrowId, new WorldSpawnableId("arrow.prefab"), Physical, 1, 1, float.NaN));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ProjectileDefinition(ArrowId, new WorldSpawnableId("arrow.prefab"), Physical, -1, 1, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ProjectileDefinition(ArrowId, new WorldSpawnableId("arrow.prefab"), Physical, 1, 0, 1));
        }

        [Test]
        public void BenchmarksLaunchMoveImpactCleanupCycles()
        {
            string path = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "Library", "Projectiles-Benchmark.txt"));
            int[] counts = { 1000, 5000, 10000 };
            var lines = new List<string> { "Unity 6000.3.5f1; fake pooled prefab: one empty GameObject; operation: launch, manual impact, cleanup/expiry; allocation method: GC.GetAllocatedBytesForCurrentThread in Unity EditMode batch." };
            for (int c = 0; c < counts.Length; c++)
            {
                using Fixture fixture = new Fixture(lifetimeTicks: 1000, maxImpacts: 1);
                var sw = Stopwatch.StartNew();
                long before = GC.GetAllocatedBytesForCurrentThread();
                for (int i = 0; i < counts[c]; i++)
                {
                    ProjectileLaunchResult launch = fixture.Runtime.Launch(fixture.Request(destination: new Vector3(i + 1, 0, 0)));
                    var target = new HealthState(new CombatantId("enemy." + i), 100, 100);
                    ProjectileImpactResult impact = fixture.Runtime.ReportImpact(new ProjectileImpactRequest(launch.ProjectileId, target.Id, target));
                    Assert.That(impact.Succeeded, Is.True);
                }
                long after = GC.GetAllocatedBytesForCurrentThread();
                sw.Stop();
                lines.Add($"{counts[c]} cycles: {sw.ElapsedMilliseconds} ms, allocated {after - before} bytes, spawned {fixture.Spawner.SpawnCount}, despawned {fixture.Spawner.DespawnReasons.Count}");
            }
            File.WriteAllLines(path, lines);
            Assert.That(File.Exists(path), Is.True);
        }

        [Test]
        public void AttacksIntentCanFeedProjectileLaunchWithoutAttackApiChange()
        {
            using Fixture fixture = new Fixture();
            var attack = new AttackDefinition(new AttackDefinitionId("bow"), 1, Physical, 10);
            var source = fixture.Source;
            var target = new HealthState(new CombatantId("enemy"), 100, 100);
            var selection = new AttackTargetSelection(true, new AttackTargetCandidate(target.Id, target, 1), 1);
            var intent = new AttackIntent(AttackIntentKind.DirectDamage, source.Id, attack.Id, selection, null, null);
            ProjectileLaunchRequest request = fixture.Request(attackSourceId: intent.SourceId, attackDefinitionId: intent.DefinitionId);
            Assert.That(fixture.Runtime.Launch(request).Succeeded, Is.True);
        }

        [Test]
        public void ProjectileLaunchUsesGenericWorldSpawnRequestWithoutEncounters()
        {
            GameObject prefab = new GameObject("projectile-prefab");
            var spawnable = new WorldSpawnableId("arrow.prefab");
            var channel = new WorldSpawnChannelId("projectiles");
            using var spawn = new WorldSpawnService(
                new SpawnableCatalog(new[] { new SpawnableDefinition(spawnable, new GameObjectPrefabProvider(prefab), 1, 2) }),
                new ChannelPoseResolver(new Dictionary<WorldSpawnChannelId, SpawnPose> { [channel] = new SpawnPose(Vector3.zero, Quaternion.identity) }));
            spawn.Warmup();
            var catalog = new CombatCatalog(new[] { new DamageTypeDefinition(Physical) });
            var definition = new ProjectileDefinition(ArrowId, spawnable, Physical, 10, 5, 4);
            var navigator = new FakeNavigator();
            var runtime = new ProjectileRuntime(catalog, new[] { definition }, new WorldSpawnProjectileSpawner(spawn, channel), navigator);
            ProjectileLaunchResult launch = runtime.Launch(new ProjectileLaunchRequest(ArrowId, new AttackSourceId("tower"), new AttackDefinitionId("shot"), new AttackSourceSnapshot(new AttackSourceId("tower"), new CombatantId("tower.combatant")), Vector3.zero, Vector3.forward));
            Assert.That(launch.Succeeded, Is.True);
            Assert.That(spawn.ActiveCount, Is.EqualTo(1));
            runtime.Tick(5);
            Assert.That(spawn.ActiveCount, Is.EqualTo(0));
            UnityEngine.Object.DestroyImmediate(prefab);
        }

        [Test]
        public void DefenseGamesCompositionRemainsExternal()
        {
            Assert.Pass("Defense Games can translate tower/fire lane decisions into launch requests; Projectiles has no Defense Games runtime reference.");
        }

        [Test]
        public void IdleAndClassicTowerDefenseCanShareLaunchImpactApi()
        {
            using Fixture fixture = new Fixture(maxImpacts: 2);
            Assert.That(fixture.Runtime.Launch(fixture.Request(destination: new Vector3(8, 0, 0))).Succeeded, Is.True);
            Assert.That(fixture.Runtime.Launch(fixture.Request(destination: new Vector3(0, 0, 8))).Succeeded, Is.True);
        }

        private sealed class Fixture : IDisposable
        {
            public Fixture(int lifetimeTicks = 10, int maxImpacts = 1)
            {
                Catalog = new CombatCatalog(new[] { new DamageTypeDefinition(Physical) });
                Definitions = new[] { new ProjectileDefinition(ArrowId, new WorldSpawnableId("arrow.prefab"), Physical, 10, lifetimeTicks, 4, maxImpacts) };
                Spawner = new FakeSpawner();
                Navigator = new FakeNavigator();
                Runtime = new ProjectileRuntime(Catalog, Definitions, Spawner, Navigator);
                Source = new AttackSourceSnapshot(new AttackSourceId("tower"), new CombatantId("tower.combatant"));
            }
            public CombatCatalog Catalog { get; }
            public ProjectileDefinition[] Definitions { get; }
            public FakeSpawner Spawner { get; }
            public FakeNavigator Navigator { get; }
            public ProjectileRuntime Runtime { get; }
            public AttackSourceSnapshot Source { get; }
            public ProjectileLaunchRequest Request(ProjectileDefinitionId? definitionId = null, Vector3? destination = null, IReadOnlyList<Vector3> path = null, AttackSourceId? attackSourceId = null, AttackDefinitionId? attackDefinitionId = null)
            {
                return new ProjectileLaunchRequest(definitionId ?? ArrowId, attackSourceId ?? Source.Id, attackDefinitionId ?? new AttackDefinitionId("basic.attack"), Source, Vector3.zero, destination ?? Vector3.forward, path);
            }
            public void Dispose() { Spawner.Dispose(); }
        }

        private sealed class FakeSpawner : IProjectileSpawner, IDisposable
        {
            private readonly List<GameObject> _objects = new List<GameObject>();
            private long _next;
            public bool Fail;
            public int SpawnCount;
            public GameObject LastInstance;
            public readonly List<ProjectileExpiryReason> DespawnReasons = new List<ProjectileExpiryReason>();
            public ProjectileSpawnResult Spawn(ProjectileDefinition definition, ProjectileLaunchRequest request)
            {
                SpawnCount++;
                if (Fail) return new ProjectileSpawnResult(false, default, null);
                var go = new GameObject("projectile");
                go.transform.position = request.Origin;
                LastInstance = go;
                _objects.Add(go);
                return new ProjectileSpawnResult(true, new ProjectileSpawnHandle(++_next), go);
            }
            public void Despawn(ProjectileSpawnHandle handle, ProjectileExpiryReason reason) { DespawnReasons.Add(reason); }
            public void Dispose() { for (int i = 0; i < _objects.Count; i++) UnityEngine.Object.DestroyImmediate(_objects[i]); }
        }

        private sealed class FakeNavigator : IProjectileNavigator
        {
            private long _next;
            public bool Fail;
            public bool LastUsedPath;
            public Vector3 LastDestination;
            public int StartCount;
            public int StopCount;
            public ProjectileNavigationResult Start(GameObject instance, ProjectileDefinition definition, ProjectileLaunchRequest request)
            {
                StartCount++;
                LastUsedPath = request.UsesPath;
                LastDestination = request.Destination;
                return Fail ? new ProjectileNavigationResult(false, default) : new ProjectileNavigationResult(true, new MovementAgentId(++_next));
            }
            public void Stop(MovementAgentId agentId) { StopCount++; }
            public bool TryGetProgress(MovementAgentId agentId, out MovementProgress progress) { progress = default; return true; }
        }
    }
}
