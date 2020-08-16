using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishedState : State {

    float timeToFinish = 1.5f;
    float counter;

    public FinishedState(GameSystem system) : base(system)
    {

    }

    public override IEnumerator Start()
    {
        // Make it so everyone stops chasing

        // zoom out

        // lift the balloon up into the air

        // display win screen

        while (counter < timeToFinish)
        {
            counter += Time.deltaTime;

            // zoom in
            Camera.main.fieldOfView = Mathf.Lerp(17, 27, counter / timeToFinish);
            //BackgroundCameraMovement.Instance.SetFieldOfVIew(Mathf.Lerp(60, 45, counter / timeToFinish));

            yield return null;
        }

        //GameSystem.SetState(new IdleState(GameSystem));

        counter = 0;
    }

    public override void OnUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Scene scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name);
        }
    }
}
