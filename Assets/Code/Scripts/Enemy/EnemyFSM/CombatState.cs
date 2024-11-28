using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.RuleTile.TilingRuleOutput;

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
        if (controller.playerTransform != null) { RotateTowards(controller.playerTransform.position); }
        Collider[] colliders = Physics.OverlapSphere(controller.transform.position, controller.losRadius, controller.playerLayer);
        if (colliders.Length < 1)
        {
            controller.playerTransform = null;
            controller.ChangeState(controller.nonCombatState);
            return;
        }
        if (Vector3.Distance(controller.playerTransform.position, controller.transform.position) >= controller.stopDistanceToPlayer)
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
