using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : SceneSingleton<Player> {
    public static float CurrentSpeedIncrement = 1f;

    public int CoinsAmountTotal;
    public float OrbitDistance = 5.0f;
    public float IntitialSpeed = 0.5f;
    public bool targetable = true;

    public GameObject PickUpParticle;
    public Transform ShipTransform;
    public GameObject CompassArrow; //Temporary

    Vector3 spawnPosition;
    float counter;
    public float increaseSpeedTimer;

    public void Start()
    {
        spawnPosition = transform.position;
    }

    public void Update()
    {
        // timer increases speed
        counter += Time.deltaTime;
        if (counter > increaseSpeedTimer)
        {
            CurrentSpeedIncrement += 0.03f;
            counter = 0;
        }
    }

    public void CollideWithItem(GameObject g)
    {
        Debug.Log("yup, I see you: ", g);
        g.transform.GetComponent<Item>().KillMe();
    }

    public Vector3 GetPosition()
    {
        return ShipTransform.position;
    }

    public Vector3 GetHomePositionRange()
    {
        Vector3 v = transform.position;
        return v + (spawnPosition - v)/3;
    }

    public void PointAtNearestPoint(Vector3 target)
    {
        if(CompassArrow.activeSelf == false) CompassArrow.SetActive(true);

        CompassArrow.transform.LookAt(target);
    }

    public bool GetTargetable()
    {
        return targetable;
    }

    public void SetTargetable(bool b)
    {
        targetable = b;
    }

    public void OnTriggerEnt()
    {
        PickUpParticle.GetComponent<ParticleSystem>().Play();
    }

    public void OnTriggerDead()
    {
        GameSystem.Instance.SetState(new DeadState(GameSystem.Instance));
        Destroy(gameObject);
    }
}

