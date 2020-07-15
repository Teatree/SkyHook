using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using UnityEngine.Jobs;

public class CubeMover : MonoBehaviour
{

    int totalCubes;

    [Header("Grid stuff")]
    public int width = 42;
    public int height = 42;
    public int layers = 3;

    [Header("Perlin stuff")]
    public float smooth = 0.03f;
    public float heightMult = 5f;


    public GameObject prefabToSpawn;
    public GameObject[] cubes;


    //Jobs
    Transform[] cubesTrans;
    TransformAccessArray cubeTransformAccess;
    JobHandle cubePosJobHandle;
    PositionUpdateJob cubejob; 



    void Awake()
    {
        totalCubes = width * height;
        cubes = new GameObject[totalCubes];
        cubesTrans = new Transform[totalCubes];
    }

    private void Start()
    {
        cubes = getCubes(totalCubes);
    }


    // Update is called once per frame
    void Update()
    {
        updatePositionOnMove();

        if (Input.GetKey("up"))
        {
            this.transform.Translate(0, 0, 2);
        }
        else if (Input.GetKey("down"))
        {
            this.transform.Translate(0, 0, -2);
        }
        else if (Input.GetKey("left"))
        {
            this.transform.Translate(-2, 0, 0);
        }
        else if (Input.GetKey("right"))
        {
            this.transform.Translate(2, 0, 0);
        }
    }

    private void updatePositionOnMove()
    {
        int xOffset = (int)(this.transform.position.x - width / 2f);
        int zOffset = (int)(this.transform.position.z - width / 2f);

        PositionUpdateJob cubejob = new PositionUpdateJob
        {
            height = this.height,
            width = this.width,
            layers = this.layers,
            smooth = this.smooth,
            heightMult = this.heightMult,
            xoffset = xOffset,
            zoffset = zOffset

        };

        cubePosJobHandle = cubejob.Schedule(cubeTransformAccess, cubePosJobHandle);

        //for (int i = 0; i < totalCubes; i++)
        //{

        //    int x = i / (width * layers);
        //    int z = (i - x * height * layers) / layers;
        //    int yOffset = (i - x * height * layers - z * layers);

        //    cubes[i].transform.position = new Vector3(
        //        x + xOffset,
        //        getPerlinHeight(x + xOffset, z + zOffset) + yOffset,
        //        z + zOffset);
        //}
    }

    private float getPerlinHeight(float posX, float posZ)
    {
        float height = (Mathf.PerlinNoise(posX * smooth, posZ * smooth * 2) * heightMult +
                        Mathf.PerlinNoise(posX * smooth, posZ * smooth * 2) * heightMult) / 2f;
        return height * 10;
    }


    private GameObject[] getCubes(int totalCubes)
    {
        var cubes = new GameObject[totalCubes];
        for (int i = 0; i < totalCubes; i++)
        {
            var cub = GameObject.Instantiate(prefabToSpawn);
            int x = i / (width * layers);
            cub.transform.position = new Vector3(x, 0, (i - x * height * layers) / layers);
            cubes[i] = cub;
            cubesTrans[i] = cub.transform;

        }
        cubeTransformAccess = new TransformAccessArray(cubesTrans);
        return cubes;

    }

    struct PositionUpdateJob : IJobParallelForTransform
    {

        public int width;
        public int height;
        public int layers;

        public float smooth;
        public float heightMult;

        public float xoffset;
        public float zoffset;

        

        public void Execute (int i, TransformAccess transform)
        {
            //int xOffset = (int)(transform.position.x - width / 2f);
            //int zOffset = (int)(transform.position.z - width / 2f);

            int x = i / (width * layers);
            int z = (i - x * height * layers) / layers;
            int yOffset = (i - x * height * layers - z * layers);

       

            transform.position = new Vector3(
                x + xoffset,
                getPerlinHeight(x + xoffset, z + zoffset) + yOffset,
                z + zoffset);
        }

        private float getPerlinHeight(float posX, float posZ)
        {
     

            float height = (Mathf.PerlinNoise(posX * smooth, posZ * smooth * 2) * heightMult +
                            Mathf.PerlinNoise(posX * smooth, posZ * smooth * 2) * heightMult) / 2f;
            return height * 10;
        }
    }
}
