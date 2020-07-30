using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseMapGeneration : SceneSingleton<NoiseMapGeneration>
{

    public float[,] GenerateNoiseMap(int mapDepth, int mapWidth, float offsetX, float offsetZ)
    {
        float[,] noiseMap = new float[mapDepth, mapWidth];
        float amplitude, frequency;

        for (int zIndex = 0; zIndex < mapDepth; zIndex++)
        {
            for (int xIndex = 0; xIndex < mapWidth; xIndex++)
            {
                float noise = 0f;
                amplitude = 1;
                frequency = 1;

                float sampleX = (xIndex + offsetX) / TerrainGenerator.Instance.noiseScale;
                float sampleZ = (zIndex + offsetZ) / TerrainGenerator.Instance.noiseScale;

                for (int i = 0; i < TerrainGenerator.Instance.numOfWaves; i++)
                {

                    float _sampleX = (sampleX * frequency + TerrainGenerator.Instance.waveOffsets[i].x) ;
                    float _sampleZ = (sampleZ * frequency + TerrainGenerator.Instance.waveOffsets[i].x);

                    noise += amplitude * Mathf.PerlinNoise(_sampleX, _sampleZ);
                    amplitude *= TerrainGenerator.Instance.persistence;
                    frequency *= TerrainGenerator.Instance.lacunarity;
                }
                noise /= TerrainGenerator.Instance.maxPossibleHeight;
               
                noiseMap[zIndex, xIndex] = noise;
            }
        }
        return noiseMap;
    }

    public float GenerateNewWaves(float offsetX, float offsetZ)
    {
        float amplitude = 1;
        TerrainGenerator.Instance.waveOffsets = new Vector2[TerrainGenerator.Instance.numOfWaves];
        System.Random prng = new System.Random(TerrainGenerator.Instance.seed);
        float maxPossibleHeight = 0;
        for (int i = 0; i < TerrainGenerator.Instance.numOfWaves; i++)
        {
            float _offsetX = prng.Next(-100000, 100000) + offsetX;
            float _offsetY = prng.Next(-100000, 100000) - offsetZ;
            TerrainGenerator.Instance.waveOffsets[i] = new Vector2(_offsetX, _offsetY);
            maxPossibleHeight += amplitude;
            amplitude *= TerrainGenerator.Instance.persistence;
        }
        TerrainGenerator.Instance.maxPossibleHeight = maxPossibleHeight;
        return maxPossibleHeight;
    }
}