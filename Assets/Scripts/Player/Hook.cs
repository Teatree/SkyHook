using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : SceneSingleton<Hook>
{
    IEnumerator hookSendCouroutine;
    LineRenderer hookLine;
    Vector3 targetPosSaved;
    Vector3 targetPosMoved;
    float _counter;

    void Start()
    {
        hookLine = GetComponent<LineRenderer>();
    }

    public void SendHook(float t)
    {
        //Vector3 pos = LevelSystem.Instance.GenerateEndPoint();
        hookLine.positionCount = 2;

        //hookSendCouroutine = SendHookCouroutine(t, pos);
        StartCoroutine(hookSendCouroutine);
    }

    IEnumerator SendHookCouroutine(float time, Vector3 pos)
    {
        float counter = 0;
        while (counter < time)
        {
            counter += Time.deltaTime;
            hookLine.SetPosition(0, transform.position);

            //somewhere here we will also need to figure out interseption course for moving hookables
            Vector3 newTargetPos = Vector3.Lerp(transform.position, pos, counter / time);

            hookLine.SetPosition(1, newTargetPos);
            targetPosSaved = newTargetPos;

            yield return null;
        }

        targetPosMoved = targetPosSaved;

        counter = 0;
    }

    public void SetHookPos(Vector3 pos)
    {
        hookLine.SetPosition(0, pos);
        hookLine.SetPosition(1, targetPosMoved);
    }

    public Vector3 GetHookPos()
    {
        return transform.position;
    }

    public void PullHookPos(Vector3 pos, float timeToPull)
    {
        if (_counter < timeToPull)
        {
            _counter += Time.deltaTime;
            targetPosMoved = Vector3.Lerp(targetPosSaved, pos, _counter / timeToPull);
        }
    }

    public void ClearPositions()
    {
        hookLine.positionCount = 0;
    }

    public Vector3 GetTargetPosSaved()
    {
        return targetPosSaved;
    }
}
