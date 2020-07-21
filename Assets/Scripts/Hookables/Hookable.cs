using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hookable : MonoBehaviour
{
    Color originalColor;
    public float minPush = 0.1f;
    public float maxPush = 0.4f;

    public int coinsAmount = 1;

    public float spinnerSpinSpeed;

    void Start()
    {
        originalColor = transform.gameObject.GetComponent<Renderer>().material.GetColor("_EmissionColor");
        GetComponent<Rigidbody>().velocity = Random.onUnitSphere * (Random.Range(minPush, maxPush) * 2);
    }

    private void Update()
    {
        //transform.Rotate(0, spinnerSpinSpeed * Time.deltaTime, 0);
        transform.LookAt(PlayerBehaviour.Instance.transform);
    }

    public int GetCoins()
    {
        return coinsAmount;
    }

    //public void OnTriggerExit(Collision collision)
    //{
    //    Debug.Log("exiting collision");
    //    transform.gameObject.GetComponent<Renderer>().material.SetColor("_EmissionColor", originalColor);
    //}
}
