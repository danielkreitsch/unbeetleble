using System;
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
    private float fallGravityMultiplier = 1;

    [Range(0, 25)]
    [SerializeField]
    private float gravityLimit = 0;

    //private Vector2 velocity = new Vector2();
    
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
    }

    void FixedUpdate()
    {
        // Aktuell hat der Spieler noch komplette horizontale Kontrolle (evtl noch ändern)
        var targetVel = new Vector2(this.horizontalInput * this.moveSpeed, this.rb.velocity.y);
        
        // Jumping
        if (this.jumpInput)
        {
            if (this.groundChecker.touchingGround)
            {
                targetVel.y = this.jumpVelocity;
                this.jumpInput = false;
            }
        }
        this.jumpInput = false;
        
        // Higher gravity when falling
        if (this.rb.velocity.y < 0)
        {
            targetVel.y = targetVel.y * this.fallGravityMultiplier;
        }
        // Higher gravity when the jump button isn't hold down
        else if (this.rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            targetVel.y = targetVel.y * this.jumpReleaseGravityMultiplier;
        }

        if (targetVel.y < -this.gravityLimit)
        {
            targetVel.y = -this.gravityLimit;
        }
        
        this.rb.velocity = targetVel;
        //this.rb.velocity = Vector2.SmoothDamp(this.rb.velocity, targetVel, ref this.currentVelocity, this.movementSmoothing);
    }

    void OnGUI()
    {
        //GUI.Box(new Rect(5, 5, 400, 100), "Velocity: " + (Math.Round(this.rb.velocity.x * 100) / 100) + ", " + (Math.Round(this.rb.velocity.y * 100) / 100));
    }
}
