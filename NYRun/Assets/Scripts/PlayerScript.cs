
using UnityEngine;
using UnityEngine.Events;

public class PlayerScript : MonoBehaviour
{
    [Header("Feedbacks")] 
    public UnityEvent onJump;
    public UnityEvent RunningSFX;
    public UnityEvent pauseRunningSFX;
    
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
                onJump.Invoke();
            }
        }
    }

    public void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.CompareTag("Ground"))
        {
            CanJump = true;
            Debug.Log("Grounded");
            RunningSFX.Invoke();
        }
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        CanJump = false;
        RunningSFX.Invoke();
        Debug.Log("In Air");

    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Obstacle"))
        {
            GameManager.Instance.ObstacleHit();
        }
        else
        {
            GameManager.Instance.StartQuickEvent();
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
                if(collision.gameObject.CompareTag("Checkpoint"))
        {
            GameManager.Instance.QuickEventActive = true;
        }
    }
}
