using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hookable : MonoBehaviour
{
    public Rigidbody rigidB;

    void Start()
    {
        GetComponent<Rigidbody>().velocity = Random.onUnitSphere * (Random.Range(0.1f, 1)*2);
    }
}
