using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldBuilder : SceneSingleton<WorldBuilder>
{
    public GameObject hookablePref;
    int AreaMinX = -50;
    int AreaMaxX = 50;
    int AreaMinZ = -50;
    int AreaMaxZ = 50;

    public int AcceptableSpawnDistance;
    public int acceptableInstantiationDistance;

    public int hookableCount;

    public Vector3[] SpawnerPoint;

    public List<GameObject> hookables = new List<GameObject>();

    public List<Vector3> locations = new List<Vector3>();
    public List<Vector3> usedLocations = new List<Vector3>();

    void Start()
    {
        SpawnerPoint = new Vector3[4]; 

        locations.Add(hookables[0].transform.position);
        usedLocations.Add(hookables[0].transform.position);

        GenerateLocationsInitial(AreaMinX, AreaMaxX, AreaMinZ, AreaMaxZ, AcceptableSpawnDistance);

        InstantiateSpinners();
    }

    private void Update()
    {
        GenerateLocationsOngoing();

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

    public void GenerateLocationsOngoing()
    {
        // Generating locations on the fly, smartly
        SpawnerPoint = PlayerBehaviour.Instance.GetPointInDirectionFacing();

        //hookables[2].transform.position = new Vector3(SpawnerPoint.x + Random.Range(-8, 8), SpawnerPoint.y, SpawnerPoint.z + Random.Range(-8, 8));

        // first check if you can spawn at nose
        //GenerateLocationsInitial(SpawnerPoint.x+10, SpawnerPoint.y, SpawnerPoint.z, AcceptableSpawnDistance);


        // when yes, generate a field of points
    }

    public void GenerateLocationsInitial(float areaMinX, float areaMaxX, float areaMinZ, float areaMaxZ, int acceptableSpawnDistance)
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;

        Gizmos.DrawSphere(SpawnerPoint[0], 0.8f);
        Gizmos.DrawSphere(SpawnerPoint[1], 0.8f);
        Gizmos.DrawSphere(SpawnerPoint[2], 0.8f);
        Gizmos.DrawSphere(SpawnerPoint[3], 0.8f);
    }
}
