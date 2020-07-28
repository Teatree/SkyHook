using System.Collections;

public abstract class State {
    protected readonly GameSystem _system;

    public State(GameSystem system)
    {
        _system = system;
    }

    public virtual IEnumerator Start()
    {
        yield break;
    }

    public virtual IEnumerator Attack()
    {
        yield break;
    }

    public virtual IEnumerator Heal()
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
