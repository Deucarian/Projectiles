using System;
using System.Linq;
using Deucarian.WorldSpawning;
using Deucarian.WorldSpawning.Unity;
using Deucarian.Combat;
using Deucarian.Combat.Unity;
using Deucarian.Editor;
using Deucarian.Editor.Definitions;
using Deucarian.Projectiles.Unity;
using UnityEditor;
using UnityEngine;

namespace Deucarian.Projectiles.Editor.Definitions
{
    [Serializable]
    public sealed class ProjectileDefinitionSpec : DeucarianDefinitionSpec
    {
        [DefinitionField("spawnable")] public SpawnableDefinitionAsset Spawnable = null;
        [DefinitionField("damageType")] public DamageTypeDefinitionAsset DamageType = null;
        [DefinitionField("damage")] public double Damage = 10d;
        [DefinitionField("lifetimeTicks")] public int LifetimeTicks = 120;
        [DefinitionField("speed")] public float Speed = 8f;
        [DefinitionField("maximumImpacts")] public int MaximumImpacts = 1;
    }
}
