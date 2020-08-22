using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBirdSystem : EnemyStateMachine {

    //public static EnemyBirdSystem Instance;
    protected EnemyState EnemyState;

    [SerializeField] public GameObject enemyPrefab;

    public Vector3 InitialEnemyPos;

    private void Start()
    {
        //if (Instance == null) Instance = this;

        //enemy = enemyPrefab.GetComponent<Enemy>();
        InitialEnemyPos = enemyPrefab.GetComponent<Enemy>().GetPosition();

        SetState(new IdleState(this));
    }
}

public class IdleState : EnemyState {

    float patrolDuration;
    float patrolCounter;
    float patrolWaitDuration;
    float patrolWaitCounter;
    Vector3 patrolTargetPosition;
    Vector3 patrolDirNormalized; 


    public IdleState(EnemyBirdSystem enemyBirdSystem) : base(enemyBirdSystem)
    {

    }

    public override IEnumerator Start()
    {
        patrolDuration = 2f; //seconds
        patrolWaitDuration = 2f; //seconds

        patrolTargetPosition = new Vector3(EnemyBirdSystem.InitialEnemyPos.x + UnityEngine.Random.Range(-2, 2), EnemyBirdSystem.InitialEnemyPos.y, EnemyBirdSystem.InitialEnemyPos.z + UnityEngine.Random.Range(-2, 2));
        patrolDirNormalized = (EnemyBirdSystem.enemyPrefab.transform.position - patrolTargetPosition).normalized;

        yield return null;
    }

    public override void OnUpdate()
    {
        if (Vector3.Distance(Player.Instance.GetPosition(), EnemyBirdSystem.enemyPrefab.transform.position) < 45)
        {
            EnemyBirdSystem.SetState(new ChasingState(EnemyBirdSystem));
        }

        PatrolLost();
    }

    public void PatrolLost()
    {
        patrolCounter += Time.deltaTime;
        if (patrolCounter < patrolDuration)
        {
            EnemyBirdSystem.enemyPrefab.transform.position = EnemyBirdSystem.enemyPrefab.transform.position + patrolDirNormalized * 7 * Player.CurrentSpeedIncrement * Time.deltaTime;
        }
        else
        {
            patrolWaitCounter += Time.deltaTime;
            if (patrolWaitCounter > patrolWaitDuration)
            {
                patrolTargetPosition = new Vector3(EnemyBirdSystem.InitialEnemyPos.x + UnityEngine.Random.Range(-2, 2), EnemyBirdSystem.InitialEnemyPos.y, EnemyBirdSystem.InitialEnemyPos.z + UnityEngine.Random.Range(-2, 2));
                patrolDirNormalized = (EnemyBirdSystem.enemyPrefab.transform.position - patrolTargetPosition).normalized;
                patrolWaitCounter = 0;
                patrolCounter = 0;
            }
        }
    }
}

public class ChasingState : EnemyState {

    Vector3 dirNormalized;
    Vector3 dirNormalizedHome;

    public ChasingState(EnemyBirdSystem enemyBirdSystem) : base(enemyBirdSystem)
    {

    }

    public override IEnumerator Start()
    {


        yield return null;
    }

    public override void OnUpdate()
    {
        if (Player.Instance.GetTargetable() == true)
        {
            dirNormalized = (Player.Instance.GetPosition() - EnemyBirdSystem.enemyPrefab.transform.position).normalized;
           
            //Debug.Log("d:" + Vector3.Distance(Player.Instance.GetPosition(), transform.position));
            if (Vector3.Distance(Player.Instance.GetPosition(), EnemyBirdSystem.enemyPrefab.transform.position) >= 50)
            {
                EnemyBirdSystem.SetState(new LostState(EnemyBirdSystem));
            }
            else
            {
                EnemyBirdSystem.enemyPrefab.transform.position = EnemyBirdSystem.enemyPrefab.transform.position + dirNormalized * 7 * Player.CurrentSpeedIncrement * Time.deltaTime;
                EnemyBirdSystem.enemyPrefab.transform.LookAt(Player.Instance.GetPosition());
            }
            //transform.position = transform.position + dirNormalized * 7 * Time.deltaTime;
        }
    }
}

