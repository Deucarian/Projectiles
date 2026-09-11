using System;
using Deucarian.Attacks;
using UnityEngine;

namespace Deucarian.Projectiles
{
    /// <summary>Configured firing origin and attack context. The shared runtime's owner drives ticks and impacts.</summary>
    [DisallowMultipleComponent]
    public sealed class ProjectileEmitter : MonoBehaviour
    {
        [SerializeField] private Transform origin;
        [SerializeField, Min(0.001f)] private float travelDistance = 100f;
        private ProjectileRuntime runtime;
        private Func<AttackSourceSnapshot> captureSource;
        private AttackDefinitionId attackDefinition;
        private bool destroyed;

        public void Configure(ProjectileRuntime projectileRuntime, Func<AttackSourceSnapshot> source,
            AttackDefinitionId attack, float distance = 100f)
        {
            if (destroyed) throw new ObjectDisposedException(nameof(ProjectileEmitter));
            if (runtime != null) throw new InvalidOperationException("The projectile emitter is already configured.");
            if (projectileRuntime == null) throw new ArgumentNullException(nameof(projectileRuntime));
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (attack.IsEmpty) throw new ArgumentException("An attack definition is required.", nameof(attack));
            if (distance <= 0 || float.IsNaN(distance) || float.IsInfinity(distance)) throw new ArgumentOutOfRangeException(nameof(distance));
            runtime = projectileRuntime;
            captureSource = source;
            attackDefinition = attack;
            travelDistance = distance;
        }

        public ProjectileLaunchResult Fire(ProjectileKey projectile, Vector3 direction)
        {
            if (projectile == null) throw new ArgumentNullException(nameof(projectile), "Select a ProjectileKey or pass a named projectile definition.");
            if (destroyed) throw new ObjectDisposedException(nameof(ProjectileEmitter));
            if (runtime == null) throw new InvalidOperationException("Configure the projectile emitter first.");
            float magnitude = direction.sqrMagnitude;
            if (magnitude <= 0 || float.IsNaN(magnitude) || float.IsInfinity(magnitude))
                return new ProjectileLaunchResult(false, ProjectileLaunchFailureReason.InvalidInput, default);
            var source = captureSource();
            if (source.Id.IsEmpty || !source.Enabled)
                return new ProjectileLaunchResult(false, ProjectileLaunchFailureReason.InvalidInput, default);
            Vector3 position = (origin != null ? origin : transform).position;
            return runtime.Launch(new ProjectileLaunchRequest(new ProjectileDefinitionId(projectile.Id), source.Id,
                attackDefinition, source, position, position + direction.normalized * travelDistance));
        }
        private void OnDestroy() { destroyed = true; runtime = null; captureSource = null; }
    }
}
