using UnityEngine;

public interface IExplosionAffected
{
    void OnExplosion(Vector3 explosionPosition, float explosionForce);
}
