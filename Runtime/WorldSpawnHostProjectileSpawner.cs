using System;
using Deucarian.WorldSpawning;
using UnityEngine;
namespace Deucarian.Projectiles
{
    /// <summary>Uses the configured host's existing pools at the launch position.</summary>
    public sealed class WorldSpawnHostProjectileSpawner : IProjectileSpawner
    {
        private readonly WorldSpawnHost host;
        public WorldSpawnHostProjectileSpawner(WorldSpawnHost host)
        { this.host = host != null ? host : throw new ArgumentNullException(nameof(host)); }
        public ProjectileSpawnResult Spawn(ProjectileDefinition definition, ProjectileLaunchRequest request)
        {
            if (host == null) throw new InvalidOperationException("The projectile spawn host was destroyed. Stop its projectile scope before releasing the spawn host.");
            var result = host.Spawn(new DefinitionKey(definition.SpawnableId.Value), request.Origin, Quaternion.identity);
            return result.Succeeded ? new ProjectileSpawnResult(true, new ProjectileSpawnHandle(result.InstanceId.Value), result.Instance) : new ProjectileSpawnResult(false, default, null);
        }
        public void Despawn(ProjectileSpawnHandle handle, ProjectileExpiryReason reason)
        { if (host != null && handle.Value > 0) host.Despawn(new SpawnInstanceId(handle.Value)); }
        private sealed class DefinitionKey : SpawnableKey { public DefinitionKey(string id) : base(id) { } }
    }
}
