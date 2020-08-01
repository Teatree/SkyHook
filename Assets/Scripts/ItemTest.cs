using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemTest : MonoBehaviour 
{
    private void OnTriggerEnter(Collider c)
    {
        Destroy(gameObject);
    }
}
