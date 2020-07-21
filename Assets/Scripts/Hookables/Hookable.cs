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

    [Header("Clue Gathering")]
    public bool IsClue;
    public Color ClueColour;
    public Vector3 AreaOfNextClue;
    GameObject arrowGo;

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

    public void SetOriginalColour()
    {
        transform.gameObject.GetComponent<Renderer>().material.SetColor("_EmissionColor", originalColor);
    }

    public void MakeClue()
    {
        IsClue = true;
        SetColourToClue();
        GenerateDirectionOfNextClue();
    }

    private void SetColourToClue()
    {
        transform.gameObject.GetComponent<Renderer>().material.SetColor("_EmissionColor", ClueColour);
    }

    private void GenerateDirectionOfNextClue()
    {
        // Generate a point in some random direction and point at it.
        AreaOfNextClue = Vector3.up * 100;
    }

    public void RevealDirection()
    {
        SpinnerSpawnController.Instance.PlaceArrowAndPoint(transform.position, AreaOfNextClue);
    }
}
