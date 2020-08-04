using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGeneration : MonoBehaviour
{

    [SerializeField]
    public MeshRenderer tileRenderer;

    [SerializeField]
    public MeshFilter meshFilter;

    public List<TerrainType> mainTerrainTypes;
    public List<TerrainType> secondaryTerrainTypes;


    void GenerateTile()
    {
        Vector3[] meshVertices = this.meshFilter.mesh.vertices;
        int tileDepth = (int)Mathf.Sqrt(meshVertices.Length);
        int tileWidth = tileDepth;

        float offsetX = -this.gameObject.transform.position.x / transform.localScale.x;
        float offsetZ = -this.gameObject.transform.position.z / transform.localScale.z;

        float[,] heightMap = TerrainGenerator.Instance.noiseMapGeneration.GenerateNoiseMap(tileDepth, tileWidth, offsetX, offsetZ);
        heightMap = terraceOrSmt(heightMap);
        Texture2D tileTexture = BuildTexture(heightMap);
        this.tileRenderer.material.mainTexture = tileTexture;

        UpdateMeshVertices(heightMap);
    }

    public Texture2D BuildTexture(float[,] heightMap)
    {
        int tileDepth = heightMap.GetLength(0);
        int tileWidth = heightMap.GetLength(1);

        Color[] colorMap = new Color[tileDepth * tileWidth];
        for (int zIndex = 0; zIndex < tileDepth; zIndex++)
        {
            for (int xIndex = 0; xIndex < tileWidth; xIndex++)
            {
                int colorIndex = zIndex * tileWidth + xIndex;
                float height = heightMap[zIndex, xIndex];
                TerrainType terrainType = ChooseTerrainType(height);
                colorMap[colorIndex] = terrainType.color;
            }
        }

        Texture2D tileTexture = new Texture2D(tileWidth, tileDepth);
        tileTexture.wrapMode = TextureWrapMode.Clamp;
        // tileTexture.filterMode = FilterMode.Point;
        tileTexture.SetPixels(colorMap);
        tileTexture.Apply();

        return tileTexture;
    }
    
    private static float[,] terraceOrSmt(float[,] heightMap)
    {
        int tileDepth = heightMap.GetLength(0);
        int tileWidth = heightMap.GetLength(1);

        for (int zIndex = 0; zIndex < tileDepth; zIndex++)
        {
            for (int xIndex = 0; xIndex < tileWidth; xIndex++)
            {
                float height = heightMap[zIndex, xIndex];

                //  float terraceHeight = 2 * (0.5f - Mathf.Abs(0.5f - heightMap[zIndex, xIndex]));
                //  terraceHeight *= TerrainGenerator.Instance.currentHeightMultiplier;
                float terraceHeight = Mathf.Pow(height, 2.1f);
                
                terraceHeight = (Mathf.Round(terraceHeight * TerrainGenerator.Instance.terraces)) /
                                TerrainGenerator.Instance.terraces;
                //Debug.Log("> height > " + terraceHeight);
                heightMap[zIndex, xIndex] = terraceHeight;
            }
        }

        return heightMap;
    }
    
    private void UpdateMeshVertices(float[,] heightMap)
    {
        int tileDepth = heightMap.GetLength(0);
        int tileWidth = heightMap.GetLength(1);

        Vector3[] meshVertices = this.meshFilter.mesh.vertices;

        int vertexIndex = 0;
        for (int zIndex = 0; zIndex < tileDepth; zIndex++)
        {
            for (int xIndex = 0; xIndex < tileWidth; xIndex++)
            {
                float height = heightMap[zIndex, xIndex];

                Vector3 vertex = meshVertices[vertexIndex];
                meshVertices[vertexIndex] = new Vector3(vertex.x, TerrainGenerator.Instance.currentHeightCurve.Evaluate(height) * TerrainGenerator.Instance.currentHeightMultiplier, vertex.z);

                vertexIndex++;
            }
        }

        this.meshFilter.mesh.vertices = meshVertices;
        this.meshFilter.mesh.RecalculateBounds();
        this.meshFilter.mesh.RecalculateNormals();
    }


    TerrainType ChooseTerrainType(float height)
    {
        if (secondaryTerrainTypes == null || secondaryTerrainTypes.Count == 0)
        {
            foreach (TerrainType terrainType in mainTerrainTypes)
            {
                if (height < terrainType.height)
                {
                    return terrainType;
                }
            }
        }
        // blend 
        if (secondaryTerrainTypes != null && secondaryTerrainTypes.Count > 0)
        {
            int r = Random.Range(0, 2);
            if (r == 0)
            {
                foreach (TerrainType terrainType in mainTerrainTypes)
                {
                    if (height < terrainType.height)
                    {
                        return terrainType;
                    }
                }
            }
            else
            {
                foreach (TerrainType terrainType in secondaryTerrainTypes)
                {
                    if (height < terrainType.height)
                    {

                        return terrainType;
                    }
                }
            }
        }
        return mainTerrainTypes[mainTerrainTypes.Count - 1];
    }

    void Start()
    {
        GenerateTile();
    }
}


