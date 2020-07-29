using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemsManager : SceneSingleton<ItemsManager>
{
    [Header("Item Spawning")]
    public Item[] Items;
    [Header("Item Random Spawn")]
    public GameObject ItemRandomSpawn;
    public float SpawnChance;
    [Tooltip("In Seconds")]
    public float SpawnTimerForRandom;
    float currentTimer;
    [Header("Item Specialized Spawn")]
    public GameObject ItemSpecializedSpawn;
    public string ItemSpecializedSpawnMethod;

    [Header("Clue Gathering")]
    bool DoHaveClue;
    float clueAppearCounter;
    float clueLifeTime;
    float clueBreakTime;
    float currentNumberOfClues;
    Vector3 directionPositionSaved;

    [System.Serializable]
    public struct Item {
        public GameObject item;
        [Tooltip("prob is weighed don worry")]
        public float prob;
    }

    void Start()
    {
        clueBreakTime = SessionController.Instance.BreakTimeBeforeNextClue;

        ChooseNextRandomSpawnObject();
        ChooseNextSpecialSpawnObject();
        ChooseNextSpecialSpawnMethod();
    }
    
    void Update()
    {
        ItemRandomSpawning();

        ItemSpecializedSpawning();
    }

    void ChooseNextRandomSpawnObject()
    {
        float totals = 0;
        foreach (Item i in Items)
        {
            totals += i.prob;
        }
        float randomP = Random.value * totals;
        for (int i = 0; i < Items.Length; i++)
        {
            if(randomP < Items[i].prob)
            {
                ItemRandomSpawn = Items[i].item;
                Items[i].item.GetComponent<ItemBehaviour>().type = "random";
                break;
            }
            else
            {
                randomP -= Items[i].prob;
            }
        }
    }

    void ChooseNextSpecialSpawnObject()
    {
        float totals = 0;
        foreach (Item i in Items)
        {
            totals += i.prob;
        }
        float randomP = Random.value * totals;
        for (int i = 0; i < Items.Length; i++)
        {
            if (randomP < Items[i].prob && Items[i].item.name != ItemRandomSpawn.name) // This second check is very stupid and bad.
            {
                ItemSpecializedSpawn = Items[i].item;
                Items[i].item.GetComponent<ItemBehaviour>().type = "special";
                break;
            }
            else
            {
                randomP -= Items[i].prob;
            }
        }
    }

    void ChooseNextSpecialSpawnMethod()
    {
        // Choose from multiple methods of specialized types of spawning objects
    }

    void ItemRandomSpawning()
    {
        currentTimer += Time.deltaTime;

        if (currentTimer >= SpawnTimerForRandom && ItemRandomSpawn != null)
        {
            SpawnItem(PlayerBehaviour.Instance.GetPointInDirectionFacing()[4]);
            currentTimer = 0;
        }
    }

    void ItemSpecializedSpawning()
    {
        // to be refactored into several way of spawning items
        // 1. random 2. clues 3. dragon / bird 4. acumulative (get this many spinenrs)

        // Choose item to spawn
        // Choose method of spawning
        // Execute and don't bother until done or failed (except for randoms)

        if (clueBreakTime >= SessionController.Instance.BreakTimeBeforeNextClue && ItemRandomSpawn != null)
        {
            if (DoHaveClue == false)
            {
                clueAppearCounter += Time.deltaTime;
                if (clueAppearCounter >= SessionController.Instance.TimeBeforeFirstClue)
                {
                    ChooseARandomHookableForClue();
                    clueAppearCounter = 0;
                }
            }
            else
            {
                clueLifeTime -= Time.deltaTime;
                // check whether distance from clue is sufficient, if yes make another
                if (directionPositionSaved != null)
                {
                    float dist = Vector3.Distance(PlayerBehaviour.Instance.GetPosition(), directionPositionSaved);
                    //Debug.Log("dist: " + dist);

                    if (dist < 25)
                    {
                        directionPositionSaved = new Vector3();
                        ChooseARandomHookableForClue();
                    }
                }
            }
        }
        else
        {
            clueBreakTime += Time.deltaTime;
            float dist = Vector3.Distance(PlayerBehaviour.Instance.GetPosition(), directionPositionSaved);
            if (dist < 25)
            {
                SpinnerSpawnController.Instance.PlaceRadarParticle(directionPositionSaved);
                ItemsManager.Instance.SpawnItem(directionPositionSaved);

                directionPositionSaved = new Vector3();
            }
        }
    }

    #region Clues
    public GameObject IdentifyPotentialClue(Vector3 lookPoint)
    {
        List<GameObject> hookables = SpinnerSpawnController.Instance.GetHookables();

        float smallest = 50;
        GameObject goWithSmallest = hookables[1];

        foreach (GameObject g in hookables)
        {
            float dist = Vector3.Distance(g.transform.position, lookPoint);
            if (dist < smallest)
            {
                smallest = dist;
                goWithSmallest = g;
            }
        }

        Debug.Log("identifying go with smallest as " + goWithSmallest.transform.position);
        return goWithSmallest;
    }

    public void ChooseARandomHookableForClue()
    {
        // Choose the last gameObject to have been moved. 
        // Why? because this is the best way of knowing that it's a object popping up in 
        // the general direction Player is moving.
        GameObject chosenClueGuy = IdentifyPotentialClue(PlayerBehaviour.Instance.GetPointInDirectionFacing()[4]);
        if (chosenClueGuy != null)
        {
            chosenClueGuy.GetComponent<Hookable>().MakeClue();
            chosenClueGuy.gameObject.name = "Clue";
            SpinnerSpawnController.Instance.RemoveFromHookables(chosenClueGuy);

            DoHaveClue = true;
        }
    }

    public void ClueKilled()
    {
        DoHaveClue = false;

        //Debug.Log("Resettings Clues -- beacuse died ");
        //currentNumberOfClues = 0;
    }

    public void SaveDirection(Vector3 v)
    {
        directionPositionSaved = v;

        currentNumberOfClues++;
        if (currentNumberOfClues == SessionController.Instance.NumberOfCluesInitial)
        {
            Debug.Log("Break Starts");
            clueBreakTime = 0;
            currentNumberOfClues = 0;
        }
    }
    #endregion

    public void RandomDied()
    {
        ItemRandomSpawn = null;
        ChooseNextRandomSpawnObject();
    }

    public void SpecialDied()
    {
        ItemSpecializedSpawn = null;
        ChooseNextSpecialSpawnObject();
        ChooseNextSpecialSpawnMethod();
    }

    public void SpawnItem(Vector3 spawnLocation)
    {
        foreach(Item i in Items)
        {
            if(i.item.name == ItemRandomSpawn.name)
            {
                i.item.SetActive(true);
                i.item.transform.position = spawnLocation;
            }
        }
    }
}
