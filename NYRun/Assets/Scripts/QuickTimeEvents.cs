using UnityEngine;

public class QuickTimeEvents : MonoBehaviour
{
   public int KeyPressedAmount = 0;

    public char TargetKey;

    public char[] QuickTimeKeys = {'E','F','M','T','A'};
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Start Mashing!");
        QuickTimeEventStart();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            QuickTimeEvent('E');
        }
        if(Input.GetKeyDown(KeyCode.F))
        {
            QuickTimeEvent('F');
        }
        if(Input.GetKeyDown(KeyCode.M))
        {
            QuickTimeEvent('M');
        }
        if(Input.GetKeyDown(KeyCode.T))
        {
            QuickTimeEvent('T');
        }
        if(Input.GetKeyDown(KeyCode.A))
        {
            QuickTimeEvent('A');
        }
        if(KeyPressedAmount == 10)
        {
            QuickTimeEventSuccess();
        }

        
    }

    public void QuickTimeEventStart()
    {
        TargetKey = QuickTimeKeys[Random.Range(0,QuickTimeKeys.Length)];
        Debug.Log(("Mash The KEY!!!!!")+TargetKey);
    }

    public void QuickTimeEvent(char PlayerKey)
    {
        if(PlayerKey == TargetKey)
        {
            KeyPressedAmount++;
        }
    }

    public void QuickTimeEventSuccess()
    {
        Debug.Log("You Did It!");
        KeyPressedAmount = 0;
    }
}
