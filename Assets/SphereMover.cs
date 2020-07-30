using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereMover : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.Translate(-Vector3.forward * 0.2f);
    }
}
