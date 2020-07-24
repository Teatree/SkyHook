using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static PlayerBehaviour;

[Serializable]
public struct DicStruct
{
    public int x;
    public int z;

    public DicStruct(int x, int z)
    {
        this.x = x;
        this.z = z;
    }
}

public class TerrainGenerator : SceneSingleton<TerrainGenerator>
{
    public int mapWidthInTiles, mapDepthInTiles;

    public GameObject tilePrefab;

    public List<GameObject> tilesList;
    public Vector3 tileSize;

    public List<Wave> waves;


    // public TerrainType[] terrainTypes;
    public List<Biome> biomes;
    public float currentHeightMultiplier;
    public AnimationCurve currentHeightCurve;

    public Dictionary<int, int> maxZPerRow;
    public Dictionary<int, int> minZPerRow;
    public List<DicStruct> maxZPerRowE;
    public List<DicStruct> minZPerRowE;
    public int maxX;
    public int minX;

    private int raycastLayerMask;



    void Start()
    {

        raycastLayerMask = LayerMask.GetMask("Terrain");
        // waves.ForEach(x => x.seed = UnityEngine.Random.Range(1, 900000));
        waves[0].seed = UnityEngine.Random.Range(5598, 6678);
        waves[1].seed = UnityEngine.Random.Range(9598, 13678);
        waves[2].seed = UnityEngine.Random.Range(6598, 7678);

        tilesList = new List<GameObject>();
        maxZPerRow = new Dictionary<int, int>();
        minZPerRow = new Dictionary<int, int>();

        tileSize = tilePrefab.GetComponent<MeshRenderer>().bounds.size;
        // biomes.ForEach(x => x.terrainTypes = new List<TerrainType>(terrainTypes));

        GenerateMap(0, 0, mapWidthInTiles, mapDepthInTiles);
    }

    private void Update()
    {
        if (Application.isEditor)
        {
            showDictionariesInEditor();
        }

        if (PlayerBehaviour.Instance.GetState() == PlayerState.launched)
        {
            RaycastHit hit;
            if (Physics.Raycast(PlayerBehaviour.Instance.transform.position, Vector3.down, out hit, 90, raycastLayerMask))
            {
                checkAndExtendMap(hit.collider.gameObject);
            }
        }

        if (Input.GetKeyDown("up"))
        {
            Biome b = biomes[UnityEngine.Random.Range(0, biomes.Count)];
            b.heightMultiplier = currentHeightMultiplier;
            ExtendMap(0, 0, tilesList[0], b);
        }
    }

    private void checkAndExtendMap(GameObject currentTile)
    {
        TileCmponent tc = currentTile.GetComponent<TileCmponent>();
        //   Debug.Log(">>>>> check tc > " + tc.indexX + "> " + tc.indexZ);
        if (maxZPerRow[tc.indexX] == tc.indexZ || minZPerRow[tc.indexX] == tc.indexZ
            || tc.indexX == maxX || tc.indexX == minX ||
            maxZPerRow[tc.indexX] == tc.indexZ + 1 || minZPerRow[tc.indexX] == tc.indexZ - 1
            || tc.indexX + 1 == maxX || tc.indexX - 1 == minX)
        {
            Biome b = biomes[UnityEngine.Random.Range(0, biomes.Count)];
            b.heightMultiplier = currentHeightMultiplier;
            ExtendMap(tc.indexX, tc.indexZ, currentTile, b);
        }

    }


    private Dictionary<string, GameObject> getExistingNeighbours(int currX, int currZ)
    {
        Dictionary<string, GameObject> res = new Dictionary<string, GameObject>();
        foreach (GameObject t in tilesList)
        {
            TileCmponent tc = t.GetComponent<TileCmponent>();

            if (tc.indexX == currX || tc.indexX == currX + 1 || tc.indexX == currX - 1 ||
                    tc.indexX == currX + 2 || tc.indexX == currX - 2 ||
                    tc.indexZ == currZ || tc.indexZ == currZ + 1 || tc.indexZ == currZ - 1 ||
                    tc.indexZ == currZ + 2 || tc.indexZ == currZ - 2)
            {
                res[tc.getCoordinates()] = t;
            }
        }
        return res;
    }

