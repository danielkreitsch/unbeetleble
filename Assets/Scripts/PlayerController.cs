using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D rb = new Rigidbody2D();

    [SerializeField]
    private GroundChecker groundChecker;
    
    [Header("Movement")]
    [SerializeField]
    private float moveSpeed;

    [SerializeField]
    private float movementSmoothing;
    
    [Header("Jumping")]
    [SerializeField]
    private float jumpVelocity;
    
    [SerializeField]
    private float jumpReleaseGravityMultiplier = 1;
    
    [Header("Gravity")]
    [SerializeField]
    private float gravityForce = 0;

    [SerializeField]
    private float fallGravityMultiplier = 1;

    [Range(0, 25)]
    [SerializeField]
    private float idleGravityLimit = 0;

    private Vector2 velocity = new Vector2();
    
    private float horizontalInput = 0;
    private bool jumpInput = false;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.horizontalInput = Input.GetAxis("Horizontal");
        
        if (Input.GetButtonDown("Jump"))
        {
            this.jumpInput = true;
        }
        
        if (this.velocity.y < -this.idleGravityLimit && this.groundChecker.TouchingGround)
        {
            this.velocity.y = -this.idleGravityLimit;
        }

        // Gravity
        this.velocity += Time.deltaTime * this.gravityForce * Vector2.down;

        // Jumping
        if (this.jumpInput)
        {
            if (this.groundChecker.TouchingGround)
            {
                this.rb.AddForce(new Vector2(0f, this.jumpVelocity));
                //this.rb.velocity = new Vector2(this.rb.velocity.x, this.jumpVelocity);
                this.jumpInput = false;
            }
        }
        
        // Higher gravity when falling
        /*if (this.velocity.y < 0)
        {
            this.velocity += Time.deltaTime * Physics2D.gravity.y * (this.fallGravityMultiplier - 1) * Vector2.up;
        }
        // Higher gravity when the jump button isn't hold down
        else if (this.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            this.velocity += Time.deltaTime * this.jumpReleaseGravityMultiplier * Physics2D.gravity;
        }*/
    }

    void FixedUpdate()
    {
        
        
        // Aktuell hat der Spieler noch komplette horizontale Kontrolle (evtl noch ändern)
        var targetVel = new Vector2(this.horizontalInput * this.moveSpeed, this.rb.velocity.y);
        this.rb.velocity = targetVel;
        //this.rb.velocity = Vector2.SmoothDamp(this.rb.velocity, targetVel, ref this.currentVelocity, this.movementSmoothing);
    }
}
