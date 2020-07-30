using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClawableArea : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        //Debug.Log("colliding");
        transform.parent.GetComponent<PlayerBehaviour>().SetClawTarget(other.gameObject);
    }
}
