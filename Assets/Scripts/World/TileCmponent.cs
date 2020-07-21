using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileCmponent : MonoBehaviour
{
    public int indexX;
    public int indexZ;
  
    public string getCoordinates()
    {
        return "" + indexX + indexZ;
    }

    public void setCoordinates (int x, int z)
    {
        indexX = x;
        indexZ = z;
    }
}
