using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SessionController : SceneSingleton<SessionController>
{
    // it controlls the session
    public int CoinsAmountSession;

    [Header("Clue Gathering")]
    public float TimeBeforeFirstClue;
    public float NumberOfCluesInitial;
    public float NumberOfCluesIcrement;

    [Header("Player Session Info")]
    public float PlayerSpeedIncreaseValuef; 
    public float PlayerSpeedMaxSpeedf; 

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
