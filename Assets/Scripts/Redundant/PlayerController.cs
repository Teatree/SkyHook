using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : SceneSingleton<PlayerController>
{
    public enum PlayerState { idle, hooking, moving, arriving, dead }
    public GameObject windParticleEffect;
    public GameObject tapToGo; // temp

    PlayerState state;
    Vector3 playerTouchPosition;
    Vector3 startDragSaved;
    Vector3 startDragPlayerPosSaved;
    IEnumerator hookSequenceCouroutine;
    IEnumerator initialPullCouroutine;

    void Start()
    {
        SetState(PlayerState.idle);
    }

    // Update is called once per frame
    void Update()
    {
        if (state == PlayerState.idle && Input.GetMouseButtonUp(0))
        {
            SetState(PlayerState.hooking);
            hookSequenceCouroutine = SendHook(1.4f);
            StartCoroutine(hookSequenceCouroutine);
        }
        else if (state == PlayerState.moving && Input.GetMouseButtonDown(0))
        {
            SavePosition();
        }
        else if (state == PlayerState.moving && Input.GetMouseButton(0))
        {
            Vector3 leftEdge = Camera.main.ViewportToWorldPoint(new Vector3(-165, 1, Camera.main.nearClipPlane));
            Vector3 rightEdge = Camera.main.ViewportToWorldPoint(new Vector3(165, 1, Camera.main.nearClipPlane));

            Plane plane = new Plane(Vector3.up, 0);

            float distance;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (plane.Raycast(ray, out distance))
            {
                Vector3 t = ray.GetPoint(distance) - startDragSaved;
                //Debug.Log("something distance from start mouse: " + t);

                transform.position = startDragPlayerPosSaved + t;
                transform.position = Vector3.Lerp(transform.position, new Vector3(Mathf.Clamp(transform.localPosition.x, leftEdge.x, rightEdge.x), 10, Mathf.Clamp(transform.position.z, 5, 7)), 0.98f); // -17, 34

                //Vector3 centerPoint = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, Camera.main.nearClipPlane));
                //
                //Vector3 direction = transform.position - centerPoint; //direction from Center to Cursor
                //Vector3 normalizedDirection = direction.normalized;
                //
                //transform.position = centerPoint + (normalizedDirection * 10f);

               // transform.LookAt(Hook.Instance.targetPosSaved);
                transform.eulerAngles = new Vector3(-130, transform.eulerAngles.y, transform.eulerAngles.z);

                //transform.position = new Vector3(transform.position.x, 10, transform.position.z);
            }

            Hook.Instance.SetHookPos(transform.position);
        }
    }

    public IEnumerator SendHook(float time)
    {
        float counter = 0;
        // send hook
        Hook.Instance.SendHook(time);

        while (counter < time)
        {
            counter += Time.deltaTime;

            // zoom out
            Camera.main.fieldOfView = Mathf.Lerp(17, 27, counter / time);
            BackgroundCameraMovement.Instance.SetFieldOfVIew(Mathf.Lerp(45, 60, counter / time));

            yield return null;
        }

        // start moving


        // perform initial pull
        windParticleEffect.SetActive(true);
        initialPullCouroutine = InitialPull(0.4f);
        StartCoroutine(initialPullCouroutine);

        counter = 0;
    }

    public IEnumerator InitialPull(float time)
    {
        float counter = 0;
        // send hook
        Hook.Instance.SendHook(time);
        BackgroundCameraMovement.Instance.PerformInitialPull(time);

        while (counter < time)
        {
            counter += Time.deltaTime;

            // move player
            Vector3 leftEdge = Camera.main.ViewportToWorldPoint(new Vector3(-165, 1, Camera.main.nearClipPlane));
            Vector3 rightEdge = Camera.main.ViewportToWorldPoint(new Vector3(165, 1, Camera.main.nearClipPlane));

            transform.position = Vector3.Lerp(transform.position, new Vector3(Mathf.Clamp(transform.localPosition.x, leftEdge.x, rightEdge.x), 10, Mathf.Clamp(transform.position.z, 3, 5)), counter / time); // -17, 34

            transform.eulerAngles = new Vector3(Mathf.Lerp(-90, -130, counter / time), transform.eulerAngles.y, transform.eulerAngles.z);

            Hook.Instance.SetHookPos(transform.position);
 
            yield return null;
        }

        // switch to move state
        SetState(PlayerState.moving);

        counter = 0;
    }

    public void SavePosition()
    {
        Plane plane = new Plane(Vector3.up, 0);

        float distance;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (plane.Raycast(ray, out distance))
        {
            startDragSaved = ray.GetPoint(distance);
            startDragPlayerPosSaved = transform.position;
        }
    }

    public void SetState(PlayerState s)
    {
        state = s;
    }

    public PlayerState GetState()
    {
        return state;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }
}
