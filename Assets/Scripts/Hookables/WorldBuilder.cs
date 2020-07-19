using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldBuilder : MonoBehaviour
{
    public GameObject hookablePref;
    public int areaMinX = -300;
    public int areaMaxX = 300;
    public int areaMinZ = -200;
    public int areaMaxZ = 200;

    public int acceptableSpawnDistance;
    public int acceptableInstantiationDistance;

    public int hookableCount;

    public List<GameObject> hookables = new List<GameObject>();

    public List<Vector3> locations = new List<Vector3>();
    public List<Vector3> usedLocations = new List<Vector3>();

    void Start()
    {
        locations.Add(hookables[0].transform.position);
        usedLocations.Add(hookables[0].transform.position);

        GenerateLocations();

        InstantiateSpinners();
    }

    private void Update()
    {
        if (PlayerBehaviour.Instance.isLaunched())
        {
            List<Vector3> toBeInstantiated = new List<Vector3>();
            foreach (Vector3 v in locations)
            {
                if (Vector3.Distance(PlayerBehaviour.Instance.GetPosition(), v) < acceptableInstantiationDistance && usedLocations.Contains(v) == false)
                {
                    toBeInstantiated.Add(v);
                }
            }

            List<GameObject> toBeRelocated = new List<GameObject>();
            foreach (GameObject g in hookables)
            {
                if (Vector3.Distance(g.transform.position, PlayerBehaviour.Instance.GetPosition()) > acceptableInstantiationDistance)
                {
                    toBeRelocated.Add(g);
                    usedLocations.Remove(g.transform.position);
                }
            }

            for (int i = 0; i < toBeRelocated.Count; i++)
            {
                if (i < toBeInstantiated.Count)
                {
                    toBeRelocated[i].transform.position = toBeInstantiated[i];
                    toBeRelocated[i].GetComponent<Hookable>().spinnerSpinSpeed += 50;
                    usedLocations.Add(toBeInstantiated[i]);


                }
            }
        }
    }

    public void GenerateLocations()
    {
        Debug.Log("Generating Hookable Positions");
        for (int i = 0; i < hookableCount; i++)
        {
            bool isPositionOk = false;
            Vector3 generatedLocation = new Vector3();
            int attempts = 5;

            while (isPositionOk == false && attempts >= 0)
            {
                attempts--;
                generatedLocation = new Vector3(Random.Range(areaMinX, areaMaxX), 10, Random.Range(areaMinZ, areaMaxZ));

                isPositionOk = true;
                foreach (Vector3 h in locations)
                {
                    if (Vector3.Distance(generatedLocation, h) < acceptableSpawnDistance)
                    {
                        isPositionOk = false;
                    }
                }
            }

            if (isPositionOk)
            {
                
                locations.Add(generatedLocation);
            }
            else
            {
                //Debug.Log("omg I failed to create a hookable! so Sorry");
            }
        }
    }

    public void InstantiateSpinners()
    {
        foreach (Vector3 v in locations)
        {
            if (Vector3.Distance(PlayerBehaviour.Instance.transform.position, v) < acceptableInstantiationDistance && usedLocations.Contains(v) == false)
            {
                GameObject t = Instantiate(hookablePref, transform);
                t.transform.position = v;

                t.GetComponent<Hookable>().spinnerSpinSpeed = 15;

                hookables.Add(t);
                usedLocations.Add(v);
            }
        }
    }
}
