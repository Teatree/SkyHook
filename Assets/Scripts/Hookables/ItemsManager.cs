using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsManager : SceneSingleton<ItemsManager>
{
    public List<GameObject> Items;
    public float SpawnChance;
    [Header("In Seconds")]
    public float Timer;

    public bool isObjectActive;
    float currentTimer;

    void Start()
    {
        
    }
    
    void Update()
    {
        currentTimer+=Time.deltaTime;

        if (currentTimer >= Timer && isObjectActive == false)
        {
            if (Random.Range(0,1) < SpawnChance)
            {
                //SpawnItem();
            }

            currentTimer = 0;
        }
    }

    public void SpawnItem(Vector3 spawnLocation)
    {
        isObjectActive = true;

        int randIndex = Random.Range(0, Items.Count);
        Items[randIndex].SetActive(true);
        Items[randIndex].transform.position = spawnLocation;
    }
}
