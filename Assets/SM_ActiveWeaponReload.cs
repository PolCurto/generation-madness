using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SM_ActiveWeaponReload : StateMachineBehaviour
{
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<ActiveWeaponController>().IsReloading = false;
    }
}
