using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BadItemTest : MonoBehaviour
{
    Vector3 dirNormalized;
    Vector3 dirNormalizedHome;
    Vector3 initialPosition;

    private void OnTriggerEnter(Collider c)
    {

    }

    private void Start()
    {
        initialPosition = transform.position;
    }

    private void Update()
    {
        dirNormalized = (Player.Instance.GetPosition() - transform.position).normalized;
        dirNormalizedHome = (initialPosition - transform.position).normalized;

        //Debug.Log("d:" + Vector3.Distance(Player.Instance.GetPosition(), transform.position));
        if (Vector3.Distance(Player.Instance.GetPosition(), transform.position) >= 45)
        {
            transform.position = transform.position + dirNormalizedHome * 7 * Player.CurrentSpeedIncrement * Time.deltaTime;
            transform.LookAt(initialPosition);
        }
        else
        {
            transform.position = transform.position + dirNormalized * 7 * Player.CurrentSpeedIncrement * Time.deltaTime;
            transform.LookAt(Player.Instance.GetPosition());
        }
        //transform.position = transform.position + dirNormalized * 7 * Time.deltaTime;
    }
}
