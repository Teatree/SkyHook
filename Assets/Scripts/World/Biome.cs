using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class Biome 
{
    public string name; 
    public float waterLevel;
    public float heightMultiplier; //Used only for the initial map generation
    public AnimationCurve heightCurve; //Used only for the initial map generation
    public List<TerrainType> terrainTypes;

    public List<TerrainType> getTerrainTypes ()
    {
        return terrainTypes.OrderBy(e => e.height).ToList();
    }
}
