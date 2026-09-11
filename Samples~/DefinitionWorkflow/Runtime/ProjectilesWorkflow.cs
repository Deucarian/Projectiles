using System;
using UnityEngine;

namespace Deucarian.Projectiles.Samples.DefinitionWorkflow
{
    /// <summary>Small caller example. The configured scene hosts own services and resource lifetimes.</summary>
    public sealed class ProjectilesWorkflow : MonoBehaviour
    {
        [SerializeField] private ProjectileEmitter emitter;
        [SerializeField] private ProjectileKey projectile;
        [SerializeField] private ProjectileTrigger trigger;
        [SerializeField] private ProjectileHost host;
        private string status = "Ready. Choose an action below.";
        public string Status => status;
        public void Fire() { var result = emitter.Fire(projectile, Vector3.right); status = "Launch: " + result.Succeeded + ". Active: " + host.Runtime.ActiveCount; }
        public void FireComponent() { trigger.Fire(); status = "Active projectiles: " + host.Runtime.ActiveCount; }
        public void Clear() { host.Clear(); status = "Projectiles cleared."; }
        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(24, 24, Math.Min(540, Screen.width - 48), Screen.height - 48), GUI.skin.box);
            GUILayout.Label("Projectiles — definition workflow");
            GUILayout.Label("The projectile definition reuses typed spawnable and damage definitions. The host owns projectile state; spawning and navigation keep their existing owners.");
            GUILayout.Space(12);
            if (GUILayout.Button("Fire with C#", GUILayout.Height(32))) { try { Fire(); } catch (Exception error) { status = error.Message; } }
            if (GUILayout.Button("Fire with component", GUILayout.Height(32))) { try { FireComponent(); } catch (Exception error) { status = error.Message; } }
            if (GUILayout.Button("Clear projectiles", GUILayout.Height(32))) { try { Clear(); } catch (Exception error) { status = error.Message; } }
            GUILayout.Space(12);
            GUILayout.Label(status);
            GUILayout.EndArea();
        }
    }
}
