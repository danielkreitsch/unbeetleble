using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D rb = default;

    [SerializeField]
    private Transform model = default;
    
    [SerializeField]
    private GroundChecker groundChecker = default;
    
    [Header("Movement")]
    [SerializeField]
    private float moveSpeed;

    [SerializeField]
    private float moveSpeedAiring;
    
    [FormerlySerializedAs("moveSpeedAiring")]
    [SerializeField]
    private float slowDownFactorAiring;

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

    private Vector2 velocity = new Vector2();
    
    private float horizontalInput = 0;
    private bool jumpInput = false;
    private bool dropInput = false;
    
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
        
        if (Input.GetButtonDown("Drop"))
        {
            this.dropInput = true;
        }

        if (this.groundChecker.touchingGround)
        {
            if (this.horizontalInput > 0.01f)
            {
                this.model.eulerAngles = new Vector3(0, 90, 0);
            }
            else if (this.horizontalInput < -0.01f)
            {
                this.model.eulerAngles = new Vector3(0, 270, 0);
            }
        }
        else
        {
            if (this.rb.velocity.x > 0.1f)
            {
                this.model.eulerAngles = new Vector3(0, 90, 0);
            }
            else if (this.rb.velocity.x < -0.1f)
            {
                this.model.eulerAngles = new Vector3(0, 270, 0);
            }
        }
    }

    void FixedUpdate()
    {
        var targetVel = new Vector2(this.rb.velocity.x, this.rb.velocity.y);
        
        if (this.groundChecker.touchingGround)
        {
            targetVel.x = this.horizontalInput * this.moveSpeed;
        }
        else
        {
            if (this.horizontalInput > 0.01f)
            {
                if (targetVel.x < -0.1f)
                {
                    targetVel.x *= this.slowDownFactorAiring;
                }
                else if (targetVel.x < this.horizontalInput * this.moveSpeedAiring)
                {
                    targetVel.x = this.horizontalInput * this.moveSpeedAiring;
                }
            }
            else if (this.horizontalInput < -0.01f)
            {
                if (targetVel.x > 0.1f)
                {
                    targetVel.x *= this.slowDownFactorAiring;
                }
                else if (targetVel.x > this.horizontalInput * this.moveSpeedAiring)
                {
                    targetVel.x = this.horizontalInput * this.moveSpeedAiring;
                }
            }
        }

        if (!this.jumpInput && this.dropInput)
        {
            if (this.groundChecker.touchingGround)
            {
                this.DropFromPlatfrom();
            }
        }
        this.dropInput = false;

        // Jumping
        if (this.jumpInput)
        {
            if (this.groundChecker.touchingGround && this.rb.velocity.y < 0.01f)
            {
                if (this.horizontalInput > 0.01f)
                {
                    targetVel.x = 3;
                }
                else if (this.horizontalInput < -0.01f)
                {
                    targetVel.x = -3;
                }
                this.rb.velocity = new Vector2(this.rb.velocity.x, 0);
                this.rb.AddForce(Vector2.up * this.jumpVelocity, ForceMode2D.Impulse);
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

        //this.rb.velocity = targetVel;
        this.rb.velocity = Vector2.SmoothDamp(this.rb.velocity, targetVel, ref this.velocity, this.movementSmoothing);
    }

    void OnGUI()
    {
        //GUI.Box(new Rect(5, 5, 400, 100), "Velocity: " + (Math.Round(this.rb.velocity.x * 100) / 100) + ", " + (Math.Round(this.rb.velocity.y * 100) / 100));
    }

    public void DropFromPlatfrom()
    {
        this.StartCoroutine(this.CDropFromPlatform());
    }

    private IEnumerator CDropFromPlatform()
    {
        GameObject groundObj = this.groundChecker.touchingObj;
        Platform platform = groundObj.GetComponent<Platform>();
        if (platform != null)
        {
            platform.DeactivateCollider();
            yield return new WaitForSeconds(0.5f);
            platform.ActivateCollider();
        }
    }
}
