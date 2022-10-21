using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E1 : MonoBehaviour
{

    private Rigidbody2D rb;
    private Animator anim;

    private bool isGrounded;

    public Transform groundCheck;
    public LayerMask whatIsGround;

    public float speedForce;
    public float checkRadius;
    public float E1Direction = -1f;  

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        AnimationControl();
        Checkers();
        if(isGrounded)
        {
            rb.velocity = new Vector2(speedForce * E1Direction, rb.velocity.y);
        }
        else
        {
            Flip();    
        }

    }

    void Flip()
    {
        transform.Rotate(0, 180, 0);
        E1Direction *= -1;
    }

    void AnimationControl()
    {
        anim.SetBool("run_param", isGrounded);
    }


    void Checkers()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround); 
    }

    private void OnDrawGizmosSelected()
    {        
        //GROUNDCHECK
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(groundCheck.position, checkRadius);
    }
}
