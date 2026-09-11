# Simple usage

Configure ProjectileEmitter once with the application's ProjectileRuntime, an explicit Func<AttackSourceSnapshot> that captures current owner/attack state, an AttackDefinitionId, and travel distance. Set an optional muzzle Transform in the Inspector; otherwise the emitter's transform is the origin. Fire normalizes direction and returns ProjectileLaunchResult. Invalid directions or disabled sources are rejected. The shared runtime's existing owner still drives ticks, navigation, impact reports, and cleanup once for the whole runtime. This emitter borrows that runtime and does not add targeting, cooldowns, or damage resolution.

Import the **Simple Usage** sample from Unity Package Manager. Its caller script is:

Definition fields now use named, domain-specific keys. Select an existing definition from the Inspector dropdown or pass the same named key in code. Declare each project key once in a marked key set; ordinary caller methods do not accept raw IDs. Generated keys for asset-authored definitions require no asset reference in the caller. Owner-issued selection and row handles represent runtime instances.

```csharp
using UnityEngine;

namespace Deucarian.Projectiles.Samples.SimpleUsage
{
    public sealed class SimpleUsageExample : MonoBehaviour
    {
        [SerializeField] private ProjectileKey arrow = ProjectileKeys.Arrow;

        [SerializeField] private ProjectileEmitter emitter;
        public void Fire(Vector3 direction) => emitter.Fire(arrow, direction);
    }
}
```