    private void updateMinMaxCoordinates(TileCmponent tc)
    {
        if (tc.indexX > maxX)
        {
            maxX = tc.indexX;
        }

        if (tc.indexX < minX)
        {
            minX = tc.indexX;
        }
        if (maxZPerRow.ContainsKey(tc.indexX))
        {
            if (maxZPerRow[tc.indexX] < tc.indexZ)
            {
                maxZPerRow[tc.indexX] = tc.indexZ;
            }
        }
        else
        {
            maxZPerRow[tc.indexX] = tc.indexZ;
        }

        if (minZPerRow.ContainsKey(tc.indexX))
        {
            if (minZPerRow[tc.indexX] > tc.indexZ)
            {
                minZPerRow[tc.indexX] = tc.indexZ;
            }
        }
        else
        {
            minZPerRow[tc.indexX] = tc.indexZ;
        }
    }

    private void InstantiateTile(int indexX, int indexZ, Vector3 tilePosition22, Biome b, Dictionary<string, GameObject> neighbours)
    {
        GameObject t = Instantiate(tilePrefab, tilePosition22, Quaternion.identity) as GameObject;
        t.GetComponent<TileCmponent>().setCoordinates(indexX, indexZ);
        t.GetComponent<TileCmponent>().biome = b;
        t.GetComponent<TileGeneration>().terrainTypes = b.getTerrainTypes();
        neighbours["" + indexX + indexZ] = t;
        tilesList.Add(t);

        updateMinMaxCoordinates(t.GetComponent<TileCmponent>());
    }

    void GenerateMap(float xOffset, float zOffset, int mapWidth, int mapDepth)
    {
        Biome b = biomes[UnityEngine.Random.Range(0, biomes.Count)];
        this.currentHeightMultiplier = b.heightMultiplier;
        this.currentHeightCurve = b.heightCurve;

        int tileWidth = (int)tileSize.x;
        int tileDepth = (int)tileSize.z;

        minX = 0;
        maxX = mapWidth - 1;
        for (int xTileIndex = 0; xTileIndex < mapWidth; xTileIndex++)
        {
            minZPerRow[xTileIndex] = 0;
            maxZPerRow[xTileIndex] = mapDepth - 1;
            for (int zTileIndex = 0; zTileIndex < mapDepth; zTileIndex++)
            {
                float tileX = (this.gameObject.transform.position.x + xTileIndex * tileWidth) + xOffset * transform.localScale.x;
                float tileZ = (this.gameObject.transform.position.z + zTileIndex * tileDepth) + zOffset * transform.localScale.z;
                Vector3 tilePosition = new Vector3(tileX,
                this.gameObject.transform.position.y,
                tileZ);
                GameObject tile = Instantiate(tilePrefab, tilePosition, Quaternion.identity) as GameObject;
                tile.GetComponent<TileCmponent>().indexX = xTileIndex;
                tile.GetComponent<TileCmponent>().indexZ = zTileIndex;
                tile.GetComponent<TileGeneration>().terrainTypes = b.getTerrainTypes();
                tilesList.Add(tile);
            }
        }
    }

    private void showDictionariesInEditor()
    {
        maxZPerRowE = new List<DicStruct>();
        minZPerRowE = new List<DicStruct>();

        foreach (int x in maxZPerRow.Keys)
        {
            maxZPerRowE.Add(new DicStruct(x, maxZPerRow[x]));
        }
        foreach (int x in minZPerRow.Keys)
        {
            minZPerRowE.Add(new DicStruct(x, minZPerRow[x]));
        }

    }

