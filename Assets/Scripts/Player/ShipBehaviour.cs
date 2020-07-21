using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipBehaviour : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        PlayerBehaviour.Instance.CollisionEnter();
    }
    
}
