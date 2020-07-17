using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using UnityEngine.Jobs;
using Unity.Collections;

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

        if (CameraPositionGiver.Instance.ReachedForwardEdge)
        {
            this.transform.Translate(0, 0, 3);
        }
        else if (CameraPositionGiver.Instance.ReachedBEhindEdge)
        {
            this.transform.Translate(0, 0, -3);
        }
        else if (CameraPositionGiver.Instance.ReachedLeftEdge)
        {
            this.transform.Translate(-3, 0, 0);
        }
        else if (CameraPositionGiver.Instance.ReachedRightEdge)
        {
            this.transform.Translate(3, 0, 0);
        }

        #region Controls
        //this.transform.Translate(300, 0, 0);

        //this.transform.position = Vector3.Lerp(transform.position, new Vector3(500, this.transform.position.y, this.transform.position.z), 0.002f);

        //if (Input.GetKey("up"))
        //{
        //    this.transform.Translate(0, 0, 2);
        //}
        //else if (Input.GetKey("down"))
        //{
        //    this.transform.Translate(0, 0, -2);
        //}
        //else if (Input.GetKey("left")) 
        //{
        //    this.transform.Translate(-2, 0, 0);
        //}
        //else if (Input.GetKey("right"))
        //{
        //    this.transform.Translate(2, 0, 0);
        //}
        #endregion
    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y, transform.position.z + height * 3), 0.4f);
    }

    private void updatePositionOnMove()
    {
        int xOffset = (int)(this.transform.position.x - width / 2f);
        int zOffset = (int)(this.transform.position.z - width / 2f);

        var handles = new NativeArray<JobHandle>(1, Allocator.Temp);
        handles[0]= cubePosJobHandle;
      
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

        JobHandle.CompleteAll(handles);
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
            cub.transform.position = new Vector3(x, 0, cub.transform.localScale.z * ((i - x * height * layers) / layers));
            cubes[i] = cub;
            cubesTrans[i] = cub.transform;

        }
            cubeTransformAccess = new TransformAccessArray(cubesTrans);
            return cubes;
        }
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

    public void Execute(int i, TransformAccess transform)
    {
        int x = i / (width * layers);
        int z = (i - x * height * layers) / layers;
        int yOffset = (i - x * height * layers - z * layers);


        transform.position = new Vector3(
            transform.localScale.x * (x + xoffset),
            getPerlinHeight(x + xoffset, z + zoffset) + yOffset - 25,
            transform.localScale.z * (z + zoffset));
    }

    private float getPerlinHeight(float posX, float posZ)
    {


        float height = (Mathf.PerlinNoise(posX * smooth, posZ * smooth * 2) * heightMult +
                        Mathf.PerlinNoise(posX * smooth, posZ * smooth * 2) * heightMult) / 2f;
        return height * 10;
    }

    
}
