
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public Rigidbody2D PlayerPhysics;
    public bool CanJump = false;
    public int JumpForce = 10;
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(CanJump)
            {
                PlayerPhysics.AddForce(new Vector2(0,1)*JumpForce,ForceMode2D.Impulse);
            }
        }
    }

    public void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.CompareTag("Ground"))
        {
            CanJump = true;
            Debug.Log("Grounded");
        }
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        CanJump = false;
        Debug.Log("In Air");
    }
}
