using UnityEngine;

public class Singleton : MonoBehaviour
{
    public static Singleton instance{get; private set;}
    
    void awake()
    {
        if (instance != null && instance != this)
        {
            Destroy (gameObject);
            return;
        }
        instance = this;
    }
}
