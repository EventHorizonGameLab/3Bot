using UnityEngine;

namespace Game.Traps
{
    [RequireComponent(typeof(Collider))]
    public class Mine : BaseExplosive
    {
        private void OnTriggerEnter(Collider other)
        {
            if (_debug) Debug.Log($"Mine triggered by {other.name}");

            Explosion();
        }
    }
}
