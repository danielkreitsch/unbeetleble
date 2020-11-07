using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

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
    private float extraJumps = 0;
    
    [SerializeField]
    private float jumpVelocity;

    [SerializeField]
    private float jumpInputAhead;

    [SerializeField]
    private float jumpDelayAfterGrounded;
    
    [SerializeField]
    private float jumpReleaseGravityMultiplier = 1;
    
    [Header("Gravity")]
    [SerializeField]
    private float fallGravityMultiplier = 1;

    [Range(0, 25)]
    [SerializeField]
    private float gravityLimit = 0;

    [SerializeField]
    private GameObject testPrefab;

    private Vector2 velocity = new Vector2();
    
    private float horizontalInput = 0;
    private float jumpInputTimeout = 0;
    private bool dropInput = false;

    private int extraJumpsUsed = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.horizontalInput = Input.GetAxis("Horizontal");

        if (this.jumpInputTimeout > 0)
        {
            this.jumpInputTimeout -= Time.deltaTime;
        }
        
        if (Input.GetButtonDown("Jump"))
        {
            this.jumpInputTimeout = this.jumpInputAhead;
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

        if (this.jumpInputTimeout > 0 && this.dropInput)
        {
            if (this.groundChecker.touchingGround)
            {
                this.DropFromPlatfrom();
            }
        }
        this.dropInput = false;

        // Jumping
        if (this.jumpInputTimeout > 0)
        {
            bool canJumpFromGround = this.groundChecker.touchingGroundTime > this.jumpDelayAfterGrounded && this.rb.velocity.y < 0.00001f;
            bool canJumpInAir = this.extraJumpsUsed < this.extraJumps;
            if (canJumpFromGround || canJumpInAir)
            {
                if (!canJumpFromGround)
                {
                    this.extraJumpsUsed++;
                }
                
                this.jumpInputTimeout = 0;
                
                if (this.horizontalInput > 0.01f)
                {
                    targetVel.x = 3;
                }
                else if (this.horizontalInput < -0.01f)
                {
                    targetVel.x = -3;
                }
                this.rb.velocity = new Vector2(this.rb.velocity.x, 0);

                var myPos = this.transform.position;
                var mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
                Debug.Log(mousePos + " vorher");
                mousePos.z = myPos.z;
                Debug.Log(mousePos + " nachher");

                //Object.Instantiate(this.testPrefab, mousePos, Quaternion.identity);

                var jumpDir = (mousePos - myPos).normalized;
       
                //Debug.Log("Jump Direction: " + jumpDir);

                this.rb.AddForce(jumpDir * this.jumpVelocity, ForceMode2D.Impulse);
            }
        }

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
