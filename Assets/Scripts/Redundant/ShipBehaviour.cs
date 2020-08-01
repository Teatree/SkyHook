using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipBehaviour : MonoBehaviour 
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<ItemTest>() != null)
        {
            Player.Instance.OnTriggerEnt();
        }
        else
        {
            Player.Instance.OnTriggerDead();
        }
    }
}
