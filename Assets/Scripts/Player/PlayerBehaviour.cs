using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : SceneSingleton<PlayerBehaviour>
{
    public GameObject Target;
    public Color NotHighlightColor;
    public Color HighlightColor;
    public GameObject Ship;
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
    Vector3 relativePosition;

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
            cam.transform.position = Vector3.Lerp(cam.transform.position, new Vector3(transform.position.x, transform.position.y + 10, cam.transform.position.z), 0.02f);
            transform.Translate(-Vector3.forward * rotSpeed * 10 * Time.deltaTime, Space.Self);
            transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        }

        if (state == "orbit")
        {
            Rotate();
            RenderHookLineOrbit();
            cam.transform.position = Vector3.Lerp(cam.transform.position, new Vector3(Target.transform.position.x, Target.transform.position.y + 10, cam.transform.position.z), 0.02f);
        }

        OnTap();

        OldPositionSaved = transform.position;

        if (Target != null)
        {
            //Vector3 velocity = (transform.position - OldPositionSaved) / Time.deltaTime;
            //Vector3 dir = Target.transform.position - transform.position;

            //relativePosition = Vector3.zero;
            //relativePosition.x = Vector3.Dot(dir, transform.right.normalized);
            //relativePosition.y = Vector3.Dot(dir, transform.up.normalized);
            //relativePosition.z = Vector3.Dot(dir, transform.forward.normalized);

            //Vector3 cross = Vector3.zero;
            
            //cross = Vector3.Cross(dir, transform.position);

            //relativePosition = cross;
            //Debug.Log("rel pos (DOT): " + relativePosition);
            //Debug.Log("rel pos (CROSS): " + cross);
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
            Target.GetComponent<Renderer>().material.SetColor("_EmissionColor", HighlightColor);
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
        Target.GetComponent<Renderer>().material.SetColor("_EmissionColor", NotHighlightColor);
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

        Vector3 velocity = (transform.position - OldPositionSaved) / Time.deltaTime;
        Vector3 dir = Target.transform.position - transform.position;

        relativePosition = Vector3.Cross(velocity, dir);

        isClockwise = relativePosition.z >= 0;
    }

    public void CollisionEnter()
    {
        GameObject v = Instantiate(deathExplosion);
        v.transform.position = transform.position;

        state = "dead";
        gameObject.SetActive(false);
    }

    public void CollisionExit()
    {
        Target.GetComponent<Renderer>().material.SetColor("_EmissionColor", NotHighlightColor);
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
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.TransformPoint(relativePosition), 0.4f);
    }
}
