using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour 
{
    Vector3 dirNormalized;
    bool shouldFall = true;

    IEnumerator HooKMeCouroutine;
    IEnumerator FallCoroutine;

    Vector3 InitialPositon;
    bool isMovingUpwards = false;

    //public float speed = 200;
    //public float scaleX = 10;
    //public float scaleZ = 15;
    //public float offsetX = 0;
    //public float offsetZ = 0;
    //
    //public bool isLinkOffsetScalePositiveX = false;
    //public bool isLinkOffsetScaleNegativeX = false;
    //public bool isLinkOffsetScalePositiveZ = false;
    //public bool isLinkOffsetScaleNegativeZ = false;
    //public bool isFigure8 = true;
    //
    //private float phase;
    //private float m_2PI = Mathf.PI * 2;
    //private Vector3 originalPosition;
    //private Vector3 pivot;
    //private Vector3 pivotOffset;
    //private bool isInverted = false;
    //private bool isRunning = false;

    private void Start()
    {
        // place on the floor
        //RaycastHit hit;
        //if (Physics.Raycast(transform.position, Vector3.down, out hit, 90, LayerMask.GetMask("Terrain")))
        //{
        //    transform.position = new Vector3(hit.point.x, hit.point.y + 2, hit.point.z);
        //}

        //pivot = transform.position;
        //originalPosition = transform.position;
        //isRunning = true;
        //
        //if (isLinkOffsetScalePositiveX)
        //    phase = 3.14f / 2f + 3.14f;
        //else if (isLinkOffsetScaleNegativeX)
        //    phase = 3.14f / 2f;
        //else if (isLinkOffsetScalePositiveZ)
        //    phase = 3.14f;
        //else
        //    phase = 0;
    }

    private void Update()
    {
        dirNormalized = (Player.Instance.GetPosition() - transform.position).normalized;

        if (Vector3.Distance(Player.Instance.GetPosition(), transform.position) <= 1)
        {
            enabled = false;  // causes that Update() of this MonoBehavior is not called anymore (until enabled is set back to true)
                              // Do whatever you want when the object is close to its target here
        }
        else
        {
            //transform.position = transform.position + dirNormalized * 7 * Time.deltaTime;
        }

        if (isMovingUpwards == false)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 90, LayerMask.GetMask("Terrain")))
            {
                if (shouldFall == true)
                {
                    FallCoroutine = FallDown(1);
                    StartCoroutine(FallCoroutine);

                    shouldFall = false;
                }
            }
        }

        //pivotOffset = Vector3.forward * 2 * scaleZ;

        //phase += speed * 2 * Time.deltaTime;

        //if (isFigure8)
        //{
        //    if (phase > m_2PI)
        //    {
        //        Debug.Log("phase " + phase + " over 2pi: " + isInverted);
        //        isInverted = !isInverted;
        //        phase -= m_2PI;
        //    }
        //    if (phase < 0)
        //    {
        //        Debug.Log("phase " + phase + " under 0");
        //        phase += m_2PI;
        //    }
        //}

        //Vector3 nextPosition = pivot + (isInverted ? pivotOffset : Vector3.zero);
        //transform.position = new Vector3(nextPosition.x + Mathf.Sin(phase) * scaleX + offsetX, nextPosition.y, nextPosition.z + Mathf.Cos(phase) * (isInverted ? -1 : 1) * scaleZ + offsetZ);
    }

    IEnumerator FallDown(float time)
    {
        //Debug.Log("moving?");
        float counter = 0;
        while (counter < time)
        {
            counter += Time.deltaTime;

            yield return null;
        }

        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 90, LayerMask.GetMask("Terrain")))
        {
            transform.position = new Vector3(hit.point.x, hit.point.y + 2, hit.point.z);
            InitialPositon = transform.position;
        }

        counter = 0;
    }

    IEnumerator MoveUpdwardsToPlayer(float time)
    {
        // Debug.Log("moving?");
        float counter = 0;
        
        while (counter <= time)
        {
            counter += Time.deltaTime / time;
           
            //somewhere here we will also need to figure out interseption course for moving hookables
            Vector3 newTargetPos = Vector3.Lerp(InitialPositon, Player.Instance.GetPosition(), counter);
            transform.position = newTargetPos;

            yield return null;
        }

        Destroy(gameObject);
        Player.Instance.OnTriggerEnt();

        counter = 0;
    }

    public void KillMe()
    {
        if (isMovingUpwards == false)
        {
            HooKMeCouroutine = MoveUpdwardsToPlayer(1f);
            StartCoroutine(HooKMeCouroutine);
            isMovingUpwards = true;
        }
    }
}
