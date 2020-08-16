using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemTest : MonoBehaviour 
{
    Vector3 dirNormalized;
    bool shouldFall = true;

    IEnumerator HooKMeCouroutine;
    bool isMovingUpwards = false;

    private void OnTriggerEnter(Collider c)
    {
        Destroy(gameObject);
    }

    private void Start()
    {
        // place on the floor
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 90, LayerMask.GetMask("Terrain")))
        {
            transform.position = new Vector3(hit.point.x, hit.point.y + 2, hit.point.z);
        }
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

        if (shouldFall)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 90, LayerMask.GetMask("Terrain")))
            {
                transform.position = new Vector3(hit.point.x, hit.point.y + 2, hit.point.z);
                shouldFall = false;
            }
        }
    }

    IEnumerator MoveUpdwardsToPlayer(float time)
    {
        Debug.Log("moving?");
        float counter = 0;
        while (counter < time)
        {
            counter += Time.deltaTime;
           
            //somewhere here we will also need to figure out interseption course for moving hookables
            Vector3 newTargetPos = Vector3.Lerp(transform.position, Player.Instance.GetPosition(), counter / time);
            transform.position = newTargetPos;

            yield return null;
        }

        counter = 0;
    }

    public void KillMe()
    {
        if (isMovingUpwards == false)
        {
            HooKMeCouroutine = MoveUpdwardsToPlayer(5);
            StartCoroutine(HooKMeCouroutine);
            isMovingUpwards = true;
        }
    }
}
