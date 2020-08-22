using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundCameraMovement : SceneSingleton<BackgroundCameraMovement>
{
    public enum CameraState { idle, moving, initialPull }
    CameraState camState;
    IEnumerator initialPullCouroutine;
    float currentCamSpeed;
    float idleCamSpeed;

    void Start()
    {
        idleCamSpeed = 0.9f;
        currentCamSpeed = idleCamSpeed;

        SetState(CameraState.idle);
    }

    // Update is called once per frame
    void Update()
    {
        if(camState == CameraState.idle)
        {
            transform.Translate(Vector3.forward * currentCamSpeed * Time.deltaTime, Space.World);
            transform.position = new Vector3(transform.position.x, 80, transform.position.z);
        }
        else if (camState == CameraState.initialPull)
        {
            
        }else if (camState == CameraState.moving)
        {
            transform.Translate(Vector3.forward * currentCamSpeed * Time.deltaTime, Space.World);
            transform.position = new Vector3(transform.position.x, 80, transform.position.z);
        }
    }

    public void PerformInitialPull(float time)
    {
        currentCamSpeed = 30f; // this will need to become actual speed
        SetState(CameraState.initialPull);

        initialPullCouroutine = InitialPull(time);
        StartCoroutine(initialPullCouroutine);
    }

    IEnumerator InitialPull(float time)
    {
        float counter = 0;

        while (counter < time)
        {
            counter += Time.deltaTime;

            transform.Translate(Vector3.forward * currentCamSpeed * Time.deltaTime, Space.World);

            yield return null;
        }

        // switch to move state
        SetState(CameraState.moving);
        currentCamSpeed = 20f;

        counter = 0;
    }

    public void SetState(CameraState c)
    {
        camState = c;
    }

    public void SetFieldOfVIew(float v)
    {
        GetComponent<Camera>().fieldOfView = v;
    }
}
