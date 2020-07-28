using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickController : SceneSingleton<JoystickController>
{
    public Camera mainCamera;
    public LineRenderer LRenderer;

    Vector3 touchStartPos;
    Vector3 touchEndPos;
    Vector2 lastDirection;
    public float maxScale;

    Vector3 endPos;
    Vector3 startPos;

    internal Vector3 GetDirection() { return lastDirection; }

    internal Vector3 GetPosition() { return endPos; }

    void Update()
    {
        if(Input.touchCount != 0)
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    touchStartPos = touch.position;
                    break;
                case TouchPhase.Stationary:
                case TouchPhase.Moved:
                    touchEndPos = touch.position;
                    break;
                case TouchPhase.Canceled:
                case TouchPhase.Ended:
                    touchStartPos = Vector3.zero;
                    break;
            }
        }

        if (Input.GetMouseButtonDown(0)) { touchStartPos = Input.mousePosition; }
        else if (Input.GetMouseButtonUp(0)) { touchStartPos = Vector2.zero; }

        if (Input.GetMouseButton(0)) { touchEndPos = Input.mousePosition; }

        if (touchStartPos != Vector3.zero)
        {
            startPos = touchStartPos;
            startPos.z = 1;

            endPos = touchEndPos;
            endPos.z = 1;

            startPos = mainCamera.ScreenToWorldPoint(startPos);
            endPos = mainCamera.ScreenToWorldPoint(endPos);

            float distance = Vector3.Distance(endPos, startPos);

            if (distance > maxScale)
            {              
                Vector3 fromOriginToObject = endPos - startPos; 
                fromOriginToObject *= maxScale / distance; 
                endPos = startPos + fromOriginToObject;
            }

            LRenderer.SetPosition(0, startPos);
            LRenderer.SetPosition(1, endPos);

            lastDirection = Quaternion.AngleAxis(90 - transform.eulerAngles.x, Vector3.right) * (endPos - startPos);
        }
        else
        {
            //LRenderer.SetPosition(0, Vector3.zero);
            //LRenderer.SetPosition(1, Vector3.zero);
            lastDirection = Quaternion.AngleAxis(90 - transform.eulerAngles.x, Vector3.right) * (endPos - startPos);
        }
    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(startPos, 0.004f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(endPos, 0.004f);
    }
}