public class LostState : EnemyState {

    float time;
    float counter;

    Vector3 initialPos;

    float patrolDuration;
    float patrolCounter;
    float patrolWaitDuration;
    float patrolWaitCounter;
    Vector3 patrolTargetPosition;
    Vector3 patrolDirNormalized;


    public LostState(EnemyBirdSystem enemyBirdSystem) : base(enemyBirdSystem)
    {

    }

    public override IEnumerator Start()
    {
        time = 5; //seconds
        patrolDuration = 2f; //seconds
        patrolWaitDuration = 0.6f; //seconds

        initialPos = EnemyBirdSystem.enemyPrefab.transform.position;
        patrolTargetPosition = new Vector3(initialPos.x + UnityEngine.Random.Range(-2,2), initialPos.y, initialPos.z + UnityEngine.Random.Range(-2, 2));

        yield return null;
    }

    public override void OnUpdate()
    {
        counter += Time.deltaTime;
        if (counter > time)
        {
            EnemyBirdSystem.SetState(new ReturningState(EnemyBirdSystem));
        }

        if (Vector3.Distance(Player.Instance.GetPosition(), EnemyBirdSystem.enemyPrefab.transform.position) < 45)
        {
            EnemyBirdSystem.SetState(new ChasingState(EnemyBirdSystem));
        }

        PatrolLost();
    }

    public void PatrolLost()
    {
        patrolDirNormalized = (EnemyBirdSystem.enemyPrefab.transform.position - patrolTargetPosition).normalized;

        patrolCounter += Time.deltaTime;
        if(patrolCounter < patrolDuration)
        {
            EnemyBirdSystem.enemyPrefab.transform.position = EnemyBirdSystem.enemyPrefab.transform.position + patrolDirNormalized * 7 * Player.CurrentSpeedIncrement * Time.deltaTime;
        }
        else
        {
            patrolWaitCounter += Time.deltaTime;
            if(patrolWaitCounter > patrolWaitDuration)
            {
                patrolTargetPosition = new Vector3(initialPos.x + UnityEngine.Random.Range(-2, 2), initialPos.y, initialPos.z + UnityEngine.Random.Range(-2, 2));
                patrolWaitCounter = 0;
                patrolCounter = 0;
            }
        }     
    }
}

public class ReturningState : EnemyState {

    Vector3 dirNormalizedHome;

    public ReturningState(EnemyBirdSystem enemyBirdSystem) : base(enemyBirdSystem)
    {

    }

    public override IEnumerator Start()
    {


        yield return null;
    }

    public override void OnUpdate()
    {
        dirNormalizedHome = (EnemyBirdSystem.InitialEnemyPos - EnemyBirdSystem.enemyPrefab.transform.position).normalized;

        EnemyBirdSystem.enemyPrefab.transform.position = EnemyBirdSystem.enemyPrefab.transform.position + dirNormalizedHome * 7 * Player.CurrentSpeedIncrement * Time.deltaTime;
        EnemyBirdSystem.enemyPrefab.transform.LookAt(EnemyBirdSystem.InitialEnemyPos);

        if (Vector3.Distance(EnemyBirdSystem.enemyPrefab.transform.position, EnemyBirdSystem.InitialEnemyPos) < 5)
        {
            EnemyBirdSystem.SetState(new IdleState(EnemyBirdSystem));
        }

        if (Vector3.Distance(Player.Instance.GetPosition(), EnemyBirdSystem.enemyPrefab.transform.position) < 45)
        {
            EnemyBirdSystem.SetState(new ChasingState(EnemyBirdSystem));
        }
    }
}