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
    IEnumerator dieInTimeCoroutine;
    float dieCounter;

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
        SpinnerSpawnController.Instance.PlaceRadarParticle(transform.position);
        SetColourToClue();

        // Die in lifetime
        dieInTimeCoroutine = DieInTime(20);
        StartCoroutine(dieInTimeCoroutine);
    }

    IEnumerator DieInTime(float dieTime)
    {
        while (dieCounter < dieTime)
        {
            dieCounter += Time.deltaTime;

            yield return null;
        }

        ItemsManager.Instance.ClueKilled();
        Debug.Log(gameObject.name + ": I died");
        Destroy(this.gameObject);
    }

    public void Die(float secs)
    {
        dieInTimeCoroutine = DieInTime(secs); 
        StartCoroutine(dieInTimeCoroutine);
    }

    private void SetColourToClue()
    {
        transform.gameObject.GetComponent<Renderer>().material.SetColor("_EmissionColor", ClueColour);
    }

    public void GenerateDirectionOfNextClue()
    {
        IsClue = false;
        // Generate a point in some random direction and point at it.
        AreaOfNextClue = transform.position + (Vector3.forward * 100) + (Vector3.right * Random.Range(-25, 25)) + (Vector3.left * Random.Range(-25, 25));
        AreaOfNextClue.y = 7.1f;
    }

    public void RevealDirection()
    {
        SpinnerSpawnController.Instance.PlaceArrowAndPoint(transform.position, AreaOfNextClue);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawSphere(AreaOfNextClue, 1.8f);
        //Gizmos.DrawSphere(transform.transform.position + (Vector3.forward * 100) + (Vector3.right * Random.Range(-25,25)) + (Vector3.left * Random.Range(-25, 25)), 1.8f);
        
    }
}
