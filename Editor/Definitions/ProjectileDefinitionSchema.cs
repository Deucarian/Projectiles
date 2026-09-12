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
    public sealed class ProjectileDefinitionSchema : DeucarianSerializedDefinitionSchema<ProjectileDefinitionAsset, ProjectileDefinitionSpec>
    {
        public override string Id => "projectiles";
        public override string DisplayName => "Projectiles";
        public override void ValidateAssetReady(ScriptableObject asset)
        {
            base.ValidateAssetReady(asset);
            ((ProjectileDefinitionAsset)asset).ToRuntimeDefinition();
        }
        public override void RefreshCatalog(bool validateOnly = false) { ProjectDefinitionCatalogAuthoring.Refresh(validateOnly); }
        [MenuItem("Assets/Create/Deucarian/Projectiles/Projectile Definition")]
        private static void CreateDefinition() { Selection.activeObject = DeucarianDefinitionSync.Create(new ProjectileDefinitionSchema(), "NewProjectile"); }
    }
}
