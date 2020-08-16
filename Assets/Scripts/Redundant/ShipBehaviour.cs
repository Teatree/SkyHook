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
        else if(other.GetComponent<BadItemTest>() != null)
        {
            Player.Instance.OnTriggerDead();
        }
        else
        {
            GameSystem.Instance.SetState(new FinishedState(GameSystem.Instance));
        }
    }
}
