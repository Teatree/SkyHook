using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinnerSpawnController : SceneSingleton<SpinnerSpawnController>
{
    public GameObject hookablePref;
    public GameObject ArrowGo;
    public GameObject RadarParticlePref;
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

        MoveSpinnersAhead();
    }

    #region Hokable Gen
    public void GenerateLocationsOngoing()
    {
        // Generating locations on the fly, smartly
        SpawnerPoint = PlayerBehaviour.Instance.GetPointInDirectionFacing();

        //hookables[2].transform.position = new Vector3(SpawnerPoint.x + Random.Range(-8, 8), SpawnerPoint.y, SpawnerPoint.z + Random.Range(-8, 8));

        SpawnerPoint[0] = transform.TransformPoint(SpawnerPoint[0]);
        SpawnerPoint[1] = transform.TransformPoint(SpawnerPoint[1]);
        SpawnerPoint[2] = transform.TransformPoint(SpawnerPoint[2]);
        SpawnerPoint[3] = transform.TransformPoint(SpawnerPoint[3]);
        // first check if you can spawn at nose
        GenerateLocationsInitial(SpawnerPoint[0].x, SpawnerPoint[3].x, SpawnerPoint[1].z, SpawnerPoint[2].z, AcceptableSpawnDistance);
        //Debug.Log("SpawnerPoint 2: " + SpawnerPoint[0] + " SpawnerPoint 1: " + SpawnerPoint[3] + " SpawnerPoint 2: " + SpawnerPoint[1] + " SpawnerPoint 3: " + SpawnerPoint[2]);

        // when yes, generate a field of points
    }

    public void GenerateLocationsInitial(float areaMinX, float areaMaxX, float areaMinZ, float areaMaxZ, int acceptableSpawnDistance)
    {
        //Debug.Log("Generating Hookable Positions");
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

    public void ForceMoveSpinner(GameObject go)
    {
        List<Vector3> toBeInstantiated = new List<Vector3>();
        foreach (Vector3 v in locations)
        {
            if (Vector3.Distance(PlayerBehaviour.Instance.GetPosition(), v) < acceptableInstantiationDistance && usedLocations.Contains(v) == false)
            {
                toBeInstantiated.Add(v);
            }
        }

        go.GetComponent<Hookable>().minPush = SessionController.Instance.GetCurrentHookableMinPush();
        go.GetComponent<Hookable>().maxPush = SessionController.Instance.GetCurrentHookableMaxPush();

        for (int i = 0; i < toBeInstantiated.Count; i++)
        {
            usedLocations.Remove(go.transform.position);

            go.transform.position = toBeInstantiated[i];
            go.GetComponent<Hookable>().spinnerSpinSpeed += 50;

            usedLocations.Add(toBeInstantiated[i]);
        }
    }

    public void MoveSpinnersAhead()
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
                    //usedLocations.Remove(g.transform.position);
                    g.GetComponent<Hookable>().minPush = SessionController.Instance.GetCurrentHookableMinPush();
                    g.GetComponent<Hookable>().maxPush = SessionController.Instance.GetCurrentHookableMaxPush();
                }
            }

            for (int i = 0; i < toBeInstantiated.Count; i++)
            {
                if (i < toBeRelocated.Count)
                {
                    usedLocations.Remove(toBeRelocated[i].transform.position);

                    toBeRelocated[i].transform.position = toBeInstantiated[i];
                    toBeRelocated[i].GetComponent<Hookable>().spinnerSpinSpeed += 50;

                    usedLocations.Add(toBeInstantiated[i]);
                } else
                {
                    GameObject t = Instantiate(hookablePref, transform);
                    t.transform.position = toBeInstantiated[i];
                    t.GetComponent<Hookable>().minPush = SessionController.Instance.GetCurrentHookableMinPush();
                    t.GetComponent<Hookable>().maxPush = SessionController.Instance.GetCurrentHookableMaxPush();

                    t.GetComponent<Hookable>().spinnerSpinSpeed = 15;

                    hookables.Add(t);
                    usedLocations.Add(toBeInstantiated[i]);
                }
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
    #endregion

    public void PlaceArrowAndPoint(Vector3 hookbleLocation, Vector3 nextClueArea)
    {
        // Activate the Arrow and Point at the supplied location
        ArrowGo.SetActive(true);

        ArrowGo.transform.position = hookbleLocation;
        ArrowGo.transform.position = new Vector3(ArrowGo.transform.position.x, ArrowGo.transform.position.y + 4, ArrowGo.transform.position.z);
        ArrowGo.transform.LookAt(nextClueArea);
        ArrowGo.transform.eulerAngles = new Vector3(0, ArrowGo.transform.eulerAngles.y, ArrowGo.transform.eulerAngles.z);

        ItemsManager.Instance.SaveDirection(nextClueArea);
    }

    public void PlaceRadarParticle(Vector3 location)
    {
        RadarParticlePref.SetActive(true);
        RadarParticlePref.transform.position = location;
        RadarParticlePref.GetComponent<ParticleSystem>().Play();
    }

    public List<GameObject> GetHookables()
    {
        return hookables;
    }

    public void RemoveFromHookables(GameObject go)
    {
        hookables.Remove(go);
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
        //foreach(Vector3 v in locations)
        //{
        //    Gizmos.DrawSphere(v, 0.3f);
        //}
    }
}
