using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishedState : State {

    float timeToFinish = 1.5f;

    public FinishedState(GameSystem system) : base(system)
    {

    }

    Transform transform;

    public override IEnumerator Start()
    {
        transform = GameSystem.Player.transform;
        float counter = 0;

        //terrible
        LevelSystem.Instance.ResetMoveCounter();
        LevelSystem.Instance.MoveSpinnerToPlayer(transform.position, timeToFinish);
        Hook.Instance.ClearPositions();

        while (counter < timeToFinish)
        {
            counter += Time.deltaTime;

            // zoom in
            Camera.main.fieldOfView = Mathf.Lerp(27, 17, counter / timeToFinish);
            BackgroundCameraMovement.Instance.SetFieldOfVIew(Mathf.Lerp(60, 45, counter / timeToFinish));

            transform.position = Vector3.Lerp(transform.position, GameSystem.InitialPlayerPos, counter / timeToFinish); // -17, 34
            //transform.eulerAngles = new Vector3(Mathf.Lerp(transform.eulerAngles.x, -90, counter / timeToFinish), transform.eulerAngles.y, transform.eulerAngles.z);

            yield return null;
        }

        GameSystem.SetState(new IdleState(GameSystem));

        counter = 0;
    }
}
