using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState1 : StateMachineBehaviour
{
    public const string MOVE_STATE = "Move";

    private void Awake()
    {
        // wut?
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //if(JoystickController.Instance.GetDirection() == Vector3.zero) { }

        //when tap, start couroutine
        if (Input.GetMouseButtonUp(0))
        {
            animator.GetComponent<PlayerIdleMono>().DoCoroutine(animator);
        }
    }

    public class PlayerIdleMono : MonoBehaviour {

        IEnumerator hookSequenceCouroutine;

        public void DoCoroutine(Animator animator)
        {
            hookSequenceCouroutine = SendHook(0.4f, animator);
            StartCoroutine(hookSequenceCouroutine);
        }

        public IEnumerator SendHook(float time, Animator a)
        {
            float counter = 0;
            // send hook
            Hook.Instance.SendHook(time);

            while (counter < time)
            {
                counter += Time.deltaTime;

                // zoom out


                yield return null;
            }

            // start moving

            // switch to move state
            a.SetTrigger(MOVE_STATE);

            counter = 0;
        }
    }
}
