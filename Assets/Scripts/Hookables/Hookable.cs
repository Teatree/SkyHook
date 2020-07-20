using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hookable : MonoBehaviour
{
    public Rigidbody rigidB;
    public float minPush = 0.1f;
    public float maxPush = 0.4f;

    public int coinsAmount = 1;

    public float spinnerSpinSpeed;

    void Start()
    {
        GetComponent<Rigidbody>().velocity = Random.onUnitSphere * (Random.Range(minPush, maxPush) * 2);
    }

    private void Update()
    {
        transform.Rotate(0, spinnerSpinSpeed * Time.deltaTime, 0);
    }

    public int GetCoins()
    {
        return coinsAmount;
    }
}
