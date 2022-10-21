using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private BoxCollider2D boxCollider;

    private float speedUpTime;
    private float speedForceStart;
    private float moveInput;

    private bool isGrounded;
    private bool isTouchingFrontWallTop;
    private bool isTouchingFrontWallMid;
    private bool isTouchingFrontWallBot;
    private bool isTouchingFrontGround;
    private bool isPlayerLookingRight = true;
    private bool doubleJumpChecker;

    public Transform groundCheck;
    public Transform frontTopCheck;
    public Transform frontMidCheck;
    public Transform frontBotCheck;
    public LayerMask whatIsGround;
    public LayerMask whatIsWall;
    public Vector2 wallJumpVector;

    public float speedForce;
    public float speedMaxForce;
    public float jumpForce;
    public float fastFallForce;
    public float wallSlideForce;
    public float wallJumpDirection;
    public float checkRadius; 


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        speedForceStart = speedForce;
        //boxCollider = GetComponent<BoxCollider2D>();
    } // End of Start()

    void Update()
    {

        UserInput();
        PlayerMovement();
        Checkers();

        // When touching the ground values reset
        GroundReset();

        //Set animator parameters
        AnimationControl();

        // Wall Slide
        if(WallSlideChecker() && !isGrounded && moveInput != 0)
        {
            WallSlide();
        }
        // // Wall Jump
        // if(Input.GetKeyDown(KeyCode.Space) && isTouchingFrontWallTop && !isGrounded && moveInput != 0)
        // {
        //     rb.velocity = new Vector2(wallJumpDirection * 50, 50);
        //     //rb.velocity = new Vector2(wallJumpDirection * speedForce, jumpForce);
        //     Debug.Log(Input.GetKeyDown(KeyCode.Space) );
        //     Debug.Log("TODO WALLJUMP");
        //     //WallJump();
        // }
        // Wall Jump
        // if(Input.GetKeyDown(KeyCode.Space) && WallSlideChecker() && !isGrounded && moveInput != 0 && (rb.velocity.y < -0.99f))
        // {
        //     rb.velocity = new Vector2(wallJumpDirection * wallJumpVector.x, -rb.velocity.y * wallJumpVector.y);
        //     Debug.Log("TODO BETTER WALLJUMP");
        //     //WallJump();
        // }

        // Crouch 
        if(Input.GetAxis("Vertical") < -0.99f && isGrounded)
        {
            Crouch();
        }      

        // Player Is Falling
        if(rb.velocity.y < -0.99f && !isGrounded && !WallSlideChecker())
        {
            Falling();
        }

    } // End of Update()

    void Flip()
    {
        wallJumpDirection *= -1;
        isPlayerLookingRight = !isPlayerLookingRight;
        transform.Rotate(0, 180, 0);
    } 

    void PlayerMovement()
    {
        rb.velocity = new Vector2(moveInput * speedForce, rb.velocity.y);
        //Debug.Log(rb.velocity.x);
        
        // Flip player when moving left or right
        if(moveInput < 0f && isPlayerLookingRight)
        {
            Flip();
            
        }
        else if(moveInput > 0f && !isPlayerLookingRight)
        {
            Flip();
        }

        // Player Jump
        if(Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }
        // Double Jump
        else if(Input.GetKeyDown(KeyCode.Space) && doubleJumpChecker && !isGrounded)
        {
            DoubleJump();
        }

        // FastFall 
        if(Input.GetAxis("Vertical") < -0.5f && !isGrounded)
        {
            FastFall();
        }

    } // End of PlayerMovement()


    void Jump()
    {
        anim.SetTrigger("jump_param");
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    } 

    void DoubleJump()
    {
        Jump();
        doubleJumpChecker = false;
        anim.SetBool("doubleJump_param", false);
    }

    void Crouch()
    {
        anim.SetBool("crouch_param", true);
        anim.SetBool("falling_param", false);
        rb.velocity = new Vector2(rb.velocity.x/5, 0);
    }

    void FastFall()
    {
        rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y-fastFallForce);
        //rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y-fastFallForce, -fastFallForce, float.MaxValue));
    }

    void Falling()
    {
        anim.SetBool("falling_param", true);
    }

    void WallSlide()
    {
        anim.SetBool("onWall_param", true);
        rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y-wallSlideForce, -wallSlideForce, float.MaxValue));
    }

    bool WallSlideChecker()
    {
        if(isTouchingFrontWallTop || isTouchingFrontWallBot || isTouchingFrontWallMid)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // void WallJump()
    // {
    //     rb.velocity = new Vector2(moveInput * wallJumpForce.x * wallJumpDirection, wallJumpForce.y);
    // } //TODO WALLJUMP 

    void UserInput()
    {
        moveInput = Input.GetAxis("Horizontal");
    } 

    void AnimationControl()
    {
        anim.SetBool("run_param", moveInput != 0);
        anim.SetBool("onGround_param", isGrounded);
        anim.SetBool("falling_param", false);
        anim.SetBool("crouch_param", false);
        anim.SetBool("onWall_param", false);
    } 

    void GroundReset()
    {
        if(isGrounded)
        {
            doubleJumpChecker = true;
            anim.SetBool("doubleJump_param", true);
        }
    } 

    void Checkers()
    {
        // Uses groundCheck to see if the player is touching the ground
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);
        // Uses frontCheck to see if the player is touching the wall
        isTouchingFrontWallTop = Physics2D.OverlapCircle(frontTopCheck.position, checkRadius, whatIsGround);
        isTouchingFrontWallMid = Physics2D.OverlapCircle(frontMidCheck.position, checkRadius, whatIsGround);
        isTouchingFrontWallBot = Physics2D.OverlapCircle(frontBotCheck.position, checkRadius, whatIsGround);  
    }

    private void OnDrawGizmosSelected()
    {
        // FRONTCHECK
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(frontTopCheck.position, checkRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(frontMidCheck.position, checkRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(frontBotCheck.position, checkRadius);

        
        //GROUNDCHECK
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(groundCheck.position, checkRadius);
    }
}
