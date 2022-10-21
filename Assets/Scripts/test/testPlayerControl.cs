using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [SerializeField]private float speedForce;
    [SerializeField]private float jumpForce;
    [SerializeField]private float wallSlideForce;

    [SerializeField]private LayerMask groundLayer;
    [SerializeField]private LayerMask wallLayer;

    private Rigidbody2D body;
    private Animator anim;
    private BoxCollider2D boxCollider;

    private bool grounded;
    private bool doubleJumpBool;

    private float wallJumpCooldown;



    // Awake is called when the script instance is being loaded.
    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        // Flip player when moving left or right
        if(horizontalInput > 0.01f)
        {
            transform.localScale = Vector3.one;
        }
        else if(horizontalInput < -0.01f)
        {
            transform.localScale = new Vector3(-1,1,1);
        }

        //Set animator parameters
        anim.SetBool("run_param", horizontalInput != 0);
        anim.SetBool("grounded_param", isGrounded());
        anim.SetBool("walljump_param", false);
        anim.SetBool("doublejump_param", false);
        doubleJumpBool = false;


        //Wall jump Logic
        if(wallJumpCooldown > 0.2f)
        {
            body.velocity = new Vector2(horizontalInput * speedForce, body.velocity.y);

            if(onWall() && !isGrounded())
            {
                //body.gravityScale = 0;
                //body.velocity = Vector2.zero;
                anim.SetBool("walljump_param", true);
                body.velocity = new Vector2(0, -wallSlideForce);
            }
            else
            {
                body.gravityScale = 5;
            }

            if(Input.GetKey(KeyCode.Space))
            {
                Jump();
                doubleJumpBool = true; 
                anim.SetBool("doublejump_param", true);
            }
        }
        else
        {
            wallJumpCooldown += Time.deltaTime;
        }

    }

    private void Jump()
    {
        if(isGrounded())
        {
            body.velocity = new Vector2(body.velocity.x, jumpForce);
            anim.SetTrigger("jump_param");
            Debug.Log("IF");
        }
        else if(onWall() && !isGrounded())
        {
            body.velocity = new Vector2(body.velocity.x, jumpForce);
            wallJumpCooldown = 0;
            //body.velocity = new Vector2();
        }
        else if(doubleJumpBool == true && !isGrounded())
        {
            body.velocity = new Vector2(body.velocity.x, jumpForce);
            anim.SetBool("doublejump_param", false);
            doubleJumpBool = false;
            Debug.Log("ELSE IF");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

    }

    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }

    private bool onWall()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, new Vector2(transform.localScale.x,0), 0.1f, wallLayer);
        return raycastHit.collider != null;
    }
}
