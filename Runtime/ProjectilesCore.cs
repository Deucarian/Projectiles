using System;
using System.Collections.Generic;
using Deucarian.Attacks;
using Deucarian.Combat;
using Deucarian.GameplayFoundation;
using Deucarian.WorldNavigation;
using UnityEngine;

namespace Deucarian.Projectiles
{
    /// <summary>Stable authored identifier for a projectile definition.</summary>
    public readonly struct ProjectileDefinitionId : IEquatable<ProjectileDefinitionId>, IComparable<ProjectileDefinitionId>
    {
        private readonly ContentId _value;
        public ProjectileDefinitionId(string value) { _value = new ContentId(value); }
        public string Value => _value.Value;
        public bool IsEmpty => _value.IsEmpty;
        public bool Equals(ProjectileDefinitionId other) => _value.Equals(other._value);
        public override bool Equals(object obj) => obj is ProjectileDefinitionId other && Equals(other);
        public override int GetHashCode() => _value.GetHashCode();
        public int CompareTo(ProjectileDefinitionId other) => _value.CompareTo(other._value);
        public override string ToString() => Value;
    }

    /// <summary>Runtime identifier assigned in deterministic launch order.</summary>
    public readonly struct ProjectileInstanceId : IEquatable<ProjectileInstanceId>, IComparable<ProjectileInstanceId>
    {
        public ProjectileInstanceId(long value) { if (value <= 0) throw new ArgumentOutOfRangeException(nameof(value)); Value = value; }
        public long Value { get; }
        public bool Equals(ProjectileInstanceId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is ProjectileInstanceId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
        public int CompareTo(ProjectileInstanceId other) => Value.CompareTo(other.Value);
        public override string ToString() => Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
    }

    /// <summary>Opaque object lifecycle handle supplied by a spawn adapter.</summary>
    public readonly struct ProjectileSpawnHandle : IEquatable<ProjectileSpawnHandle>
    {
        public ProjectileSpawnHandle(long value) { if (value <= 0) throw new ArgumentOutOfRangeException(nameof(value)); Value = value; }
        public long Value { get; }
        public bool Equals(ProjectileSpawnHandle other) => Value == other.Value;
        public override bool Equals(object obj) => obj is ProjectileSpawnHandle other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
    }

    public enum ProjectileLaunchFailureReason { None = 0, UnknownDefinition = 1, SpawnFailed = 2, NavigationFailed = 3, InvalidInput = 4 }
    public enum ProjectileImpactFailureReason { None = 0, UnknownProjectile = 1, Expired = 2, DuplicateTarget = 3, InvalidInput = 4 }
    public enum ProjectileExpiryReason { None = 0, LifetimeExpired = 1, HitLimitReached = 2, ManualCleanup = 3, NavigationFailed = 4 }
    public enum ProjectileLifecycle { Active = 0, Expired = 1 }

    /// <summary>Authored projectile data that remains weapon-agnostic.</summary>
    public sealed class ProjectileDefinition
    {
        public ProjectileDefinition(ProjectileDefinitionId id, ContentId spawnableId, DamageTypeId damageTypeId, double baseDamage, int lifetimeTicks, float speed, int maxImpacts = 1)
        {
            if (id.IsEmpty) throw new ArgumentException("Projectile definition id cannot be empty.", nameof(id));
            if (spawnableId.IsEmpty) throw new ArgumentException("Spawnable id cannot be empty.", nameof(spawnableId));
            if (damageTypeId.IsEmpty) throw new ArgumentException("Damage type id cannot be empty.", nameof(damageTypeId));
            CombatNumbers.RequireNonNegative(baseDamage, nameof(baseDamage));
            if (lifetimeTicks <= 0) throw new ArgumentOutOfRangeException(nameof(lifetimeTicks));
            if (speed < 0f || float.IsNaN(speed) || float.IsInfinity(speed)) throw new ArgumentOutOfRangeException(nameof(speed));
            if (maxImpacts <= 0) throw new ArgumentOutOfRangeException(nameof(maxImpacts));
            Id = id; SpawnableId = spawnableId; DamageTypeId = damageTypeId; BaseDamage = baseDamage; LifetimeTicks = lifetimeTicks; Speed = speed; MaxImpacts = maxImpacts;
        }
        public ProjectileDefinitionId Id { get; }
        public ContentId SpawnableId { get; }
        public DamageTypeId DamageTypeId { get; }
        public double BaseDamage { get; }
        public int LifetimeTicks { get; }
        public float Speed { get; }
        public int MaxImpacts { get; }
    }

    /// <summary>Caller-owned launch request. Targeting and weapon cooldowns happen before this point.</summary>
    public readonly struct ProjectileLaunchRequest
    {
        public ProjectileLaunchRequest(ProjectileDefinitionId definitionId, AttackSourceId attackSourceId, AttackDefinitionId attackDefinitionId, AttackSourceSnapshot source, Vector3 origin, Vector3 destination, IReadOnlyList<Vector3> path = null)
        {
            if (definitionId.IsEmpty) throw new ArgumentException("Definition id cannot be empty.", nameof(definitionId));
            DefinitionId = definitionId; AttackSourceId = attackSourceId; AttackDefinitionId = attackDefinitionId; Source = source; Origin = origin; Destination = destination; Path = Copy(path);
        }
        public ProjectileDefinitionId DefinitionId { get; }
        public AttackSourceId AttackSourceId { get; }
        public AttackDefinitionId AttackDefinitionId { get; }
        public AttackSourceSnapshot Source { get; }
        public Vector3 Origin { get; }
        public Vector3 Destination { get; }
        public IReadOnlyList<Vector3> Path { get; }
        public bool UsesPath => Path.Count > 0;
        private static Vector3[] Copy(IReadOnlyList<Vector3> source) { if (source == null) return Array.Empty<Vector3>(); var copy = new Vector3[source.Count]; for (int i = 0; i < source.Count; i++) copy[i] = source[i]; return copy; }
    }

    public readonly struct ProjectileSpawnResult
    {
        public ProjectileSpawnResult(bool succeeded, ProjectileSpawnHandle handle, GameObject instance)
        {
            Succeeded = succeeded; Handle = handle; Instance = instance;
        }
        public bool Succeeded { get; }
        public ProjectileSpawnHandle Handle { get; }
        public GameObject Instance { get; }
    }

    public interface IProjectileSpawner
    {
        ProjectileSpawnResult Spawn(ProjectileDefinition definition, ProjectileLaunchRequest request);
        void Despawn(ProjectileSpawnHandle handle, ProjectileExpiryReason reason);
    }

    public readonly struct ProjectileNavigationResult
    {
        public ProjectileNavigationResult(bool succeeded, MovementAgentId agentId)
        {
            Succeeded = succeeded; AgentId = agentId;
        }
        public bool Succeeded { get; }
        public MovementAgentId AgentId { get; }
    }

    public interface IProjectileNavigator
    {
        ProjectileNavigationResult Start(GameObject instance, ProjectileDefinition definition, ProjectileLaunchRequest request);
        void Stop(MovementAgentId agentId);
        bool TryGetProgress(MovementAgentId agentId, out MovementProgress progress);
    }

    /// <summary>World Navigation adapter for projectile transform movement.</summary>
    public sealed class WorldNavigationProjectileNavigator : IProjectileNavigator
    {
        private readonly WorldNavigationService _service;
        private readonly float _reachDistance;
        public WorldNavigationProjectileNavigator(WorldNavigationService service, float reachDistance = 0.001f)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service)); _reachDistance = Mathf.Max(0f, reachDistance);
        }
        public ProjectileNavigationResult Start(GameObject instance, ProjectileDefinition definition, ProjectileLaunchRequest request)
        {
            if (instance == null || definition == null) return new ProjectileNavigationResult(false, default);
            var handle = _service.Register(new TransformMovementPoseAccessor(instance.transform), new ConstantMovementSpeedProvider(definition.Speed));
            MovementResult command;
            if (request.UsesPath)
            {
                var waypoints = new MovementWaypoint[request.Path.Count];
                for (int i = 0; i < waypoints.Length; i++) waypoints[i] = new MovementWaypoint(request.Path[i]);
                command = _service.FollowPath(handle.Id, new MovementPath(waypoints), _reachDistance);
            }
            else command = _service.SetDestination(handle.Id, request.Destination, _reachDistance);
            if (!command.Succeeded) { _service.CleanupDespawned(handle.Id); return new ProjectileNavigationResult(false, handle.Id); }
            return new ProjectileNavigationResult(true, handle.Id);
        }
        public void Stop(MovementAgentId agentId) { _service.CleanupDespawned(agentId); }
        public bool TryGetProgress(MovementAgentId agentId, out MovementProgress progress) => _service.TryGetProgress(agentId, out progress);
    }

    public readonly struct ProjectileImpactRequest
    {
        public ProjectileImpactRequest(ProjectileInstanceId projectileId, CombatantId targetId, HealthState targetHealth, CombatDefenseSnapshot defense = null)
        {
            if (targetId.IsEmpty) throw new ArgumentException("Target id cannot be empty.", nameof(targetId));
            ProjectileId = projectileId; TargetId = targetId; TargetHealth = targetHealth; Defense = defense ?? new CombatDefenseSnapshot();
        }
        public ProjectileInstanceId ProjectileId { get; }
        public CombatantId TargetId { get; }
        public HealthState TargetHealth { get; }
        public CombatDefenseSnapshot Defense { get; }
    }

    public interface IProjectileDamageRequestFactory
    {
        DamageResolutionRequest Create(CombatCatalog catalog, ProjectileDefinition definition, ProjectileRecordSnapshot projectile, ProjectileImpactRequest impact);
    }

    public sealed class ProjectileDamageRequestFactory : IProjectileDamageRequestFactory
    {
        public DamageResolutionRequest Create(CombatCatalog catalog, ProjectileDefinition definition, ProjectileRecordSnapshot projectile, ProjectileImpactRequest impact)
        {
            if (catalog == null || definition == null || impact.TargetHealth == null) return new DamageResolutionRequest(catalog, null, null, null);
            double amount = definition.BaseDamage * projectile.Source.DamageMultiplier;
            var damage = new DamageRequest(impact.TargetId, new[] { new DamageComponent(definition.DamageTypeId, amount) }, projectile.Source.CombatSource, impact.Defense, projectile.Source.CombatantId);
            return new DamageResolutionRequest(catalog, impact.TargetHealth, null, damage);
        }
    }

    public readonly struct ProjectileRecordSnapshot
    {
        public ProjectileRecordSnapshot(ProjectileInstanceId id, ProjectileDefinitionId definitionId, AttackSourceId attackSourceId, AttackDefinitionId attackDefinitionId, AttackSourceSnapshot source, ProjectileSpawnHandle spawnHandle, MovementAgentId movementAgentId, int remainingTicks, int impactCount, ProjectileLifecycle lifecycle)
        {
            Id = id; DefinitionId = definitionId; AttackSourceId = attackSourceId; AttackDefinitionId = attackDefinitionId; Source = source; SpawnHandle = spawnHandle; MovementAgentId = movementAgentId; RemainingTicks = remainingTicks; ImpactCount = impactCount; Lifecycle = lifecycle;
        }
        public ProjectileInstanceId Id { get; }
        public ProjectileDefinitionId DefinitionId { get; }
        public AttackSourceId AttackSourceId { get; }
        public AttackDefinitionId AttackDefinitionId { get; }
        public AttackSourceSnapshot Source { get; }
        public ProjectileSpawnHandle SpawnHandle { get; }
        public MovementAgentId MovementAgentId { get; }
        public int RemainingTicks { get; }
        public int ImpactCount { get; }
        public ProjectileLifecycle Lifecycle { get; }
    }

    public sealed class ProjectileSnapshot
    {
        public ProjectileSnapshot(IReadOnlyList<ProjectileRecordSnapshot> projectiles, long nextInstanceId)
        {
            Projectiles = Copy(projectiles); NextInstanceId = nextInstanceId;
        }
        public IReadOnlyList<ProjectileRecordSnapshot> Projectiles { get; }
        public long NextInstanceId { get; }
        private static T[] Copy<T>(IReadOnlyList<T> source) { if (source == null) return Array.Empty<T>(); var copy = new T[source.Count]; for (int i = 0; i < source.Count; i++) copy[i] = source[i]; return copy; }
    }

    public readonly struct ProjectileLaunchResult
    {
        public ProjectileLaunchResult(bool succeeded, ProjectileLaunchFailureReason failureReason, ProjectileInstanceId projectileId)
        {
            Succeeded = succeeded; FailureReason = failureReason; ProjectileId = projectileId;
        }
        public bool Succeeded { get; }
        public ProjectileLaunchFailureReason FailureReason { get; }
        public ProjectileInstanceId ProjectileId { get; }
    }

    public readonly struct ProjectileImpactResult
    {
        public ProjectileImpactResult(bool succeeded, ProjectileImpactFailureReason failureReason, bool expiredProjectile, DamageResolutionRequest damageRequest)
        {
            Succeeded = succeeded; FailureReason = failureReason; ExpiredProjectile = expiredProjectile; DamageRequest = damageRequest;
        }
        public bool Succeeded { get; }
        public ProjectileImpactFailureReason FailureReason { get; }
        public bool ExpiredProjectile { get; }
        public DamageResolutionRequest DamageRequest { get; }
    }

    public readonly struct ProjectileExpiryEvent
    {
        public ProjectileExpiryEvent(ProjectileInstanceId projectileId, ProjectileExpiryReason reason)
        {
            ProjectileId = projectileId; Reason = reason;
        }
        public ProjectileInstanceId ProjectileId { get; }
        public ProjectileExpiryReason Reason { get; }
    }

    public sealed class ProjectileTickResult
    {
        public ProjectileTickResult(IReadOnlyList<ProjectileExpiryEvent> expiries) { Expiries = Copy(expiries); }
        public IReadOnlyList<ProjectileExpiryEvent> Expiries { get; }
        private static T[] Copy<T>(IReadOnlyList<T> source) { if (source == null) return Array.Empty<T>(); var copy = new T[source.Count]; for (int i = 0; i < source.Count; i++) copy[i] = source[i]; return copy; }
    }

    public sealed class ProjectileRuntime
    {
        private readonly CombatCatalog _catalog;
        private readonly IProjectileSpawner _spawner;
        private readonly IProjectileNavigator _navigator;
        private readonly IProjectileDamageRequestFactory _factory;
        private readonly Dictionary<ProjectileDefinitionId, ProjectileDefinition> _definitions = new Dictionary<ProjectileDefinitionId, ProjectileDefinition>();
        private readonly Dictionary<ProjectileInstanceId, State> _projectiles = new Dictionary<ProjectileInstanceId, State>();
        private readonly Dictionary<ProjectileInstanceId, HashSet<CombatantId>> _hits = new Dictionary<ProjectileInstanceId, HashSet<CombatantId>>();
        private readonly HashSet<ProjectileInstanceId> _expired = new HashSet<ProjectileInstanceId>();
        private readonly List<ProjectileInstanceId> _ordered = new List<ProjectileInstanceId>();
        private long _nextInstanceId;

        public ProjectileRuntime(CombatCatalog catalog, IReadOnlyList<ProjectileDefinition> definitions, IProjectileSpawner spawner, IProjectileNavigator navigator, IProjectileDamageRequestFactory factory = null)
        {
            _catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
            _spawner = spawner ?? throw new ArgumentNullException(nameof(spawner));
            _navigator = navigator ?? throw new ArgumentNullException(nameof(navigator));
            _factory = factory ?? new ProjectileDamageRequestFactory();
            if (definitions == null || definitions.Count == 0) throw new ArgumentException("At least one projectile definition is required.", nameof(definitions));
            for (int i = 0; i < definitions.Count; i++)
            {
                ProjectileDefinition definition = definitions[i] ?? throw new ArgumentException("Projectile definition cannot be null.");
                if (_definitions.ContainsKey(definition.Id)) throw new ArgumentException("Duplicate projectile definition: " + definition.Id);
                _definitions.Add(definition.Id, definition);
            }
        }

        public int ActiveCount => _projectiles.Count;

        public ProjectileLaunchResult Launch(ProjectileLaunchRequest request)
        {
            if (!_definitions.TryGetValue(request.DefinitionId, out ProjectileDefinition definition))
                return new ProjectileLaunchResult(false, ProjectileLaunchFailureReason.UnknownDefinition, default);
            ProjectileSpawnResult spawn = _spawner.Spawn(definition, request);
            if (!spawn.Succeeded || spawn.Instance == null)
                return new ProjectileLaunchResult(false, ProjectileLaunchFailureReason.SpawnFailed, default);
            ProjectileNavigationResult navigation = _navigator.Start(spawn.Instance, definition, request);
            if (!navigation.Succeeded)
            {
                _spawner.Despawn(spawn.Handle, ProjectileExpiryReason.NavigationFailed);
                return new ProjectileLaunchResult(false, ProjectileLaunchFailureReason.NavigationFailed, default);
            }
            var id = new ProjectileInstanceId(++_nextInstanceId);
            var snapshot = new ProjectileRecordSnapshot(id, definition.Id, request.AttackSourceId, request.AttackDefinitionId, request.Source, spawn.Handle, navigation.AgentId, definition.LifetimeTicks, 0, ProjectileLifecycle.Active);
            _projectiles.Add(id, new State(snapshot));
            _hits.Add(id, new HashSet<CombatantId>());
            _ordered.Add(id);
            return new ProjectileLaunchResult(true, ProjectileLaunchFailureReason.None, id);
        }

        public ProjectileTickResult Tick(int ticks)
        {
            if (ticks < 0) throw new ArgumentOutOfRangeException(nameof(ticks));
            var expiries = new List<ProjectileExpiryEvent>();
            if (ticks == 0) return new ProjectileTickResult(expiries);
            var toExpire = new List<ProjectileInstanceId>();
            for (int i = 0; i < _ordered.Count; i++)
            {
                ProjectileInstanceId id = _ordered[i];
                State state = _projectiles[id];
                int remaining = state.Snapshot.RemainingTicks - ticks;
                state.Snapshot = WithRemaining(state.Snapshot, Math.Max(0, remaining));
                if (remaining <= 0) toExpire.Add(id);
            }
            for (int i = 0; i < toExpire.Count; i++) expiries.Add(Expire(toExpire[i], ProjectileExpiryReason.LifetimeExpired));
            return new ProjectileTickResult(expiries);
        }

        public ProjectileImpactResult ReportImpact(ProjectileImpactRequest impact)
        {
            if (!_projectiles.TryGetValue(impact.ProjectileId, out State state))
                return new ProjectileImpactResult(false, _expired.Contains(impact.ProjectileId) ? ProjectileImpactFailureReason.Expired : ProjectileImpactFailureReason.UnknownProjectile, false, null);
            if (state.Snapshot.Lifecycle != ProjectileLifecycle.Active)
                return new ProjectileImpactResult(false, ProjectileImpactFailureReason.Expired, false, null);
            if (impact.TargetHealth == null || !impact.TargetHealth.Id.Equals(impact.TargetId))
                return new ProjectileImpactResult(false, ProjectileImpactFailureReason.InvalidInput, false, null);
            HashSet<CombatantId> hits = _hits[impact.ProjectileId];
            if (!hits.Add(impact.TargetId))
                return new ProjectileImpactResult(false, ProjectileImpactFailureReason.DuplicateTarget, false, null);
            ProjectileDefinition definition = _definitions[state.Snapshot.DefinitionId];
            DamageResolutionRequest damage = _factory.Create(_catalog, definition, state.Snapshot, impact);
            int impactCount = state.Snapshot.ImpactCount + 1;
            state.Snapshot = WithImpactCount(state.Snapshot, impactCount);
            bool expired = impactCount >= definition.MaxImpacts;
            if (expired) Expire(impact.ProjectileId, ProjectileExpiryReason.HitLimitReached);
            return new ProjectileImpactResult(true, ProjectileImpactFailureReason.None, expired, damage);
        }

        public bool Cleanup(ProjectileInstanceId id, ProjectileExpiryReason reason = ProjectileExpiryReason.ManualCleanup)
        {
            if (!_projectiles.ContainsKey(id)) return false;
            Expire(id, reason);
            return true;
        }

        public bool TryGetSnapshot(ProjectileInstanceId id, out ProjectileRecordSnapshot snapshot)
        {
            if (_projectiles.TryGetValue(id, out State state)) { snapshot = state.Snapshot; return true; }
            snapshot = default; return false;
        }

        public ProjectileSnapshot CreateSnapshot()
        {
            var records = new ProjectileRecordSnapshot[_ordered.Count];
            for (int i = 0; i < _ordered.Count; i++) records[i] = _projectiles[_ordered[i]].Snapshot;
            return new ProjectileSnapshot(records, _nextInstanceId);
        }

        public static ProjectileRuntime FromSnapshot(CombatCatalog catalog, IReadOnlyList<ProjectileDefinition> definitions, IProjectileSpawner spawner, IProjectileNavigator navigator, ProjectileSnapshot snapshot, IProjectileDamageRequestFactory factory = null)
        {
            var runtime = new ProjectileRuntime(catalog, definitions, spawner, navigator, factory);
            runtime._nextInstanceId = snapshot == null ? 0 : snapshot.NextInstanceId;
            if (snapshot == null) return runtime;
            for (int i = 0; i < snapshot.Projectiles.Count; i++)
            {
                ProjectileRecordSnapshot record = snapshot.Projectiles[i];
                if (record.Lifecycle != ProjectileLifecycle.Active) continue;
                runtime._projectiles.Add(record.Id, new State(record));
                runtime._hits.Add(record.Id, new HashSet<CombatantId>());
                runtime._ordered.Add(record.Id);
            }
            runtime._ordered.Sort();
            return runtime;
        }

        private ProjectileExpiryEvent Expire(ProjectileInstanceId id, ProjectileExpiryReason reason)
        {
            State state = _projectiles[id];
            _navigator.Stop(state.Snapshot.MovementAgentId);
            _spawner.Despawn(state.Snapshot.SpawnHandle, reason);
            _projectiles.Remove(id);
            _hits.Remove(id);
            _ordered.Remove(id);
            _expired.Add(id);
            return new ProjectileExpiryEvent(id, reason);
        }

        private static ProjectileRecordSnapshot WithRemaining(ProjectileRecordSnapshot s, int remaining) => new ProjectileRecordSnapshot(s.Id, s.DefinitionId, s.AttackSourceId, s.AttackDefinitionId, s.Source, s.SpawnHandle, s.MovementAgentId, remaining, s.ImpactCount, s.Lifecycle);
        private static ProjectileRecordSnapshot WithImpactCount(ProjectileRecordSnapshot s, int count) => new ProjectileRecordSnapshot(s.Id, s.DefinitionId, s.AttackSourceId, s.AttackDefinitionId, s.Source, s.SpawnHandle, s.MovementAgentId, s.RemainingTicks, count, s.Lifecycle);

        private sealed class State { public State(ProjectileRecordSnapshot snapshot) { Snapshot = snapshot; } public ProjectileRecordSnapshot Snapshot; }
    }
}
