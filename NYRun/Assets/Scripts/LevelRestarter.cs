using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelRestarter : MonoBehaviour
{

    public float Countdown = 5;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Countdown -= Time.deltaTime;
        if(Countdown <= 0)
        {
            SceneManager.LoadScene(1);
        }
    }
}
