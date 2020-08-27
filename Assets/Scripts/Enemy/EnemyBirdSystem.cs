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
        InitialEnemyPos = enemyPrefab.GetComponent<EnemyBird>().GetPosition();

        SetState(new EnemyBirdIdleState(this));
    }
}

public class EnemyBirdIdleState : EnemyState {

    float patrolDuration;
    float patrolCounter;
    float patrolWaitDuration;
    float patrolWaitCounter = 0;
    Vector3 patrolTargetPosition;
    Vector3 patrolDirNormalized;

    Vector3 oldPos;
    Vector3 newPos;
    public Vector3 rotationPivot;

    public EnemyBirdIdleState(EnemyBirdSystem enemyBirdSystem) : base(enemyBirdSystem)
    {

    }

    public override IEnumerator Start()
    {
        patrolDuration = 1f; //seconds
        patrolWaitDuration = 5f; //seconds

        patrolTargetPosition = new Vector3(EnemyBirdSystem.InitialEnemyPos.x + UnityEngine.Random.Range(-2, 2), EnemyBirdSystem.InitialEnemyPos.y, EnemyBirdSystem.InitialEnemyPos.z + UnityEngine.Random.Range(-2, 2));
        patrolDirNormalized = (EnemyBirdSystem.enemyPrefab.transform.position - patrolTargetPosition).normalized;

        yield return null;
    }

    public override void OnUpdate()
    {
        if (Vector3.Distance(Player.Instance.GetPosition(), EnemyBirdSystem.enemyPrefab.transform.position) < 45)
        {
            EnemyBirdSystem.SetState(new EnemyBirdChasingState(EnemyBirdSystem));
        }

        Patrol();
    }

    public void Patrol()
    {
        patrolCounter += Time.deltaTime;

        // old pos
        oldPos = EnemyBirdSystem.enemyPrefab.transform.position;

        if (patrolCounter < patrolDuration)
        {
            EnemyBirdSystem.enemyPrefab.transform.position = EnemyBirdSystem.enemyPrefab.transform.position + patrolDirNormalized * 7 * Player.CurrentSpeedIncrement * Time.deltaTime;
            //EnemyBirdSystem.enemyPrefab.transform.LookAt(patrolTargetPosition);
        }
        else
        {
            if (patrolWaitCounter <= 0.1f) rotationPivot = EnemyBirdSystem.enemyPrefab.transform.position - EnemyBirdSystem.enemyPrefab.transform.forward * 3;

            patrolWaitCounter += Time.deltaTime;

            EnemyBirdSystem.enemyPrefab.transform.RotateAround(rotationPivot, Vector3.up, 90 * Time.deltaTime);

            if (patrolWaitCounter > patrolWaitDuration)
            {
                patrolTargetPosition = new Vector3(EnemyBirdSystem.InitialEnemyPos.x + UnityEngine.Random.Range(-2, 2), EnemyBirdSystem.InitialEnemyPos.y, EnemyBirdSystem.InitialEnemyPos.z + UnityEngine.Random.Range(-2, 2));

                patrolDirNormalized = (patrolTargetPosition - EnemyBirdSystem.enemyPrefab.transform.position).normalized;

                //Debug.Log("initianPos: "+ EnemyBirdSystem.InitialEnemyPos + " patrolTargetPosition: " + patrolTargetPosition + " patrolDirNormalized: " + patrolDirNormalized);
                patrolWaitCounter = 0;
                patrolCounter = 0;
            }
        }

        // new pos
        newPos = EnemyBirdSystem.enemyPrefab.transform.position;
        Vector3 dir = (oldPos - newPos).normalized;
        // look at
        Vector3 lookAtter = oldPos - dir * 2;
        EnemyBirdSystem.enemyPrefab.transform.LookAt(lookAtter);
        Debug.Log("new:" + newPos + " old: " + oldPos);
    }
}

public class EnemyBirdChasingState : EnemyState {

    Vector3 dirNormalized;
    Vector3 dirNormalizedHome;

