using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    [SerializeField]
    private int mapWidthInTiles, mapDepthInTiles;

    [SerializeField]
    private GameObject tilePrefab;

    private GameObject[,] tiles; 

    void Start()
    {
        tiles = new GameObject[mapWidthInTiles, mapDepthInTiles];

        GenerateMap(0, 0, mapWidthInTiles, mapDepthInTiles);
    }

    private void Update()
    {
     
        if ((Input.GetKey("up")))
        {
            Debug.Log("UP!");
            GenerateMap(tiles[0,0].transform.position.x, 
                tiles[0,0].transform.position.z - tiles[0, 0].transform.localScale.z
                , 2, 2);
        }


    }
    void GenerateMap(float xOffset, float zOffset, int mapWidth, int mapDepth)
    {
        Debug.Log(">>>>> offset " + xOffset + " > " + zOffset);
        // get the tile dimensions from the tile Prefab
        Vector3 tileSize = tilePrefab.GetComponent<MeshRenderer>().bounds.size;
        int tileWidth = (int)tileSize.x;
        int tileDepth = (int)tileSize.z;

        // for each Tile, instantiate a Tile in the correct position
        for (int xTileIndex = 0; xTileIndex < mapWidth; xTileIndex++)
        {
            for (int zTileIndex = 0; zTileIndex < mapDepth; zTileIndex++)
            {
                // calculate the tile position based on the X and Z indices
                float tileX = (this.gameObject.transform.position.x + xTileIndex * tileWidth) + xOffset * transform.localScale.x;
                float tileZ = (this.gameObject.transform.position.z + zTileIndex * tileDepth) + zOffset * transform.localScale.z;
                Vector3 tilePosition = new Vector3(tileX,
                this.gameObject.transform.position.y,
                tileZ);
                // instantiate a new Tile
                GameObject tile = Instantiate(tilePrefab, tilePosition, Quaternion.identity) as GameObject;
                tiles[xTileIndex, zTileIndex] = tile;
                Debug.Log(">>>> instantiate > " + tile.transform.position);
            }
        }
    }


}
