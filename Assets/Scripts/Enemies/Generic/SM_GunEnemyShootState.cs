using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SM_GunEnemyShootState : StateMachineBehaviour
{
    private HoodSkeleton _enemyController;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _enemyController = animator.GetComponent<HoodSkeleton>();
        _enemyController.IsShooting = true;
    }



    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _enemyController.IsShooting = false;
    }
}
