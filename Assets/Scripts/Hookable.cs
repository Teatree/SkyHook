using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hookable : MonoBehaviour
{
    public Rigidbody rigidB;
    public float minPush = 0.1f;
    public float maxPush = 0.4f;

    void Start()
    {
        GetComponent<Rigidbody>().velocity = Random.onUnitSphere * (Random.Range(minPush, maxPush) * 2);
    }
}
