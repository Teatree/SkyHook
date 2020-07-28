using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : StateMachineBehaviour
{
    public float playerSpeed = 200;
    public Vector3 pos;

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //if(JoystickController.Instance.GetDirection() == Vector3.zero) { }
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //dir = JoystickController.Instance.GetDirection();
        //dir.z = -dir.y;

        //pos = JoystickController.Instance.GetPosition();
        ////pos.z = -pos.y;
        //
        //Vector3 lookAtPos = animator.transform.position + dir;
        //lookAtPos.y = animator.transform.position.y;
        //
        //animator.transform.LookAt(lookAtPos);
        //// don't think too much about this positions resetting crap. It works though ¯\_(ツ)_/¯
        ////float dist = Vector3.Distance(Player.Instance.GetPosition(), Camera.main.transform.position);
        //Vector3 leftEdge = Camera.main.ViewportToWorldPoint(new Vector3(-165,1, Camera.main.nearClipPlane));
        //Vector3 rightEdge = Camera.main.ViewportToWorldPoint(new Vector3(165,1, Camera.main.nearClipPlane));
        //
        ////animator.transform.position = animator.transform.position + (dir * playerSpeed * Time.deltaTime);
        ////animator.transform.position = new Vector3(Mathf.Clamp(animator.transform.localPosition.x, leftEdge.x, rightEdge.x), 10, Mathf.Clamp(animator.transform.position.z, -17, 34));
        //animator.transform.position = Camera.main.ViewportToWorldPoint(pos);

        Vector3 leftEdge = Camera.main.ViewportToWorldPoint(new Vector3(-165, 1, Camera.main.nearClipPlane));
        Vector3 rightEdge = Camera.main.ViewportToWorldPoint(new Vector3(165, 1, Camera.main.nearClipPlane));

        Plane plane = new Plane(Vector3.up, 0);

        float distance;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (plane.Raycast(ray, out distance))
        {
            animator.transform.position = ray.GetPoint(distance);
            animator.transform.position = new Vector3(Mathf.Clamp(animator.transform.localPosition.x, leftEdge.x, rightEdge.x), 10, Mathf.Clamp(animator.transform.position.z, 0, 2)); // -17, 34
        }
    }
}
