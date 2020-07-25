using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundCameraMovement : MonoBehaviour
{
    float currentCamSpeed;
    float idleCamSpeed;

    void Start()
    {
        idleCamSpeed = 0.003f;
        currentCamSpeed = idleCamSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("currentCamSpeed: " + currentCamSpeed);
        if (PlayerTest.Instance.state == PlayerTest.PlayerState.launched)
        {
            currentCamSpeed = PlayerTest.Instance.currentSpeed / 150;
            //currentCamSpeed = PlayerTest.Instance.GetDistance() / PlayerTest.Instance.currentSpeed / 100;
            //Debug.Log("currentCamSpeed: " + currentCamSpeed);
        }
        else
        {
            currentCamSpeed = idleCamSpeed;
        }

        Vector3 v = PlayerTest.Instance.targetPositon;
        v.y = 140;
        transform.Translate(v * currentCamSpeed * Time.deltaTime, Space.World);
        transform.position = new Vector3(transform.position.x, 140, transform.position.z);
        
    }
}
