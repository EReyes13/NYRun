using UnityEngine;
using UnityEngine.Events;

public class BusScript : MonoBehaviour
{
    [Header("Feedbacks")]
    public UnityEvent busMoving;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        busMoving.Invoke();
    }
}
