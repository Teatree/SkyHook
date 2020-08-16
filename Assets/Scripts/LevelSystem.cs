using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSystem : SceneSingleton<LevelSystem> 
{
    public GameObject[] HomeGos;
    int AreaMinX = -50;
    int AreaMaxX = 50;
    int AreaMinZ = -50;
    int AreaMaxZ = 50;

    public int AcceptableSpawnDistance;
    //public int acceptableInstantiationDistance;

    public int homesCount;

    public Vector3[] SpawnerPoint;
    public List<Vector3> UsedSpawnerPoints;

    public float SpawnHomeDelay = 5;
    float counter;
    bool isSpawned = false;

    void Start()
    {
        SpawnerPoint = new Vector3[10]; // yes max homes is 10... There will never be more than 5 though :P
    }

    private void Update()
    {
        if (counter < SpawnHomeDelay)
        {
            counter += Time.deltaTime;
        }
        else if(isSpawned == false)
        {
            GenerateHomeLocations(); // Will be couroutine
            isSpawned = true;
        }

        if (isSpawned)
        {
            Player.Instance.PointAtNearestPoint(UsedSpawnerPoints[0]);
        }
    }

    public void GenerateHomeLocations()
    {
        List<Vector3> usedSpawnPoints = new List<Vector3>();
        
        // create number of homes + generate locations for each
        for (int i = 0; i < homesCount; i++)
        {
            Vector3 generatedLocation = new Vector3();

            bool isPositionOk = false;
            var min = AreaMinX;
            var max = AreaMaxX;

            while (isPositionOk == false)
            {
                generatedLocation = new Vector3(Player.Instance.GetHomePositionRange().x + Random.Range(min, max), 10, Player.Instance.GetHomePositionRange().z + Random.Range(AreaMinZ, AreaMaxZ));

                isPositionOk = true;
                foreach (Vector3 k in SpawnerPoint)
                {
                    if (Vector3.Distance(generatedLocation, k) < AcceptableSpawnDistance)
                    {
                        isPositionOk = false;
                        max += max / 20;
                        min += min / 20;
                    }
                }
            }

            if (isPositionOk)
            {
                SpawnerPoint[i] = generatedLocation;
                HomeGos[i].SetActive(true);
                HomeGos[i].transform.position = SpawnerPoint[i];

                UsedSpawnerPoints.Add(SpawnerPoint[i]);
            }
        }
        
    }

    private void OnDrawGizmosSelected()
    {
        //Gizmos.color = Color.black;
        //Gizmos.DrawSphere(SpawnerPoint[0], 0.8f);
        //Gizmos.color = Color.yellow;
        //Gizmos.DrawSphere(SpawnerPoint[1], 0.8f);
        //Gizmos.color = Color.green;
        //Gizmos.DrawSphere(SpawnerPoint[2], 0.8f);
        //Gizmos.color = Color.red;
        //Gizmos.DrawSphere(SpawnerPoint[3], 0.8f);

        //Gizmos.color = Color.white;
        //foreach(Vector3 v in SpawnerPoint)
        //{
        //    Gizmos.DrawSphere(v, 0.3f);
        //}
    }
}
