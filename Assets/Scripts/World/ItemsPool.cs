using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsPool : SceneSingleton<ItemsPool>
{
    public GameObject treePrefab;
    public GameObject bushPrefab;

    public int poolSize = 142;
    public List<GameObject> items;

    void Start()
    {
        for (int i = 0; i < poolSize; i++)
        {
            // GameObject o = Instantiate(treePrefab, new Vector3(0,0,0),Quaternion.identity, parentTile.transform );
            GameObject o = Instantiate(treePrefab );
            o.SetActive(false);
            items.Add(o);
        }
    }

    public GameObject getItem()
    {
        for (int i = 0; i < poolSize; i++)
        {
            if (!items[i].activeInHierarchy)
            {
                return items[i];
            }
        }

        return null;
    }
}