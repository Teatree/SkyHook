using UnityEngine;

public interface IInitable {
    void Init();
}

public class SceneSingleton<T> : MonoBehaviour, IInitable where T : MonoBehaviour {
    protected static T _instance;

    public static T Instance {
        get {
            if (_instance == null)
                Fetch();
            return _instance;
        }
    }

    static void Fetch()
    {
        try
        {

            _instance = (T)FindObjectOfType(typeof(T));
            if (_instance != null)
            {
                (_instance as IInitable).Init();
            }
            else
            {
                Debug.Log(">>>> fetch singleton > " + typeof(T));
            }
        }
        catch (MissingReferenceException e)
        {
            Debug.Log(">>>> e fetch singleton > " + typeof(T));
        }
    }

    public virtual void Init()
    {
        // NOTE: use this function instead of default Unity event function
    }

    protected virtual void Awake()
    {
        if (_instance == null)
            Fetch();
        else if (!ReferenceEquals(_instance, this))
            Debug.LogError(name + ": second instance exists!");
    }

    protected virtual void OnDestroy()
    {
        if (ReferenceEquals(_instance, this))
            _instance = null;
        else
            Debug.LogError(name + ": second instance exists!");
    }
}