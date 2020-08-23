using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipBehaviour : MonoBehaviour 
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Item>() != null)
        {
            //Player.Instance.OnTriggerEnt();
        }
        else if(other.GetComponent<EnemyBird>() != null)
        {
            //Player.Instance.OnTriggerDead();
        }
        else if(other.gameObject.layer == 14)
        {
            GameSystem.Instance.SetState(new FinishedState(GameSystem.Instance));
        }
    }
}
