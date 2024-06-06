using UnityEngine;

//Generic Singleton class. Primarily used for managers that need to be persistent between scenes.
public class MonoSingleton<T> : MonoBehaviour where T: MonoBehaviour 
{
    static T m_instance = null;
    bool m_persistant = false;

    public static T instance
    {
        get { return m_instance; }
    }

    public virtual void Awake()
    {
        if (m_instance != null && m_instance != this) {

            string _persistantText = "";

            if (m_persistant)
                _persistantText = "Persistant ";

            string s = string.Concat(_persistantText,"MonoSingleton: ",typeof(T).Name, " Object being instantiated more than once. Deleting GameObject");

            Debug.LogWarning(s);

            Destroy(this.gameObject);

            return;
        } else
            m_instance = this as T;

        if (m_persistant)
            DontDestroyOnLoad(this.gameObject);
    }

    // set before call base.awake() when overriding the awake fucntion
    public void MakePersistant(bool state)
    {
        m_persistant = state;
    }
}