    #region new tile positions
    private void ExtendMap(int indexX, int indexZ, GameObject currentTile, Biome b)
    {

        Dictionary<string, GameObject> neighbours = getExistingNeighbours(indexX, indexZ);

        Transform currT = currentTile.GetComponent<Transform>();
        if (!neighbours.ContainsKey("" + (indexX + 1) + (indexZ - 1)))
        {
            Vector3 tilePosition = new Vector3(currT.position.x + tileSize.x,
                                                    currT.position.y,
                                                    currT.position.z - tileSize.z);

            InstantiateTile(indexX + 1, indexZ - 1, tilePosition, b, neighbours);
        }

        if (!neighbours.ContainsKey("" + (indexX + 1) + (indexZ)))
        {
            Vector3 tilePosition = new Vector3(currT.position.x + tileSize.x,
                                                 currT.position.y,
                                                 currT.position.z);
            InstantiateTile(indexX + 1, indexZ, tilePosition, b, neighbours);
        }

        if (!neighbours.ContainsKey("" + (indexX + 1) + (indexZ + 1)))
        {
            Vector3 tilePosition = new Vector3(currT.position.x + tileSize.x,
                                                 currT.position.y,
                                                 currT.position.z + tileSize.z);
            InstantiateTile(indexX + 1, indexZ + 1, tilePosition, b, neighbours);
        }
        if (!neighbours.ContainsKey("" + (indexX) + (indexZ - 1)))
        {
            Vector3 tilePosition = new Vector3(currT.position.x,
                                                 currT.position.y,
                                                 currT.position.z - tileSize.z);
            InstantiateTile(indexX, indexZ - 1, tilePosition, b, neighbours);
        }
        if (!neighbours.ContainsKey("" + (indexX) + (indexZ + 1)))
        {
            Vector3 tilePosition = new Vector3(currT.position.x,
                                                   currT.position.y,
                                                   currT.position.z + tileSize.z);
            InstantiateTile(indexX, indexZ + 1, tilePosition, b, neighbours);
        }
        if (!neighbours.ContainsKey("" + (indexX - 1) + (indexZ - 1)))
        {
            Vector3 tilePosition = new Vector3(currT.position.x - tileSize.x,
                                                currT.position.y,
                                                currT.position.z - tileSize.z);
            InstantiateTile(indexX - 1, indexZ - 1, tilePosition, b, neighbours);
        }
        if (!neighbours.ContainsKey("" + (indexX - 1) + (indexZ)))
        {
            Vector3 tilePosition = new Vector3(currT.position.x - tileSize.x,
                                                 currT.position.y,
                                                 currT.position.z);
            InstantiateTile(indexX - 1, indexZ, tilePosition, b, neighbours);
        }

        if (!neighbours.ContainsKey("" + (indexX - 1) + (indexZ + 1)))
        {
            Vector3 tilePosition22 = new Vector3(currT.position.x - tileSize.x,
                                                 currT.position.y,
                                                 currT.position.z + tileSize.z);
            InstantiateTile(indexX - 1, indexZ + 1, tilePosition22, b, neighbours);
        }

        //second area

        if (!neighbours.ContainsKey("" + (indexX + 2) + (indexZ)))
        {
            Vector3 tilePosition = new Vector3(currT.position.x + 2 * tileSize.x,
                                                 currT.position.y,
                                                 currT.position.z);
            InstantiateTile(indexX + 2, indexZ, tilePosition, b, neighbours);
        }
        if (!neighbours.ContainsKey("" + (indexX + 2) + (indexZ - 1)))
        {
            Vector3 tilePosition = new Vector3(currT.position.x + 2 * tileSize.x,
                                                 currT.position.y,
                                                 currT.position.z - tileSize.z);
            InstantiateTile(indexX + 2, indexZ - 1, tilePosition, b, neighbours);
        }
        if (!neighbours.ContainsKey("" + (indexX + 2) + (indexZ + 1)))
        {
            Vector3 tilePosition = new Vector3(currT.position.x + 2 * tileSize.x,
                                                 currT.position.y,
                                                 currT.position.z + tileSize.z);
            InstantiateTile(indexX + 2, indexZ + 1, tilePosition, b, neighbours);
        }

        if (!neighbours.ContainsKey("" + (indexX - 1) + (indexZ - 2)))
        {
            Vector3 tilePosition = new Vector3(currT.position.x - tileSize.x,
                                                 currT.position.y,
                                                 currT.position.z - 2 * tileSize.z);
            InstantiateTile(indexX - 1, indexZ - 2, tilePosition, b, neighbours);
        }
        if (!neighbours.ContainsKey("" + (indexX + 1) + (indexZ - 2)))
        {
            Vector3 tilePosition = new Vector3(currT.position.x + tileSize.x,
                                                 currT.position.y,
                                                 currT.position.z - 2 * tileSize.z);
            InstantiateTile(indexX + 1, indexZ - 2, tilePosition, b, neighbours);
        }

        if (!neighbours.ContainsKey("" + (indexX) + (indexZ - 2)))
        {
            Vector3 tilePosition = new Vector3(currT.position.x,
                                                 currT.position.y,
                                                 currT.position.z - 2 * tileSize.z);
            InstantiateTile(indexX, indexZ - 2, tilePosition, b, neighbours);
        }

        ///
        if (!neighbours.ContainsKey("" + (indexX) + (indexZ + 2)))
        {
            Vector3 tilePosition = new Vector3(currT.position.x,
                                                   currT.position.y,
                                                   currT.position.z + 2 * tileSize.z);
            InstantiateTile(indexX, indexZ + 2, tilePosition, b, neighbours);
        }
        if (!neighbours.ContainsKey("" + (indexX - 1) + (indexZ + 2)))
        {
            Vector3 tilePosition = new Vector3(currT.position.x - tileSize.x,
                                                   currT.position.y,
                                                   currT.position.z + 2 * tileSize.z);
            InstantiateTile(indexX - 1, indexZ + 2, tilePosition, b, neighbours);
        }
        if (!neighbours.ContainsKey("" + (indexX + 1) + (indexZ + 2)))
        {
            Vector3 tilePosition = new Vector3(currT.position.x + tileSize.x,
                                                   currT.position.y,
                                                   currT.position.z + 2 * tileSize.z);
            InstantiateTile(indexX + 1, indexZ + 2, tilePosition, b, neighbours);
        }

        //
        if (!neighbours.ContainsKey("" + (indexX - 2) + (indexZ)))
        {
            Vector3 tilePosition = new Vector3(currT.position.x - 2 * tileSize.x,
                                                 currT.position.y,
                                                 currT.position.z);
            InstantiateTile(indexX - 2, indexZ, tilePosition, b, neighbours);
        }
        if (!neighbours.ContainsKey("" + (indexX - 2) + (indexZ - 1)))
        {
            Vector3 tilePosition = new Vector3(currT.position.x - 2 * tileSize.x,
                                                 currT.position.y,
                                                 currT.position.z - tileSize.z);
            InstantiateTile(indexX - 2, indexZ - 1, tilePosition, b, neighbours);
        }
        if (!neighbours.ContainsKey("" + (indexX - 2) + (indexZ + 1)))
        {
            Vector3 tilePosition = new Vector3(currT.position.x - 2 * tileSize.x,
                                                 currT.position.y,
                                                 currT.position.z + tileSize.z);
            InstantiateTile(indexX - 2, indexZ + 1, tilePosition, b, neighbours);
        }

        if (!neighbours.ContainsKey("" + (indexX - 2) + (indexZ - 2)))
        {
            Vector3 tilePosition = new Vector3(currT.position.x - 2 * tileSize.x,
                                                 currT.position.y,
                                                 currT.position.z - 2 * tileSize.z);
            InstantiateTile(indexX - 2, indexZ - 2, tilePosition, b, neighbours);
        }
        if (!neighbours.ContainsKey("" + (indexX - 2) + (indexZ + 2)))
        {
            Vector3 tilePosition = new Vector3(currT.position.x - 2 * tileSize.x,
                                                 currT.position.y,
                                                 currT.position.z + 2 * tileSize.z);
            InstantiateTile(indexX - 2, indexZ + 2, tilePosition, b, neighbours);
        }

        if (!neighbours.ContainsKey("" + (indexX + 2) + (indexZ + 2)))
        {
            Vector3 tilePosition = new Vector3(currT.position.x + 2 * tileSize.x,
                                                 currT.position.y,
                                                 currT.position.z + 2 * tileSize.z);
            InstantiateTile(indexX + 2, indexZ + 2, tilePosition, b, neighbours);
        }
        if (!neighbours.ContainsKey("" + (indexX + 2) + (indexZ - 2)))
        {
            Vector3 tilePosition = new Vector3(currT.position.x + 2 * tileSize.x,
                                                 currT.position.y,
                                                 currT.position.z - 2 * tileSize.z);
            InstantiateTile(indexX + 2, indexZ - 2, tilePosition, b, neighbours);
        }
    }
    #endregion
}
