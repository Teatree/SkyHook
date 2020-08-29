using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : SceneSingleton<Player> {
    public static float CurrentSpeedIncrement = 1f;

    public float Health;
    public float Fuel;
    public List<ItemWorld> items;

    public int CoinsAmountTotal;
    public float OrbitDistance = 5.0f;
    public float IntitialSpeed = 0.5f;
    public bool targetable = true;

    public GameObject PickUpParticle;
    public Transform ShipTransform;
    public GameObject CompassArrow; //Temporary
    public GameObject DirectionIndicator; 

    Vector3 spawnPosition;
    float counter;
    public float increaseSpeedTimer;
    public float fuelDepleatingSpeed;

    public void Start()
    {
        spawnPosition = transform.position;
        Health = 100;
        Fuel = 100;
    }

    public void Update()
    {
        // timer increases speed
        counter += Time.deltaTime;
        if (counter > increaseSpeedTimer)
        {
            //CurrentSpeedIncrement += 0.03f;
            counter = 0;
        }

        UpdateFuel();
    }

    public void UpdateDirecionIndicator(Vector3 dir)
    {
        Vector3 lookAtPos = transform.position + dir;
        lookAtPos.y = transform.position.y;

        transform.LookAt(lookAtPos);
        DirectionIndicator.transform.LookAt(lookAtPos);
        DirectionIndicator.transform.eulerAngles = new Vector3(0, DirectionIndicator.transform.eulerAngles.y, DirectionIndicator.transform.eulerAngles.z);
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

    public Vector3 GetPredictedPosition()
    {
        return transform.position - transform.forward * -10f;
    }

    public Vector3 GetHomePositionRange()
    {
        Vector3 v = transform.position;
        return v + (spawnPosition - v)/3;
    }

    public void PointAtNearestPoint(Vector3 target)
    {
        //if(CompassArrow.activeSelf == false) CompassArrow.SetActive(true);

        Vector3 dir = target - transform.position;
        var angle = -Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        GameUISystem.Instance.homePointer.eulerAngles = new Vector3(0, 0, angle);
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

    public void DeclareYourItems()
    {
        GameUISystem.Instance.OpenResultScreenPopup(items);
    }

    public void TakeDamage(float damage)
    {
        Health -= damage;
        GameUISystem.Instance.UpdateHealthUI(Health);

        CheckIfDead();
    }

    public void AddItem(ItemWorld item)
    {
        items.Add(item);
    }

    public void UpdateFuel()
    {
        Fuel -= fuelDepleatingSpeed * Time.deltaTime;

        GameUISystem.Instance.UpdateFuelUI(Fuel);
    }

    void CheckIfDead()
    {
        if (Health <= 0)
        {
            OnTriggerDead();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(GetPredictedPosition(), 0.4f);
    }
}

