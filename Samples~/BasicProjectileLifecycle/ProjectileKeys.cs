namespace Deucarian.Projectiles.Samples.SimpleUsage
{
    [ProjectileKeySet]
    public static class ProjectileKeys
    {
        public static ProjectileKey Arrow => new Definition();
        private sealed class Definition : ProjectileKey
        {
            public Definition() : base("arrow") { }
        }
    }
}
