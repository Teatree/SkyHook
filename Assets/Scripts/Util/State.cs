using System.Collections;

public abstract class State {
    public readonly GameSystem GameSystem;

    public State(GameSystem system)
    {
        GameSystem = system;
    }

    public virtual void OnUpdate()
    {

    }

    public virtual IEnumerator Start()
    {
        yield break;
    }

    public virtual IEnumerator Pause()
    {
        yield break;
    }

    public virtual IEnumerator Resume()
    {
        yield break;
    }
}
