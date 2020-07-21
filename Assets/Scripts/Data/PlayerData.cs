using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData : SceneSingleton<PlayerData>
{
    public int CoinsAmountTotal;
    public float OrbitDistance = 5.0f;
    public float IntitialSpeed = 0.5f;
}
