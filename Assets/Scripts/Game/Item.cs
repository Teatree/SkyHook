using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour 
{
    Vector3 dirNormalized;
    bool shouldFall = true;
    public ItemWorld[] itemWorld;

    IEnumerator HooKMeCouroutine;
    IEnumerator FallCoroutine;

    Vector3 InitialPositon;
    bool isMovingUpwards = false;

    private void Start()
    {
        int r = Random.RandomRange(0, itemWorld.Length-1);
        transform.gameObject.name = itemWorld[r].name;
        transform.GetComponent<MeshFilter>().mesh = itemWorld[r].mesh;

        // place on the floor
        //RaycastHit hit;
        //if (Physics.Raycast(transform.position, Vector3.down, out hit, 90, LayerMask.GetMask("Terrain")))
        //{
        //    transform.position = new Vector3(hit.point.x, hit.point.y + 2, hit.point.z);
        //}
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
