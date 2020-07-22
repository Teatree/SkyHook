using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : SceneSingleton<PlayerBehaviour>
{
    public GameObject Target;
    public GameObject ClawTarget;
    public Color HighlightColor;
    public GameObject Ship;
    public SphereCollider hookable;
    public LineRenderer hookLine;
    public SphereCollider ClawArea;
    public LineRenderer ClawLine;
    public Camera cam;
    public GameObject deathExplosion;
    
    public float currentDistance = 2;
    float currentSpeed;
    string state;
    float hookTravelCounter = 0;
    float clawTravelCounter = 0;
    bool isClockwise;
    bool isLatchedOn;
    bool isSendingClaw;

    Vector3 orbitPoint;
    int launchCoolDown;
    Vector3 oldPositionSaved;
    Vector3 relativePosition;
    IEnumerator hookSendCouroutine;
    IEnumerator clawSendCouroutine;
    IEnumerator clawBackCouroutine;

    void Start()
    {
        state = "orbit";
        cam.transform.position = new Vector3(Target.transform.position.x, cam.transform.position.y, Target.transform.position.z+10);

        currentSpeed = PlayerData.Instance.IntitialSpeed;
    }
     
    void Update()
    {
        float start = 1;
        float end = 9000;
        start += (end - start) * Mathf.Pow(7, 12);
        end -= (end - start) * Mathf.Pow(7, 12);

        if (state == "launched")
        {
            //currentDistance += currentSpeed;
            currentSpeed += ((1 - Mathf.Pow(1 - currentSpeed / SessionController.Instance.PlayerSpeedMaxSpeedf, SessionController.Instance.PlayerSpeedIncreaseValuef))/100);
            //Debug.Log("currentSpeed - " + currentSpeed);

            // Move Player
            cam.transform.position = Vector3.Lerp(cam.transform.position, new Vector3(transform.position.x, cam.transform.position.y, transform.position.z + 10), 0.06f);
            transform.Translate(-Vector3.forward * currentSpeed/10 * Time.deltaTime, Space.Self);
            transform.position = new Vector3(transform.position.x, 10, transform.position.z);
        }

        if (state == "orbit")
        {
            Rotate();
            RenderHookLineOrbit();
            cam.transform.position = Vector3.Lerp(cam.transform.position, new Vector3(Target.transform.position.x, cam.transform.position.y, Target.transform.position.z + 10), 0.02f);
        }

        OnTap();

        oldPositionSaved = transform.position;
    }

    void Rotate()
    {
        // keep at orbit distance
        Vector3 orbitPos = Target.transform.position + (transform.position - Target.transform.position).normalized * PlayerData.Instance.OrbitDistance;
        transform.position = Vector3.Lerp(transform.position, orbitPos, 0.01f);

        if(isClockwise) transform.RotateAround(Target.transform.position, Vector3.up, currentSpeed * Time.deltaTime);
        else transform.RotateAround(Target.transform.position, -Vector3.up, currentSpeed * Time.deltaTime);

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
            DetermineIfClockwise();

            hookLine.gameObject.SetActive(true);
            hookSendCouroutine = SendHook(0.01f); // Needs to be an actual hook value
            StartCoroutine(hookSendCouroutine);
        }
    }

    IEnumerator SendHook(float hookSpeed) {
        while (hookTravelCounter < hookSpeed) {

            hookTravelCounter += Time.deltaTime / hookSpeed;

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

    IEnumerator SendClaw(float clawSpeed)
    {
        while (clawTravelCounter < clawSpeed)
        {
            clawTravelCounter += Time.deltaTime / clawSpeed;

            ClawLine.SetPosition(0, transform.position);
            Vector3 newTargetPos = Vector3.Lerp(transform.position, ClawTarget.transform.position, clawTravelCounter);
            ClawLine.SetPosition(1, newTargetPos);

            yield return null;
        }

        isLatchedOn = true;
        clawTravelCounter = 0;

        // bring item back in the claw
        clawBackCouroutine = ClawBringBack(0.9f);
        StartCoroutine(clawBackCouroutine);
    }

    IEnumerator ClawBringBack(float clawSpeed)
    {
        while (clawTravelCounter < clawSpeed)
        {
            clawTravelCounter += Time.deltaTime / clawSpeed;

            ClawLine.SetPosition(0, transform.position);
            Vector3 newTargetPos = Vector3.Lerp(transform.position, ClawTarget.transform.position, clawTravelCounter);
            ClawLine.SetPosition(1, newTargetPos);

            ClawTarget.transform.position = Vector3.Lerp(ClawTarget.transform.position, transform.position, clawTravelCounter);

            yield return null;
        }

        isSendingClaw = false;
        isLatchedOn = false;
        clawTravelCounter = 0;
        ClawLine.gameObject.SetActive(false);

        ClawTarget.GetComponent<ItemBehaviour>().Die();
        ClawTarget = null;
    }

    public void SetTarget(GameObject target)
    {
        if (state == "launched")
        {
            Target = target;
            //NotHighlightColor = Target.GetComponent<Renderer>().material.GetColor("_EmissionColor");
            Target.GetComponent<Renderer>().material.SetColor("_EmissionColor", HighlightColor);
        }
    }

    public void SetClawTarget(GameObject target)
    {
        if (state == "launched" && isLatchedOn == false && isSendingClaw == false)
        {
            ClawTarget = target;

            //send claw
            isSendingClaw = true;
            ClawLine.gameObject.SetActive(true);
            clawSendCouroutine = SendClaw(0.5f);
            StartCoroutine(clawSendCouroutine);
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
        Target.GetComponent<Hookable>().SetOriginalColour();
        Target = null;

        hookLine.gameObject.SetActive(false);

        state = "launched";
    }

    public void SwitchToOrbit()
    {
        state = "orbit";

        launchCoolDown = 10;

        IncreaseCoins();

        //if this object has a clue, reveal it.
        if (Target.GetComponent<Hookable>().IsClue == true)
        {
            Target.GetComponent<Hookable>().GenerateDirectionOfNextClue();
            Target.GetComponent<Hookable>().RevealDirection();
        }
    }

    private void DetermineIfClockwise()
    {
        Vector3 velocity = (transform.position - oldPositionSaved) / Time.deltaTime;
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
        if (Target != null)
        {
            Target.GetComponent<Hookable>().SetOriginalColour();
            Target = null;
        }
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public bool isLaunched()
    {
        return state == "launched";
    }

    public void IncreaseCoins()
    {
        SessionController.Instance.CoinsAmountSession += Target.GetComponent<Hookable>().GetCoins();
    }

    public Vector3[] GetPointInDirectionFacing()
    {
        // notice that if you use TransformPoint and give it Vector.forward it takes forward from that transform, not world
        Vector3[] res = new Vector3[5];

        // nice try
        //res[0] = transform.TransformPoint((-Vector3.forward * 30) + (Vector3.right * 17.5f));                                                                                      
        //res[1] = transform.TransformPoint((-Vector3.forward * 45) - (Vector3.right * 17.5f));                                                                                       
        //res[2] = transform.TransformPoint((-Vector3.forward * 30) - (Vector3.right * 17.5f));                                                                                      
        //res[3] = transform.TransformPoint((-Vector3.forward * 45) + (Vector3.right * 17.5f));

        res[0] = transform.TransformPoint(-Vector3.forward * 30);
        res[0].x -= 25;
        res[0].z -= 20;

        res[1] = transform.TransformPoint(-Vector3.forward * 30);
        res[1].x += 25;
        res[1].z -= 20;

        res[2] = transform.TransformPoint(-Vector3.forward * 30);
        res[2].x -= 25;
        res[2].z += 20;

        res[3] = transform.TransformPoint(-Vector3.forward * 30);
        res[3].x += 25;
        res[3].z += 20;

        res[4] = transform.TransformPoint(-Vector3.forward * 60); //This last point is just for identifying a position right in front of Player, useful for placing clues as stuff...

        return res;
    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        //Gizmos.DrawSphere(GetPointInDirectionFacing(), 0.4f);
    }

    public string GetState()
    {
        return state;
    }
}
