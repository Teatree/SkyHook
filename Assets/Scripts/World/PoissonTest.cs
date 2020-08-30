using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoissonTest : MonoBehaviour
{

    public float radius = 1;
    public Vector2 regionSize; 
    public int rejectAfterAttempts;
    public float displayRadius;

    private List<Vector2> points;

    void OnValidate()
    {
        points = PoissonDiscSampling.GeneratePoints(radius, regionSize, rejectAfterAttempts);
    }

   

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(regionSize / 2, regionSize); 
        if (points != null)
        {
            foreach (var v in points)
            {
                Gizmos.DrawSphere(v, displayRadius);
            }
        }
    }
}

