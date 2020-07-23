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

    // Update is called once per frame
    void Update()
    {
        if (PlayerBehaviour.Instance.GetState() == PlayerBehaviour.PlayerState.dead)
        {
            Scene scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name);
        }
    }
}
