using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPositionGiver : SceneSingleton<CameraPositionGiver>
{
    // This script gives Camera Position to understand when the world cubes should be updated

    public Vector3 RightEdge;
    public Vector3 LeftEdge;
    public Vector3 ForwardEdge;
    public Vector3 BehindEdge;

    public bool ReachedRightEdge = false;
    public bool ReachedLeftEdge = false;
    public bool ReachedForwardEdge = false;
    public bool ReachedBEhindEdge = false;

    public void Start()
    {
        UpdateEdges();
    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(RightEdge, 0.4f);
        Gizmos.DrawSphere(LeftEdge, 0.4f);
        Gizmos.DrawSphere(ForwardEdge, 0.4f);
        Gizmos.DrawSphere(BehindEdge, 0.4f);
    }

    private void Update()
    {
        if(transform.position.x > RightEdge.x) {
            //Debug.Log("Reached Right Edge");
            ReachedRightEdge = true;
            UpdateEdges();
        } else {
            ReachedRightEdge = false;
        }

        if (transform.position.x < LeftEdge.x)
        {
            //Debug.Log("Reached Left Edge");
            ReachedLeftEdge = true;
            UpdateEdges();
        }
        else
        {
            ReachedLeftEdge = false;
        }

        if (transform.position.z > ForwardEdge.z)
        {
            //Debug.Log("Reached Forward Edge");
            ReachedForwardEdge = true;
            UpdateEdges();
        }
        else
        {
            ReachedForwardEdge = false;
        }

        if (transform.position.z < BehindEdge.z)
        {
            //Debug.Log("Reached Back Edge");
            ReachedBEhindEdge = true;
            UpdateEdges();
        }
        else
        {
            ReachedBEhindEdge = false;
        }
    }

    public void UpdateEdges()
    {
        //Debug.Log("Updating Edges");
        RightEdge = new Vector3(transform.position.x + 9, transform.position.y, transform.position.z);
        LeftEdge = new Vector3(transform.position.x - 9, transform.position.y, transform.position.z);
        ForwardEdge = new Vector3(transform.position.x, transform.position.y, transform.position.z + 7);
        BehindEdge = new Vector3(transform.position.x, transform.position.y, transform.position.z - 7);
    }
}
