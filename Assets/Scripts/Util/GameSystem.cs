using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSystem : StateMachine
{
    public const float TIME_TO_HOOK = 1.4f;
    public const float TIME_TO_PULL = 0.4f;

    private Player player;

    public Player Player => player;


}
