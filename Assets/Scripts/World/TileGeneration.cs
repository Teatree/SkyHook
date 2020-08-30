using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TileGeneration : MonoBehaviour
{
    [SerializeField] public MeshRenderer tileRenderer;

    [SerializeField] public MeshFilter meshFilter;

    public List<TerrainType> mainTerrainTypes;

    private List<GameObject> myProps = new List<GameObject>();

    public float distanceBetweenProps = 2f;
    public int poissonRejectAfter = 3;
    public int propsOnTileAmount = 5;
    private List<Vector2> propsPositions;

    void GenerateTile()
    {
        Vector3[] meshVertices = this.meshFilter.mesh.vertices;
        int tileDepth = (int) Mathf.Sqrt(meshVertices.Length);
        int tileWidth = tileDepth;

        float offsetX = -gameObject.transform.position.x / transform.localScale.x;
        float offsetZ = -gameObject.transform.position.z / transform.localScale.z;

        float[,] heightMap =
            TerrainGenerator.Instance.noiseMapGeneration.GenerateNoiseMap(tileDepth, tileWidth, offsetX, offsetZ);
        // heightMap = terraceOrSmt(heightMap);
        Texture2D tileTexture = BuildTexture(heightMap);
        this.tileRenderer.material.mainTexture = tileTexture;
        UpdateMeshVertices(heightMap);
        addTrees();
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
                meshVertices[vertexIndex] = new Vector3(vertex.x,
                    TerrainGenerator.Instance.currentHeightCurve.Evaluate(height) *
                    TerrainGenerator.Instance.currentHeightMultiplier, vertex.z);
                vertexIndex++;
            }
        }

        this.meshFilter.mesh.vertices = meshVertices;
        this.meshFilter.mesh.RecalculateBounds();
        this.meshFilter.mesh.RecalculateNormals();

        DestroyImmediate(this.GetComponent<MeshCollider>()); // TODO: SOOOO NOT OPTIMAL, OMG
        gameObject.AddComponent<MeshCollider>();
    }

    private void addTrees()
    {
        propsPositions = PoissonDiscSampling.GeneratePoints(distanceBetweenProps,
            new Vector2(transform.localScale.x * 2, transform.localScale.z * 2), propsOnTileAmount, poissonRejectAfter);
        foreach (Vector2 treePos in propsPositions)
        {
            GameObject tree = ItemsPool.Instance.getItem();
            if (tree != null)
            {
                tree.transform.parent = this.transform;
                tree.transform.localPosition = new Vector3(treePos.x - transform.localScale.x, 3,
                    treePos.y - transform.localScale.y);
                tree.SetActive(true);
                myProps.Add(tree);
            }
        }
    }

    TerrainType ChooseTerrainType(float height)
    {
        foreach (TerrainType terrainType in mainTerrainTypes)
        {
            if (height < terrainType.height)
            {
                return terrainType;
            }
        }

        return mainTerrainTypes[mainTerrainTypes.Count - 1];
    }

    void Start()
    {
        GenerateTile();
    }

    public void DeactivateItems()
    {
        foreach (var t in myProps)
        {
            t.SetActive(false);
        }
    }

    // private void OnDrawGizmos()
    // {
    //     Gizmos.DrawWireCube(transform.localScale / 2, transform.localScale); 
    //     if (treePositions != null)
    //     {
    //         foreach (var v in treePositions)
    //         {
    //             Gizmos.DrawSphere(v, 0.1f);
    //         }
    //     }
    // }
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
}