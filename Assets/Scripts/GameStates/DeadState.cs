using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeadState : State {

    public DeadState(GameSystem gameSystem) : base(gameSystem)
    {

    }

    public override IEnumerator Start()
    {
        GameSystem.PlayDeathParticle();

        yield return null;
    }

    public override void OnUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Scene scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name);
        }
    }
}
