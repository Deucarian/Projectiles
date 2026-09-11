using UnityEngine;

namespace Deucarian.Projectiles.Samples.SimpleUsage
{
    public sealed class SimpleUsageExample : MonoBehaviour
    {
        [SerializeField] private ProjectileEmitter emitter;
        public void Fire(Vector3 direction) => emitter.Fire("arrow", direction);
    }
}
