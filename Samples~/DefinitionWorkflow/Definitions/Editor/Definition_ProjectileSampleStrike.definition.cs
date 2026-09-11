// <deucarian-definition schema="attacks" />
// Editable declaration. Use the package definition editor or edit the values below.
namespace Deucarian.ProjectDefinitions.Definition_attacks
{
    public static class Definition_ProjectileSampleStrike
    {
        public static global::Deucarian.Attacks.Editor.Definitions.AttackDefinitionSpec Value =>
        // definition-value
        new global::Deucarian.Attacks.Editor.Definitions.AttackDefinitionSpec
        {
            BalancingNotes = "",
            Delivery = new global::Deucarian.Attacks.Editor.Definitions.AttackDeliveryDefinitionSpec
            {
                BeamVfxPrefab = null,
                Homing = false,
                HomingTurnRate = 180f,
                ImpactVfxPrefab = null,
                MaxHits = 1,
                Mode = global::Deucarian.Attacks.Authoring.AttackRecipeDeliveryMode.Projectile,
                PierceCount = 0,
                ProjectileDefinitionId = "projectile.example.basic",
                ProjectileLifetimeTicks = 120,
                ProjectilePrefab = null,
                ProjectileSpawnableId = "projectile.example.basic",
                ProjectileSpeed = 8f,
                Radius = 1.5f,
                TickIntervalSeconds = 0.5f,
            },
            Icon = null,
            Id = "40d5286e2ade48268e8ef21ce55831e3",
            Mechanics = new global::Deucarian.Attacks.Editor.Definitions.AttackMechanicsDefinitionSpec
            {
                CooldownTicks = 20,
                DamageAmount = 10f,
                DamageTypeId = "52d28d9a46fc400d901d9fd98ee4f4e1",
                Range = 6f,
            },
            Name = "ProjectileSampleStrike",
            Presentation = new global::Deucarian.Attacks.Editor.Definitions.AttackPresentationDefinitionSpec
            {
                Events = new global::Deucarian.Attacks.Editor.Definitions.AttackPresentationEventSpec[]
                {
                },
            },
            StatusEffects = new global::Deucarian.Attacks.Editor.Definitions.AttackStatusEffectsDefinitionSpec
            {
                StatusEffects = new global::Deucarian.Attacks.Editor.Definitions.AttackStatusEffectSpec[]
                {
                },
            },
            Tags = new global::System.String[]
            {
            },
            Targeting = new global::Deucarian.Attacks.Editor.Definitions.AttackTargetingDefinitionSpec
            {
                MaxTargets = 1,
                Mode = global::Deucarian.Attacks.Authoring.AttackRecipeTargetingMode.Nearest,
                RequiresLineOfSight = false,
            },
            UpgradeHookId = "",
        };
        // end-definition-value
    }
}
