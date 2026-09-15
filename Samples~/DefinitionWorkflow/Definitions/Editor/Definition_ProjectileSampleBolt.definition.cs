// <deucarian-definition schema="projectiles" />
// Editable declaration. Use the package definition editor or edit the values below.
namespace Deucarian.ProjectDefinitions.Definition_projectiles
{
    public static class Definition_ProjectileSampleBolt
    {
        public static global::Deucarian.Projectiles.Editor.Definitions.ProjectileDefinitionSpec Value =>
        // definition-value
        new global::Deucarian.Projectiles.Editor.Definitions.ProjectileDefinitionSpec
        {
            Damage = 10d,
            DamageType = global::Deucarian.Editor.Definitions.DeucarianDefinitionAssets.Load<global::Deucarian.Combat.Unity.DamageTypeDefinitionAsset>("61e623c512fae3e41be5f5c490a8eaf5", 11400000L),
            Id = "85c12afc45bb4c0c8c3c03e918f4ed5e",
            LifetimeTicks = 150,
            MaximumImpacts = 1,
            Name = "ProjectileSampleBolt",
            Spawnable = global::Deucarian.Editor.Definitions.DeucarianDefinitionAssets.Load<global::Deucarian.WorldSpawning.Unity.SpawnableDefinitionAsset>("65f39e55d7e6f0d42aecb332d7ea2cea", 11400000L),
            Speed = 3f,
        };
        // end-definition-value
    }
}
