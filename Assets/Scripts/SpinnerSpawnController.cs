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

    [Header("Clue Gathering")]
    bool DoHaveClue;
    float clueAppearCounter;
    float clueLifeTime;
    float clueBreakTime;
    float currentNumberOfClues;
    Vector3 directionPositionSaved;
    

    void Start()
    {
        SpawnerPoint = new Vector3[4]; 

        locations.Add(hookables[0].transform.position);
        usedLocations.Add(hookables[0].transform.position);

        GenerateLocationsInitial(AreaMinX, AreaMaxX, AreaMinZ, AreaMaxZ, AcceptableSpawnDistance);

        clueBreakTime = SessionController.Instance.BreakTimeBeforeNextClue;
        InstantiateSpinners();
    }

    private void Update()
    {
        GenerateLocationsOngoing();

        MoveSpinnersAhead();

        if (clueBreakTime >= SessionController.Instance.BreakTimeBeforeNextClue)
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
                PlaceRadarParticle(directionPositionSaved);
                ItemsManager.Instance.SpawnItem(directionPositionSaved);

                directionPositionSaved = new Vector3();
            }
        }
    }

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

    public GameObject IdentifyPotentialClue(Vector3 lookPoint)
    {
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
        GameObject chosenClueGuy = IdentifyPotentialClue(SpawnerPoint[4]);
        if (chosenClueGuy != null)
        {
            chosenClueGuy.GetComponent<Hookable>().MakeClue();
            chosenClueGuy.gameObject.name = "Clue";
            hookables.Remove(chosenClueGuy);

            DoHaveClue = true;
        }
    }

    public void ClueKilled()
    {
        DoHaveClue = false;

        //Debug.Log("Resettings Clues -- beacuse died ");
        //currentNumberOfClues = 0;
    }

    public void PlaceArrowAndPoint(Vector3 hookbleLocation, Vector3 nextClueArea)
    {
        // Activate the Arrow and Point at the supplied location
        ArrowGo.SetActive(true);

        ArrowGo.transform.position = hookbleLocation;
        ArrowGo.transform.position = new Vector3(ArrowGo.transform.position.x, ArrowGo.transform.position.y + 4, ArrowGo.transform.position.z);
        ArrowGo.transform.LookAt(nextClueArea);
        ArrowGo.transform.eulerAngles = new Vector3(0, ArrowGo.transform.eulerAngles.y, ArrowGo.transform.eulerAngles.z);

        directionPositionSaved = nextClueArea;

        currentNumberOfClues++;
        if(currentNumberOfClues == SessionController.Instance.NumberOfCluesInitial)
        {
            Debug.Log("Break Starts");
            clueBreakTime = 0;
            currentNumberOfClues = 0;
        }
    }

    public void PlaceRadarParticle(Vector3 location)
    {
        RadarParticlePref.SetActive(true);
        RadarParticlePref.transform.position = location;
        RadarParticlePref.GetComponent<ParticleSystem>().Play();
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
