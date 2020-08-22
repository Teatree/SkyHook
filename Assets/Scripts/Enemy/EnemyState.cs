using System.Collections;

public abstract class EnemyState {
    public readonly EnemyBirdSystem EnemyBirdSystem;

    public EnemyState(EnemyBirdSystem enemyBirdSystem)
    {
        EnemyBirdSystem = enemyBirdSystem;
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
