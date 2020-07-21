using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBehaviour : MonoBehaviour
{
    public GameObject deathEffect;
    public float minPush = 0.8f;
    public float maxPush = 1.4f;

    public float lifespan;
    float currentLife;

    public float spinnerSpinSpeed;

    void OnEnable()
    {
        GetComponent<Rigidbody>().velocity = Random.onUnitSphere * (Random.Range(minPush, maxPush) * 2);
    }

    void Update()
    {
        transform.Rotate(0, spinnerSpinSpeed * Time.deltaTime, 0);

        // die timer
        currentLife += Time.deltaTime;
        if(currentLife >= lifespan)
        {
            Die();
        }
    }

    public void Die()
    {
        ItemsManager.Instance.isObjectActive = false;

        GameObject v = Instantiate(deathEffect);
        v.transform.position = transform.position;

        gameObject.SetActive(false);
    }
}
