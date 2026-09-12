using System;
using UnityEngine;

namespace Deucarian.Projectiles
{
    [AddComponentMenu("Deucarian/Projectiles/Projectile Trigger")]
    public sealed class ProjectileTrigger : MonoBehaviour
    {
        [SerializeField] private ProjectileEmitter emitter;
        [SerializeField] private ProjectileKey projectile;
        [SerializeField] private Transform direction;
        public ProjectileLaunchResult FireProjectile()
        {
            if (emitter == null) throw new InvalidOperationException("Assign a configured ProjectileEmitter to ProjectileTrigger '" + name + "'.");
            return emitter.Fire(projectile, (direction != null ? direction : transform).forward);
        }
        public void Fire()
        {
            var result = FireProjectile();
            if (!result.Succeeded) throw new InvalidOperationException("ProjectileTrigger '" + name + "' could not fire: " + result.FailureReason + ". Check its definition, registered source and emitter configuration.");
        }
    }
}
