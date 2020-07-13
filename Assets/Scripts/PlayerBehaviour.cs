using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    public GameObject Target;
    public SphereCollider hookable;
    public Camera cam;

    public float xSpread;
    public float ySpread;
    public float zOffset;
    public int launchCoolDown;

    public float rotSpeed;

    string state;
    float timer = 0;

    Vector3 orbitPoint;
    Vector3 OldPositionSaved;

    void Start()
    {
        state = "orbit";
        cam.transform.position = new Vector3(Target.transform.position.x, Target.transform.position.y + 10, cam.transform.position.z);
    }

    void Update()
    {
        Debug.Log("state: " + state);

        if (state == "launched")
        {
            cam.transform.position = Vector3.Lerp(cam.transform.position, new Vector3(transform.position.x, transform.position.y + 10, cam.transform.position.z), 0.1f);
            transform.Translate(-Vector3.forward * rotSpeed * 10 * Time.deltaTime, Space.Self);
        }

        if (state == "orbit")
        {
            Rotate();
            cam.transform.position = Vector3.Lerp(cam.transform.position, new Vector3(Target.transform.position.x, Target.transform.position.y + 10, cam.transform.position.z), 0.1f);
        }

        OnTap();

        OldPositionSaved = transform.position;
    }

    void Rotate()
    {
        //timer += Time.deltaTime * rotSpeed;

        //float x = -Mathf.Cos(timer) * xSpread;
        //float y = Mathf.Sin(timer) * ySpread;
        //Vector3 pos = new Vector3(x, y, zOffset);

        //Vector3 newPos = pos + Target.transform.position;

        ////float dist = Vector3.Distance(newPos, Target.transform.position);
        //Vector3 lookDir = Target.transform.position - transform.position;
        //Vector3 finalPoint = lookDir + lookDir.normalized * xSpread;
        //orbitPoint = transform.position + finalPoint;

        //transform.position = Vector3.Slerp(transform.position, newPos, 0.02f);


        transform.RotateAround(Target.transform.position, Vector3.forward, rotSpeed);

        transform.LookAt(Target.transform);
    }

    void OnTap()
    {
        launchCoolDown--;

        if (Input.GetMouseButton(0) && state == "orbit" && launchCoolDown <= 0)
        {
            Time.timeScale = 0.25f;
        }

        else if (Input.GetMouseButtonUp(0) && state == "orbit" && launchCoolDown <= 0)
        {
            Time.timeScale = 1f;
            Target = null;

            state = "launched";
        }

        else if (Input.GetMouseButtonUp(0) && Target != null && state == "launched")
        {
            state = "orbit";
            launchCoolDown = 250;
        }
    }

    public void SetTarget(GameObject target)
    {
        if (state == "launched")
        {
            Target = target;
        }
    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        //Gizmos.color = Color.yellow;
        //Gizmos.DrawSphere(orbitPoint, 0.4f);
    }
}
