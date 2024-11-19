using System;
using UnityEngine;

[Obsolete("Use ITakeDamage instead")]
public interface IExplosionAffected
{
    void OnExplosion(Vector3 explosionPosition, float explosionForce);
}
