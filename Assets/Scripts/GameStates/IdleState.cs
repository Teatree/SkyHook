using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : State
{
    public IdleState(GameSystem gameSystem) : base(gameSystem)
    {

    }

    public override void OnUpdate()
    {
        if (Input.GetMouseButtonUp(0))
        {
            GameSystem.SetState(new HookState(GameSystem));
        }
    }
}
