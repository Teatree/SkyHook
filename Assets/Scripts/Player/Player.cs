﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : SceneSingleton<Player> {
    public int CoinsAmountTotal;
    public float OrbitDistance = 5.0f;
    public float IntitialSpeed = 0.5f;

    public Vector3 GetPosition()
    {
        return transform.position;
    }
}

