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
    public bool IsHooked;
    public int launchCoolDown;

    public float rotSpeed;

    string state;
    float timer = 0;
    float hookTravelCounter = 0;
    bool isClockwise;

    Vector3 orbitPoint;
    Vector3 OldPositionSaved;
    Vector3 relativePosition;
    IEnumerator hookSendCouroutine;

    void Start()
    {
        state = "orbit";
        cam.transform.position = new Vector3(Target.transform.position.x, cam.transform.position.y, Target.transform.position.z+10);

        
    }
     
    void Update()
    {
        //Debug.Log("state: " + state);

        if (state == "launched")
        {
            // Move Player
            cam.transform.position = Vector3.Lerp(cam.transform.position, new Vector3(transform.position.x, cam.transform.position.y, transform.position.z + 10), 0.06f);
            transform.Translate(-Vector3.forward * rotSpeed * 10 * Time.deltaTime, Space.Self);
            transform.position = new Vector3(transform.position.x, 10, transform.position.z);
        }

        if (state == "orbit")
        {
            Rotate();
            RenderHookLineOrbit();
            cam.transform.position = Vector3.Lerp(cam.transform.position, new Vector3(Target.transform.position.x, cam.transform.position.y, Target.transform.position.z + 10), 0.02f);
        }

        OnTap();

        OldPositionSaved = transform.position;
    }

    void Rotate()
    {
        if(isClockwise) transform.RotateAround(Target.transform.position, Vector3.up, rotSpeed);
        else transform.RotateAround(Target.transform.position, -Vector3.up, rotSpeed);

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
            //Debug.Log("all good!");

            DetermineIfClockwise();

            hookLine.gameObject.SetActive(true);
            hookSendCouroutine = SendHook(0.4f); // Needs to be an actual hook value
            StartCoroutine(hookSendCouroutine);
            
            //SwitchToOrbit();
        }
    }

    IEnumerator SendHook(float hookSpeed) {
        while (hookTravelCounter < hookSpeed) {

            hookTravelCounter += Time.deltaTime / hookSpeed;
            //Debug.Log("sending hook couroutine");

            hookLine.SetPosition(0, transform.position);

            //somewhere here we will also need to figure out interseption course for moving hookables

            Vector3 newTargetPos = Vector3.Lerp(transform.position, Target.transform.position, hookTravelCounter);

            hookLine.SetPosition(1, newTargetPos);

            yield return null;
        }

        if(Target.GetComponent<ItemBehaviour>() != null)
        {
            Target.GetComponent<ItemBehaviour>().Die();
        }
        else
        {
            SwitchToOrbit();
        }

        hookTravelCounter = 0;
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

        launchCoolDown = 10;
        rotSpeed += speedIncrement;

        //DetermineIfClockwise();
    }

    private void DetermineIfClockwise()
    {
        Vector3 velocity = (transform.position - OldPositionSaved) / Time.deltaTime;
        Vector3 dir = Target.transform.position - transform.position;

        relativePosition = Vector3.Cross(velocity, dir);
        //Debug.Log(relativePosition);

        isClockwise = relativePosition.y >= 0;
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
