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
                SpawnItem();
            }

            currentTimer = 0;
        }
    }

    void SpawnItem()
    {
        isObjectActive = true;

        int randIndex = Random.Range(0, Items.Count);
        Items[randIndex].SetActive(true);
        Items[randIndex].transform.position = new Vector3(PlayerBehaviour.Instance.GetPosition().x, PlayerBehaviour.Instance.GetPosition().y, PlayerBehaviour.Instance.GetPosition().z+60);
    }
}
