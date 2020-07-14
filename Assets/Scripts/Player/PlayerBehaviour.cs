using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : SceneSingleton<PlayerBehaviour>
{
    public GameObject Target;
    public SphereCollider hookable;
    public LineRenderer hookLine;
    public Camera cam;
    public GameObject deathExplosion;
    public float speedIncrement = 0.5f;

    public float xSpread;
    public float ySpread;
    public float zOffset;
    public int launchCoolDown;

    public float rotSpeed;

    string state;
    float timer = 0;
    bool isClockwise;

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
            // Move Player
            cam.transform.position = Vector3.Lerp(cam.transform.position, new Vector3(transform.position.x, transform.position.y + 10, cam.transform.position.z), 0.1f);
            transform.Translate(-Vector3.forward * rotSpeed * 10 * Time.deltaTime, Space.Self);
            transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        }

        if (state == "orbit")
        {
            Rotate();
            RenderHookLineOrbit();
            cam.transform.position = Vector3.Lerp(cam.transform.position, new Vector3(Target.transform.position.x, Target.transform.position.y + 10, cam.transform.position.z), 0.1f);
        }

        OnTap();

        OldPositionSaved = transform.position;

        if (Target != null)
        {
            Vector3 v = Target.transform.InverseTransformPoint(transform.position);
            Vector3 head = v - transform.localPosition;
            var dis = v.magnitude;
            Vector3 dir = head / dis;

            Vector3 distance = Target.transform.position - transform.position;
            Vector3 relativePosition = Vector3.zero;
            relativePosition.x = Vector3.Dot(distance, transform.right.normalized);
            relativePosition.y = Vector3.Dot(distance, transform.up.normalized);
            relativePosition.z = Vector3.Dot(distance, transform.forward.normalized);

            //Debug.Log("local pos: " + relativePosition);
        }
    }

    void Rotate()
    {
        if(isClockwise) transform.RotateAround(Target.transform.position, Vector3.forward, rotSpeed);
        else transform.RotateAround(Target.transform.position, -Vector3.forward, rotSpeed);

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
            SwitchToLaunched();
        }

        else if (Input.GetMouseButtonUp(0) && Target != null && state == "launched")
        {
            SwitchToOrbit();
        }
    }

    public void SetTarget(GameObject target)
    {
        if (state == "launched")
        {
            Target = target;
        }
    }

    void RenderHookLineOrbit()
    {
        hookLine.SetPosition(0, transform.position);
        hookLine.SetPosition(1, Target.transform.position);
    }

    public void SwitchToLaunched()
    {
        Time.timeScale = 1f;
        Target = null;

        hookLine.gameObject.SetActive(false);

        state = "launched";
    }

    public void SwitchToOrbit()
    {
        state = "orbit";
        hookLine.gameObject.SetActive(true);

        launchCoolDown = 10;
        rotSpeed += speedIncrement;

        Vector3 distance = Target.transform.position - transform.position;
        Vector3 relativePosition = Vector3.zero;
        relativePosition.x = Vector3.Dot(distance, transform.right.normalized);
        relativePosition.y = Vector3.Dot(distance, transform.up.normalized);
        relativePosition.z = Vector3.Dot(distance, transform.forward.normalized);

        Debug.Log("CAUGHT local pos: " + relativePosition);
        Debug.Log("distance: " + distance);

        isClockwise = relativePosition.y * distance.x > 0;
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject v = Instantiate(deathExplosion);
        v.transform.position = transform.position;

        state = "dead";
        gameObject.SetActive(false);
    }

    private void OnCollisionExit(Collision collision)
    {
        Target = null;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public bool isLaunched()
    {
        return state == "launched";
    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        //Gizmos.color = Color.yellow;
        //Gizmos.DrawSphere(orbitPoint, 0.4f);
    }
}
