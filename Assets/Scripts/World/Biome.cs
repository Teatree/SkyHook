using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class Biome 
{
    public string name;
    public Color waterColour; 
    public float waterLevelMin = -69f;
    public float waterLevelMax = -44;
    public float heightMultiplier; //Used only for the initial map generation
    public AnimationCurve heightCurve; //Used only for the initial map generation
    public float persistenceMin = 0.3f;
    public float persistenceMax = 0.5f;
    public float lacunarityMin = 3;
    public float lacunarityMax = 11;
    public int numOfWaves = 5;
    public float noiseScaleMin = 7;
    public float noiseScaleMax = 21;
    
    public List<TerrainType> terrainTypes;

    public List<TerrainType> getTerrainTypes ()
    {
        return terrainTypes.OrderBy(e => e.height).ToList();
    }
}
