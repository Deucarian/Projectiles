using System;
using System.Linq;
using UnityEngine;

namespace Deucarian.Projectiles.Unity
{
    public sealed class ProjectileDefinitionCatalog : ScriptableObject
    {
        public const string ResourcePath = "Deucarian/Definitions/ProjectileDefinitionCatalog";
        [SerializeField] private ProjectileDefinitionAsset[] definitions = Array.Empty<ProjectileDefinitionAsset>();
        public static ProjectileDefinitionCatalog LoadProject() => Resources.Load<ProjectileDefinitionCatalog>(ResourcePath) ?? throw new InvalidOperationException("Create a Projectile definition in Definitions before loading its project catalog.");
        public ProjectileDefinition[] CreateRuntimeDefinitions()
        {
            if (definitions.Any(x => x == null) || definitions.GroupBy(x => x.Id).Any(x => x.Count() > 1)) throw new InvalidOperationException("The Projectile catalog contains missing or duplicate definitions. Synchronize it in Definitions.");
            return definitions.Select(x => x.ToRuntimeDefinition()).ToArray();
        }
    }
}
