using System;
using System.Linq;
using Deucarian.Editor.Definitions;
using Deucarian.Projectiles.Unity;
using UnityEditor;

namespace Deucarian.Projectiles.Editor.Definitions
{
    internal static class ProjectDefinitionCatalogAuthoring
    {
        internal static void Refresh(bool validateOnly = false)
        {
            var definitions = AssetDatabase.FindAssets("t:ProjectileDefinitionAsset", new[] { "Assets" })
                .Select(x => AssetDatabase.LoadAssetAtPath<ProjectileDefinitionAsset>(AssetDatabase.GUIDToAssetPath(x)))
                .Where(x => x != null).OrderBy(x => x.Id, StringComparer.Ordinal).ToArray();
            if (definitions.GroupBy(x => x.Id).Any(x => x.Count() > 1)) throw new InvalidOperationException("Projectile definition IDs must be unique.");
            DeucarianDefinitionCatalog.Update<ProjectileDefinitionCatalog>("Assets/DeucarianDefinitions/Resources/Deucarian/Definitions/ProjectileDefinitionCatalog.asset", "definitions", definitions, validateOnly);
        }
    }
}
