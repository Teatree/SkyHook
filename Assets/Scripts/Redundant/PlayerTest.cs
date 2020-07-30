using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerTest : SceneSingleton<PlayerTest>
{
    public enum PlayerState { orbit, launched, dead, sendingHook }
    public PlayerState state;
    public GameObject hookable; 
    public LineRenderer aimLine;
    public LineRenderer hookLine;
    public Camera MainCamera;
    public float currentSpeed;

    public GameObject joyStickGo;
    public GameObject tapGo;

    Vector3 aimPosition;
    Vector3 oldPositionSaved;
    public Vector3 targetPositon;

    IEnumerator sendHookCouroutine;
    IEnumerator playerMoveCouroutine;

    // Start is called before the first frame update
    void Start()
    {
        currentSpeed = 0.1f;
        PlayerTest.Instance.state = PlayerState.orbit;
        //joyStickGo.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //if(state == PlayerState.launched)
        //{
        //    MovePlayer(targetPositon);
        //}

        //if (Input.GetMouseButtonDown(0))
        //{
        //    tapGo.SetActive(false);
        //    joyStickGo.SetActive(true);
        //}

        ////Joystick Movement
        //if(CrossPlatformInputManager.GetAxis("Horizontal") > 0)
        //{
        //    if (CrossPlatformInputManager.GetAxis("Vertical") > 0)
        //    {
        //        Vector3 moveVec = new Vector3(1, 0, 1) * currentSpeed;
        //        transform.Translate(moveVec);
        //    }
        //    else if (CrossPlatformInputManager.GetAxis("Vertical") < 0)
        //    {
        //        Vector3 moveVec = new Vector3(1, 0, -1) * currentSpeed;
        //        transform.Translate(moveVec);
        //    }
        //}
        //else if (CrossPlatformInputManager.GetAxis("Horizontal") < 0)
        //{
        //    if (CrossPlatformInputManager.GetAxis("Vertical") > 0)
        //    {
        //        Vector3 moveVec = new Vector3(-1, 0, 1) * currentSpeed;
        //        transform.Translate(moveVec);
        //    }
        //    else if (CrossPlatformInputManager.GetAxis("Vertical") < 0)
        //    {
        //        Vector3 moveVec = new Vector3(-1, 0, -1) * currentSpeed;
        //        transform.Translate(moveVec);
        //    }
        //}

        //transform.position = new Vector3(Mathf.Clamp(transform.position.x, -10, 10), transform.position.y, transform.position.z);
        //transform.position = new Vector3(transform.position.x, transform.position.y, Mathf.Clamp(transform.position.z, -8, 32));

        //OnTap();
    }

    void OnTap()
    {
        if (Input.GetMouseButton(0) && state == PlayerState.orbit)
        {
            aimLine.gameObject.SetActive(true);
            Plane plane = new Plane(Vector3.up, 0);

            float distance;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (plane.Raycast(ray, out distance))
            {
                aimPosition = ray.GetPoint(distance);
                aimPosition.y = 10;

                aimLine.SetPosition(0, transform.position);

                Vector3 dir = aimPosition - transform.position;

                aimLine.SetPosition(1, dir * 100);
                var dis = Vector3.Distance(transform.position, dir * 100);
                aimLine.materials[0].mainTextureScale = new Vector3(dis, 1, 1);

                transform.LookAt(aimPosition);
            }
        }
        else if (Input.GetMouseButtonUp(0) && state == PlayerState.orbit)
        {
            aimLine.gameObject.SetActive(false);
            hookLine.gameObject.SetActive(true);
            targetPositon = transform.TransformPoint(Vector3.forward * 240);
            
            // start moving there
            sendHookCouroutine = SendHook(targetPositon, 1f);
            StartCoroutine(sendHookCouroutine);

        }
    }

    public float GetDistance()
    {
        return Vector3.Distance(transform.position, targetPositon);
    }

    IEnumerator SendHook(Vector3 pos, float sec)
    {
        state = PlayerState.sendingHook;
        float s = 0;

        while (s < sec)
        {
            s += Time.deltaTime / sec;

            hookLine.SetPosition(0, transform.position);

            //somewhere here we will also need to figure out interseption course for moving hookables
            Vector3 newTargetPos = Vector3.Lerp(transform.position, pos, s);

            hookLine.SetPosition(1, newTargetPos);
            hookable.SetActive(true);
            hookable.transform.position = newTargetPos;
            hookable.transform.position = new Vector3(hookable.transform.position.x, 10, hookable.transform.position.z);

            yield return null;
        }
        s = 0;

        Debug.Log("LaunchingPlayer");
        state = PlayerState.launched;
        oldPositionSaved = transform.position;
    }

    public void MovePlayer(Vector3 pos)
    {
        hookLine.SetPosition(0, transform.position);
        hookLine.SetPosition(1, targetPositon);
        hookable.transform.position = hookLine.GetPosition(1);

        //MainCamera.transform.position = Vector3.Lerp(MainCamera.transform.position, new Vector3(transform.position.x, MainCamera.transform.position.y, transform.position.z + 10), 0.06f);
        MainCamera.transform.Translate(-GetDirectionTowardsPointOldSaved() * currentSpeed / 50 * Time.deltaTime, Space.World);
        MainCamera.transform.position = new Vector3(MainCamera.transform.position.x, 127.821f, MainCamera.transform.position.z);
        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime, Space.Self);

        if(Vector3.Distance(transform.position, targetPositon) < 15)
        {
            state = PlayerState.orbit;
        }

        // finger stering
        Plane plane = new Plane(Vector3.up, 0);

        float distance;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (plane.Raycast(ray, out distance))
        {
            Vector3 v = ray.GetPoint(distance);
            v.y = 10;
            transform.position = Vector3.Lerp(transform.position, v, 0.1f);
        }
        
    }

    public Vector3 GetDirectionTowardsPoint()
    {
        return transform.position - targetPositon;
    }

    public Vector3 GetDirectionTowardsPointOldSaved()
    {
        return oldPositionSaved- targetPositon;
    }
}
