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
        timer += Time.deltaTime * rotSpeed;

        float x = -Mathf.Cos(timer) * xSpread;
        float y = Mathf.Sin(timer) * ySpread;
        Vector3 pos = new Vector3(x, y, zOffset);
        Vector3 newPost = pos + Target.transform.position;
        transform.position = Vector3.Slerp(transform.position, newPost, 0.02f);

        transform.LookAt(Target.transform);

        //Vector3 dir = new Vector3(Target.transform.position.x, transform.position.y, Target.transform.position.z);
        ////transform.rotation = Quaternion.LookRotation(dir - transform.position, Vector3.up);

        //transform.LookAt(dir);
    
        ////transform.eulerAngles = new Vector3(transform.eulerAngles.x, -90, transform.eulerAngles.z);
    }

    void OnTap()
    {
        launchCoolDown--;
        //Debug.Log("launchCoolDown: " + launchCoolDown);

        if (Input.GetMouseButton(0) && state == "orbit" && launchCoolDown <= 0)
        {
            Time.timeScale = 0.25f;
        }

        else if (Input.GetMouseButtonUp(0) && state == "orbit" && launchCoolDown <= 0)
        {
            Time.timeScale = 1f;
            Target = null;

            state = "launched";

            // push
            var look = OldPositionSaved - transform.position;
            var distance = look.magnitude;
            var dir = look / distance;
            var velocity = (transform.position - OldPositionSaved) / Time.deltaTime;

            transform.eulerAngles = new Vector3(transform.eulerAngles.x, Mathf.Abs(transform.eulerAngles.y), transform.eulerAngles.z);
            transform.gameObject.GetComponent<Rigidbody>().AddRelativeForce(-dir * velocity.magnitude * 100);

            Debug.Log("force: " + -dir * velocity.magnitude * 100);
        }

        else if (Input.GetMouseButtonUp(0) && Target != null && state == "launched")
        {
            state = "orbit";
            transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
            transform.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

            //cam.transform.position = new Vector3(Target.transform.position.x, Target.transform.position.y + 10, cam.transform.position.z);

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
}
