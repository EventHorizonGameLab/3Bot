using UnityEngine;
using Sirenix.OdinInspector;

namespace Game.Traps
{
    public class Explosive : BaseExplosive
    {
        [SerializeField, Tooltip("Force required to trigger explosion"), Min(0)] private float _triggerForce = 0.5f;
        [SerializeField, Tooltip("Enable detection area")] private bool _haveDetectionArea = true;

        [ShowIf(nameof(_haveDetectionArea)), Title("Detection Settings")]
        [ShowIf(nameof(_haveDetectionArea)), SerializeField, Tooltip("Radius for detecting objects"), PropertyRange(0, nameof(_explosionRadius))] private float _detectionRadius = 6f;
        [ShowIf(nameof(_haveDetectionArea)), SerializeField, Tooltip("LayerMask for detecting objects")] private LayerMask _detectionLayer;

        private void Update()
        {
            if (_haveDetectionArea) DetectObjects();
        }

        /// <summary>
        /// Detects objects in the detection area and triggers an explosion if conditions are met.
        /// </summary>
        private void DetectObjects()
        {
            if (_debug) Debug.Log($"Detection Radius: {_detectionRadius}");

            if (Physics.OverlapSphere(transform.position, _detectionRadius, _detectionLayer).Length > 0)
            {
                Explosion();
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (_debug) Debug.Log($"Collision Impulse: {collision.impulse.magnitude}");

            if (collision.impulse.magnitude >= _triggerForce)
            {
                Explosion();
            }
        }

        private new void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            if (!_haveDetectionArea) return;

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, _detectionRadius);
        }
    }
}
