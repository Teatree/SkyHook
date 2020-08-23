using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBird : MonoBehaviour
{
    public GameObject FireParticle;

    private void OnTriggerEnter(Collider c)
    {
        if (c.GetComponent<ShipBehaviour>() != null)
        {
            Player.Instance.TakeDamage(40);
            Die();
        }
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    public void TurnRed()
    {
        FireParticle.SetActive(true);
        FireParticle.GetComponent<ParticleSystem>().Play();
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }
}
