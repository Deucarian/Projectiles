using System;

namespace Deucarian.Projectiles
{
    /// <summary>Marks an authoritative set of named ProjectileKey fields or properties for the Inspector.</summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class ProjectileKeySetAttribute : Attribute { }
}