    public EnemyBirdChasingState(EnemyBirdSystem enemyBirdSystem) : base(enemyBirdSystem)
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
                EnemyBirdSystem.SetState(new EnemyBirdLostState(EnemyBirdSystem));
            }
            if (Vector3.Distance(Player.Instance.GetPosition(), EnemyBirdSystem.enemyPrefab.transform.position) < 20)
            {
                EnemyBirdSystem.SetState(new EnemyBirdPrepareChargeState(EnemyBirdSystem));
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

public class EnemyBirdLostState : EnemyState {

    float time;
    float counter;

    Vector3 initialPos;

    float patrolDuration;
    float patrolCounter;
    float patrolWaitDuration;
    float patrolWaitCounter;
    Vector3 patrolTargetPosition;
    Vector3 patrolDirNormalized;

    Vector3 oldPos;
    Vector3 newPos;
    public Vector3 rotationPivot;

    public EnemyBirdLostState(EnemyBirdSystem enemyBirdSystem) : base(enemyBirdSystem)
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
            EnemyBirdSystem.SetState(new EnemyBirdReturningState(EnemyBirdSystem));
        }

        if (Vector3.Distance(Player.Instance.GetPosition(), EnemyBirdSystem.enemyPrefab.transform.position) < 45)
        {
            EnemyBirdSystem.SetState(new EnemyBirdChasingState(EnemyBirdSystem));
        }

        PatrolLost();
    }

    public void PatrolLost()
    {
        patrolCounter += Time.deltaTime;

        // old pos
        oldPos = EnemyBirdSystem.enemyPrefab.transform.position;

        if (patrolCounter < patrolDuration)
        {
            EnemyBirdSystem.enemyPrefab.transform.position = EnemyBirdSystem.enemyPrefab.transform.position + patrolDirNormalized * 7 * Player.CurrentSpeedIncrement * Time.deltaTime;
            //EnemyBirdSystem.enemyPrefab.transform.LookAt(patrolTargetPosition);
        }
        else
        {
            if (patrolWaitCounter <= 0.1f) rotationPivot = EnemyBirdSystem.enemyPrefab.transform.position - EnemyBirdSystem.enemyPrefab.transform.forward * 3;

            patrolWaitCounter += Time.deltaTime;

            EnemyBirdSystem.enemyPrefab.transform.RotateAround(rotationPivot, Vector3.up, 90 * Time.deltaTime);

            if (patrolWaitCounter > patrolWaitDuration)
            {
                patrolTargetPosition = new Vector3(EnemyBirdSystem.InitialEnemyPos.x + UnityEngine.Random.Range(-2, 2), EnemyBirdSystem.InitialEnemyPos.y, EnemyBirdSystem.InitialEnemyPos.z + UnityEngine.Random.Range(-2, 2));

                patrolDirNormalized = (patrolTargetPosition - EnemyBirdSystem.enemyPrefab.transform.position).normalized;

                //Debug.Log("initianPos: "+ EnemyBirdSystem.InitialEnemyPos + " patrolTargetPosition: " + patrolTargetPosition + " patrolDirNormalized: " + patrolDirNormalized);
                patrolWaitCounter = 0;
                patrolCounter = 0;
            }
        }

        // new pos
        newPos = EnemyBirdSystem.enemyPrefab.transform.position;
        Vector3 dir = (oldPos - newPos).normalized;
        // look at
        Vector3 lookAtter = oldPos - dir * 2;
        EnemyBirdSystem.enemyPrefab.transform.LookAt(lookAtter);
        Debug.Log("new:" + newPos + " old: " + oldPos);
    }
}

public class EnemyBirdReturningState : EnemyState {

    Vector3 dirNormalizedHome;

    public EnemyBirdReturningState(EnemyBirdSystem enemyBirdSystem) : base(enemyBirdSystem)
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
            EnemyBirdSystem.SetState(new EnemyBirdIdleState(EnemyBirdSystem));
        }

        if (Vector3.Distance(Player.Instance.GetPosition(), EnemyBirdSystem.enemyPrefab.transform.position) < 45)
        {
            EnemyBirdSystem.SetState(new EnemyBirdChasingState(EnemyBirdSystem));
        }
    }
}

public class EnemyBirdPrepareChargeState : EnemyState {

    float counter;
    float time;

    public EnemyBirdPrepareChargeState(EnemyBirdSystem enemyBirdSystem) : base(enemyBirdSystem)
    {
        time = 1.5f;
    }

    public override IEnumerator Start()
    {
        while (counter <= time)
        {
            counter += Time.deltaTime / time;

            // turn red
            EnemyBirdSystem.enemyPrefab.GetComponent<EnemyBird>().TurnRed();

            // look at Player direction


            yield return null;
        }

        // change to charge
        EnemyBirdSystem.SetState(new EnemyBirdChargeState(EnemyBirdSystem));
    }
}

public class EnemyBirdChargeState : EnemyState {

    Vector3 playerPredictPos;
    Vector3 dirPredict;
    float counter;
    float time;

    public EnemyBirdChargeState(EnemyBirdSystem enemyBirdSystem) : base(enemyBirdSystem)
    {
        time = 3f;
    }

    public override IEnumerator Start()
    {
        playerPredictPos = Player.Instance.GetPredictedPosition();

        Vector3 d = EnemyBirdSystem.enemyPrefab.transform.position - playerPredictPos;
        d = EnemyBirdSystem.enemyPrefab.transform.position - d * 3f;

        while (counter <= time)
        {
            counter += Time.deltaTime / time;

            // move to Player predicted position
            dirPredict = (d - EnemyBirdSystem.enemyPrefab.transform.position).normalized;

            //EnemyBirdSystem.enemyPrefab.transform.position = d;
            EnemyBirdSystem.enemyPrefab.transform.position = EnemyBirdSystem.enemyPrefab.transform.position + dirPredict * 21 * Player.CurrentSpeedIncrement * Time.deltaTime;
            EnemyBirdSystem.enemyPrefab.transform.LookAt(d);

            yield return null;
        }

        // change to charge
        EnemyBirdSystem.enemyPrefab.GetComponent<EnemyBird>().Die();
    }
}