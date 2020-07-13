using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldBuilder : MonoBehaviour
{
    public GameObject hookable;
    public int areaMinX = -300;
    public int areaMaxX = 300;
    public int areaMinY = -200;
    public int areaMaxY = 200;

    public int acceptableSpawnDistance;
    public int hookableCount;

    public List<GameObject> hookables = new List<GameObject>();

    void Start()
    {
        for (int i = 0; i < hookableCount; i++)
        {
            bool isPositionOk = false;
            Vector3 temp = new Vector3();
            int attempts = 5;

            while (isPositionOk == false && attempts >= 0)
            {
                attempts--;
                temp = new Vector3(Random.Range(areaMinX, areaMaxX), Random.Range(areaMinY, areaMaxY), 0);

                isPositionOk = true;
                foreach (GameObject h in hookables)
                {
                    if (Vector3.Distance(temp, h.transform.position) < acceptableSpawnDistance)
                    {
                        isPositionOk = false;
                    }
                }
            }

            if (isPositionOk)
            {
                GameObject t = Instantiate(hookable, transform);
                t.transform.position = temp;

                hookables.Add(t);
            }
            else
            {
                Debug.Log("omg I failed to create a hookable! so Sorry");
            }
        }
    }
}
