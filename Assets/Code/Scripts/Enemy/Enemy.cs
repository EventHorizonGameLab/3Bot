using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    GunSettings gun;
    [SerializeField] float losRadius;
    [SerializeField] Transform muzzle;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] float rotationSpeed;

    private void Awake()
    {
        gun = GetComponentInChildren<GunSettings>();
    }

    private void FixedUpdate()
    {
        if (PlayerIsInVision()) gun.Shoot();
    }

    bool PlayerIsInVision()
    {
        Collider[] colInRange = (Physics.OverlapSphere(transform.position, losRadius, playerLayer));
        if (colInRange.Length <= 0) return false;
        Collider player = colInRange[0];
        Vector3 playerDirection = (player.transform.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, playerDirection, out RaycastHit hit, distance))
        {
            if (hit.collider != player) return false;
        }

        RotateTowards(player.transform.position);
        return true;
    }

    bool RotateTowards(Vector3 targetPos)
    {
        Vector3 dir = targetPos - transform.position;
        dir.y = 0;
        Quaternion targetRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed);
        if (transform.rotation == targetRot) return true; else return false;
    }



    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, losRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * 100);
    }
}
