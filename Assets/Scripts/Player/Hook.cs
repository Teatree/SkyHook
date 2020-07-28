using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : SceneSingleton<Hook>
{
    IEnumerator hookSendCouroutine;
    LineRenderer hookLine;
    public Vector3 targetPosSaved;

    void Start()
    {
        hookLine = GetComponent<LineRenderer>();
    }

    public Vector3 GeneratePosition()
    {
        return new Vector3(0, 10, 300);// forward from the Player
    }

    public void SendHook(float t)
    {
        Vector3 pos = GeneratePosition();

        hookSendCouroutine = SendHookCouroutine(t, pos);
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

        counter = 0;
    }

    public void SetHookPos(Vector3 pos)
    {
        hookLine.SetPosition(0, pos);
        hookLine.SetPosition(1, targetPosSaved);
    }
}
