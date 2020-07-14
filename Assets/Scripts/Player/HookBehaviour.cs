using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookBehaviour : MonoBehaviour
{
    [Header("In Seconds")]
    public int hookTimer;
    public Material mat;
    public Color initialColour;
    public Color endColour;

    Color currentColour;
    float currentColourTimer;
    float currentTimer;

    private void OnEnable()
    {
        currentColour = initialColour;
        currentColourTimer = 0;

        currentTimer = hookTimer;
    }

    void Update()
    {
        currentTimer -= Time.deltaTime;
        currentColourTimer += Time.deltaTime / (hookTimer-1);

        currentColour = Color.Lerp(initialColour, endColour, currentColourTimer);

        mat.SetColor("_EmissionColor", currentColour);

        if (currentTimer <= 0)
        {
            PlayerBehaviour.Instance.SwitchToLaunched();
            mat.SetColor("_EmissionColor", initialColour);
        }
    }
}
