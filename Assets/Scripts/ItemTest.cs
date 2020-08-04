using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemTest : MonoBehaviour 
{
    Vector3 dirNormalized;

    private void OnTriggerEnter(Collider c)
    {
        Destroy(gameObject);
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
            transform.position = transform.position + dirNormalized * 7 * Time.deltaTime;
        }
    }
}
