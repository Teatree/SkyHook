using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookCollisionAreaBehaviour : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("colliding: ", other);
        if (other.GetComponent<Item>() != null)
        {
            Player.Instance.CollideWithItem(other.gameObject);
        }
    }

}
