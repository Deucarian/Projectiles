using System;
using Deucarian.Editor;
using UnityEditor;

namespace Deucarian.Projectiles.Editor
{
    [CustomPropertyDrawer(typeof(ProjectileKey), true)]
    public sealed class ProjectileKeyDrawer : DeucarianKeyDrawer
    {
        public override Type KeyType => typeof(ProjectileKey);
        public override Type DefinitionSetAttribute => typeof(ProjectileKeySetAttribute);
        public override string SetupHint => "Select an existing ProjectileKey; declare reusable keys once in a [ProjectileKeySet] class.";
    }
}
