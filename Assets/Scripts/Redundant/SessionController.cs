using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SessionController : SceneSingleton<SessionController>
{
    // it controlls the session
    public int CoinsAmountSession;

    [Header("Clue Gathering")]
    public float TimeBeforeFirstClue;
    public float BreakTimeBeforeNextClue;
    public float NumberOfCluesInitial;
    public float NumberOfCluesIcrement;

    [Header("Player Session Info")]
    public float PlayerSpeedIncreaseValuef; 
    public float PlayerSpeedMaxSpeedf;

    float restartCounter;
    IEnumerator restartGameCouroutine;

    float currentMinimumPush;
    float currentMaximumPush;

    // Update is called once per frame
    void Update()
    {
        //if (PlayerBehaviour.Instance.GetState() == PlayerBehaviour.PlayerState.dead)
        //{
        //    Scene scene = SceneManager.GetActiveScene();
        //    SceneManager.LoadScene(scene.name);
        //}

        //Beeeeeee, I change your code!!! What are you going to do about it!! Muahahahahhaha
    }

    public void IncreaseThePush()
    {
        currentMinimumPush += 0.2f;
        currentMaximumPush += 0.4f;
    }

    public float GetCurrentHookableMinPush()
    {
        return currentMinimumPush;
    }

    public float GetCurrentHookableMaxPush()
    {
        return currentMaximumPush;
    }

    public void RestartGame()
    {
        restartGameCouroutine = StartNewLevel(2f);
        StartCoroutine(restartGameCouroutine);
    }

    IEnumerator StartNewLevel(float timer)
    {
        while (restartCounter < timer)
        {
            restartCounter += Time.deltaTime / timer;

            yield return null;
        }

        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
}
