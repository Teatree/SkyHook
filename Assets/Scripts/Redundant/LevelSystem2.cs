using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSystem2 : SceneSingleton<LevelSystem2>
{
    public GameObject SpinnerGo;
    Vector3 generatedPoint;
    Vector3 pointToMoveTo;
    Vector3 initialPositionSaved;
    float moveCounter;

    IEnumerator moveSpinnerToPlayer;

    public void Update()
    {
        
    }
    // create a bunch of spawnables

    // create an end point
    // place the end point thing there too
    public Vector3 GenerateEndPoint()
    {
        generatedPoint = new Vector3(0, 10, 1000);
        SpinnerGo.SetActive(true);
        SpinnerGo.transform.position = generatedPoint;
        pointToMoveTo = transform.position - generatedPoint;
        initialPositionSaved = transform.position;

        return SpinnerGo.transform.position;
    }

    public void MoveLevel(float timeToFinish)
    {
        // constantly move with update, gradually increasing speed in time
        if (moveCounter < timeToFinish)
        {
            moveCounter += Time.deltaTime;
            //Debug.Log("moveCounter: " + moveCounter);

            transform.position = Vector3.Lerp(initialPositionSaved, pointToMoveTo, moveCounter / timeToFinish);
        }
    }

    public void MoveSpinnerToPlayer(Vector3 pos, float timeToFinish)
    {
        moveSpinnerToPlayer = MoveSpinnerToPlayerC(pos, timeToFinish);
        StartCoroutine(moveSpinnerToPlayer);
        Debug.Log("starting couroutine");
    }

    public IEnumerator MoveSpinnerToPlayerC(Vector3 pos, float timeToFinish)
    {
        float counter = 0;
        Vector3 spinnerPosition = SpinnerGo.transform.position;

        while (counter < timeToFinish)
        {
            counter += Time.deltaTime;
            Vector3 actualPos = new Vector3(pos.x, pointToMoveTo.y, pos.z);

            SpinnerGo.transform.position = Vector3.Lerp(spinnerPosition, actualPos, counter / timeToFinish);

            yield return null;
        }

        counter = 0;
        Debug.Log("done");
    }

    public void ResetMoveCounter()
    {
        // terrible
        moveCounter = 0;
    }
}
