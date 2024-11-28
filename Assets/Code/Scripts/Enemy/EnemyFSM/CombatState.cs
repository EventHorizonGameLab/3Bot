using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class CombatState : IEenemyState
{
    EnemyController controller;
    NavMeshAgent agent;
    GunSettings gun;
    public CombatState(EnemyController enemyController)
    {
        controller = enemyController;
        agent = enemyController.GetComponent<NavMeshAgent>();
        gun = controller.GetComponentInChildren<GunSettings>();
    }

    public void Enter()
    {

    }

    public void Exit()
    {
        controller.playerTransform = null;
    }

    public void Process()
    {
        if (!controller.PlayerIsInRange() || !controller.PlayerIsInVision())
        {
            controller.ChangeState(controller.nonCombatState);
            return;
        }

        if (controller.playerTransform != null) { RotateTowards(controller.playerTransform.position); }

        if (Vector3.Distance(controller.playerTransform.position + Vector3.up , controller.transform.position + Vector3.up ) > controller.stopDistanceToPlayer)
        {
            agent.speed = controller.chasingSpeed;
            agent.isStopped = false;
            agent.SetDestination(controller.playerTransform.position);
        }
        gun.Shoot();
    }

    void RotateTowards(Vector3 targetPos)
    {
        Vector3 dir = targetPos - controller.transform.position;
        dir.y = 0;
        Quaternion targetRot = Quaternion.LookRotation(dir);
        controller.transform.rotation = Quaternion.Slerp(controller.transform.rotation, targetRot, controller.rotationSpeed);
    }
}
