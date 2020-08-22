using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine : MonoBehaviour
{
    protected EnemyState EnemyState;

    public void SetState(EnemyState enemyState)
    {
        EnemyState = enemyState;
        Debug.Log(gameObject.name + " enemyState = " + EnemyState.ToString());
        StartCoroutine(EnemyState.Start());
    }

    public void Update()
    {
        EnemyState.OnUpdate();
    }
}
