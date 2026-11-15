
using UnityEngine;
using UnityEngine.Events;

public class PlayerScript : MonoBehaviour
{
    [Header("Feedbacks")] 
    public UnityEvent onJump;
    
    public Rigidbody2D PlayerPhysics;
    public Animator PlayerAnimator;

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

                PlayerAnimator.SetBool("IsJumping", true);
            }
        }
    }

    public void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.CompareTag("Ground"))
        {
            CanJump = true;
            PlayerAnimator.SetBool("IsJumping", false);

            Debug.Log("Grounded");
        }
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        CanJump = false;
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
}
