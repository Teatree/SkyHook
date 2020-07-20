using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClawableArea : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        transform.parent.GetComponent<PlayerBehaviour>().SetClawTarget(other.gameObject);
    }
}
