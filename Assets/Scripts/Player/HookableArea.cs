using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookableArea : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        transform.parent.GetComponent<PlayerBehaviour>().SetTarget(other.gameObject);
    }

    void OnTriggerExit(Collider other)
    {
        PlayerBehaviour.Instance.CollisionExit();
    }
}
