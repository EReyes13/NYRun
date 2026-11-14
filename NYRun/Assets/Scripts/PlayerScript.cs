
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public Rigidbody2D PlayerPhysics;
    public int JumpForce = 10;
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            PlayerPhysics.AddForce(new Vector2(0,1)*JumpForce,ForceMode2D.Impulse);
        }
    }
}
