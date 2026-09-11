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
    public sealed class ProjectileKeySource : DeucarianAssetKeySource<ProjectileDefinitionAsset>
    {
        public override Type KeyType => typeof(ProjectileKey);
        public override Type DefinitionSetAttribute => typeof(ProjectileKeySetAttribute);
        public override string GeneratedClassName => "ProjectProjectiles";
        protected override DeucarianKeyChoice ReadDefinition(ProjectileDefinitionAsset asset) => new DeucarianKeyChoice(asset.Id, asset.DisplayName);
    }
}
