using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HookState : State {

    public HookState(GameSystem gameSystem) : base(gameSystem)
    {

    }

    public override IEnumerator Start()
    {
        GameSystem.WindParticlesGo.SetActive(true);

        float counter = 0;
        float timeToHook = GameSystem.TIME_TO_HOOK;
        float timeToPull = GameSystem.TIME_TO_PULL;
        Transform transform = GameSystem.Player.transform;
        // send hook
        Hook.Instance.SendHook(timeToHook);

        while (counter < timeToHook)
        {
            counter += Time.deltaTime;

            yield return null;
        }

        counter = 0;

        // send hook
        Hook.Instance.SendHook(timeToPull);
        BackgroundCameraMovement.Instance.PerformInitialPull(timeToPull);

        while (counter < timeToPull)
        {
            counter += Time.deltaTime;

            // zoom out
            Camera.main.fieldOfView = Mathf.Lerp(17, 27, counter / timeToPull);
            BackgroundCameraMovement.Instance.SetFieldOfVIew(Mathf.Lerp(45, 60, counter / timeToPull));

            // move player
            Vector3 leftEdge = Camera.main.ViewportToWorldPoint(new Vector3(-165, 1, Camera.main.nearClipPlane));
            Vector3 rightEdge = Camera.main.ViewportToWorldPoint(new Vector3(165, 1, Camera.main.nearClipPlane));

            transform.position = Vector3.Lerp(transform.position, new Vector3(Mathf.Clamp(transform.localPosition.x, leftEdge.x, rightEdge.x), 10, Mathf.Clamp(transform.position.z, 3, 5)), counter / timeToPull); // -17, 34

            transform.eulerAngles = new Vector3(Mathf.Lerp(-90, -130, counter / timeToPull), transform.eulerAngles.y, transform.eulerAngles.z);

            Hook.Instance.SetHookPos(transform.position);

            yield return null;
        }

        GameSystem.SetState(new MoveState(GameSystem));

        counter = 0;
    }
}

