﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
    [SerializeField]
    private int mapWidthInTiles, mapDepthInTiles;

    [SerializeField]
    private GameObject tilePrefab;

    [SerializeField]
    private List<GameObject> tilesList;
    private Vector3 tileSize;

    [SerializeField]
    public Wave[] waves;


    public Dictionary<int, int> maxZPerRow;
    public Dictionary<int, int> minZPerRow; 
    public List<DicStruct> maxZPerRowE;
    public List<DicStruct> minZPerRowE;
    public int maxX;
    public int minX;


    void Start()
    {
        waves[0].seed = UnityEngine.Random.Range(1, 900000);
        waves[1].seed = UnityEngine.Random.Range(1, 900000);
        waves[2].seed = UnityEngine.Random.Range(1, 900000);

        tilesList = new List<GameObject>();
        maxZPerRow = new Dictionary<int, int>();
        minZPerRow = new Dictionary<int, int>();

        tileSize = tilePrefab.GetComponent<MeshRenderer>().bounds.size;

        GenerateMap(0, 0, mapWidthInTiles, mapDepthInTiles);
    }

    private void Update()
    {
        showDictionariesInEditor();
        RaycastHit hit;
        if (Physics.Raycast(PlayerBehaviour.Instance.transform.position, Vector3.down, out hit, 100))
        {
            //Debug.Log(">>>>> " + hit.collider.gameObject.GetComponent<TileCmponent>().indexX + " : " + hit.collider.gameObject.GetComponent<TileCmponent>().indexX);
            checkAndExtendMap(hit.collider.gameObject);
        }
    }

    private void checkAndExtendMap(GameObject currentTile)
    {
        TileCmponent tc = currentTile.GetComponent<TileCmponent>();

        if (maxZPerRow[tc.indexX] == tc.indexZ || minZPerRow[tc.indexX] == tc.indexZ
            || tc.indexX == maxX || tc.indexX == minX)
        {
            ExtendMap(tc.indexX, tc.indexZ, currentTile);
        }

    }


    private Dictionary<string, GameObject> getExistingNeighbours(int currX, int currZ)
    {
        Dictionary<string, GameObject> res = new Dictionary<string, GameObject>();
        foreach (GameObject t in tilesList)
        {
            TileCmponent tc = t.GetComponent<TileCmponent>();

            if (tc.indexX == currX || tc.indexX == currX + 1 || tc.indexX == currX - 1 ||
            tc.indexZ == currZ || tc.indexZ == currZ + 1 || tc.indexZ == currZ - 1)
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


    private void ExtendMap(int indexX, int indexZ, GameObject currentTile)
    {

        Dictionary<string, GameObject> neighbours = getExistingNeighbours(indexX, indexZ);

        Transform currT = currentTile.GetComponent<Transform>();
        if (!neighbours.ContainsKey("" + (indexX + 1) + (indexZ - 1)))
        {
            Vector3 tilePosition = new Vector3(currT.position.x + tileSize.x,
                                                    currT.position.y,
                                                    currT.position.z - tileSize.z);
            InstantiateTile(indexX + 1, indexZ - 1, tilePosition);
        }

        if (!neighbours.ContainsKey("" + (indexX + 1) + (indexZ)))
        {
            Vector3 tilePosition = new Vector3(currT.position.x + tileSize.x,
                                                 currT.position.y,
                                                 currT.position.z);
            InstantiateTile(indexX + 1, indexZ, tilePosition);
        }

        if (!neighbours.ContainsKey("" + (indexX + 1) + (indexZ + 1)))
        {
            Vector3 tilePosition = new Vector3(currT.position.x + tileSize.x,
                                                 currT.position.y,
                                                 currT.position.z + tileSize.z);
            InstantiateTile(indexX + 1, indexZ + 1, tilePosition);
        }
        if (!neighbours.ContainsKey("" + (indexX) + (indexZ - 1)))
        {
            Vector3 tilePosition = new Vector3(currT.position.x,
                                                 currT.position.y,
                                                 currT.position.z - tileSize.z);
            InstantiateTile(indexX, indexZ - 1, tilePosition);
        }
        if (!neighbours.ContainsKey("" + (indexX) + (indexZ + 1)))
        {
            Vector3 tilePosition = new Vector3(currT.position.x,
                                                   currT.position.y,
                                                   currT.position.z + tileSize.z);
            InstantiateTile(indexX, indexZ + 1, tilePosition);
        }
        if (!neighbours.ContainsKey("" + (indexX - 1) + (indexZ - 1)))
        {
            Vector3 tilePosition = new Vector3(currT.position.x - tileSize.x,
                                                currT.position.y,
                                                currT.position.z - tileSize.z);
            InstantiateTile(indexX - 1, indexZ - 1, tilePosition);
        }
        if (!neighbours.ContainsKey("" + (indexX - 1) + (indexZ)))
        {
            Vector3 tilePosition = new Vector3(currT.position.x - tileSize.x,
                                                 currT.position.y,
                                                 currT.position.z);
            InstantiateTile(indexX - 1, indexZ, tilePosition);
        }

        if (!neighbours.ContainsKey("" + (indexX - 1) + (indexZ + 1)))
        {
            Vector3 tilePosition22 = new Vector3(currT.position.x - tileSize.x,
                                                 currT.position.y,
                                                 currT.position.z + tileSize.z);
            InstantiateTile(indexX - 1, indexZ + 1, tilePosition22);
        }
    }

    private void InstantiateTile(int indexX, int indexZ, Vector3 tilePosition22)
    {
        GameObject t = Instantiate(tilePrefab, tilePosition22, Quaternion.identity) as GameObject;
        t.GetComponent<TileCmponent>().setCoordinates(indexX, indexZ);
        tilesList.Add(t);
        updateMinMaxCoordinates(t.GetComponent<TileCmponent>());
    }

    void GenerateMap(float xOffset, float zOffset, int mapWidth, int mapDepth)
    {
        //Debug.Log(">>>>> offset " + xOffset + " > " + zOffset);
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
                tilesList.Add(tile);
            }
        }
    }

    private void showDictionariesInEditor ()
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
}
