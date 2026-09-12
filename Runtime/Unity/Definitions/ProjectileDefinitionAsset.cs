using System;
using Deucarian.WorldSpawning;
using Deucarian.WorldSpawning.Unity;
using Deucarian.Combat;
using Deucarian.Combat.Unity;

using UnityEngine;

namespace Deucarian.Projectiles.Unity
{
    /// <summary>Reusable defaults for code calls and serialized typed keys; runtime state remains in the core service.</summary>
    public sealed class ProjectileDefinitionAsset : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [SerializeField] private SpawnableDefinitionAsset spawnable = null;
        [SerializeField] private DamageTypeDefinitionAsset damageType = null;
        [SerializeField] private double damage = 10d;
        [SerializeField] private int lifetimeTicks = 120;
        [SerializeField] private float speed = 8f;
        [SerializeField] private int maximumImpacts = 1;
        public string Id => id;
        public string DisplayName => displayName;
        public ProjectileKey Key => new AssetKey(id);
        public ProjectileDefinition ToRuntimeDefinition()
        {
            if (spawnable == null || damageType == null) throw new InvalidOperationException("Choose an existing spawnable and damage type for projectile '" + DisplayName + "' in Definitions.");
            return new ProjectileDefinition(new ProjectileDefinitionId(Id), new WorldSpawnableId(spawnable.Id), new DamageTypeId(damageType.Id), damage, lifetimeTicks, speed, maximumImpacts);
        }
        private sealed class AssetKey : ProjectileKey { public AssetKey(string value) : base(value) { } }
    }
}
