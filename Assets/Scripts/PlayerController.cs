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
    private Collider2D collider;

    [SerializeField]
    private GroundChecker groundChecker = default;

    [SerializeField]
    private LayerMask borderLayer;

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

    [Header("Air Movement")]
    [SerializeField]
    private float extraAirActions = 0;

    [Header("Jumping")]
    [SerializeField]
    private float jumpVelocity;

    [SerializeField]
    private float jumpInputAhead;

    [SerializeField]
    private float jumpDelayAfterGrounded;

    [SerializeField]
    private float jumpReleaseGravityMultiplier = 1;

    [Header("Dashing")]
    [SerializeField]
    private float dashDistance;

    [SerializeField]
    private float postDashVelocity;

    [SerializeField]
    private float dashTime;

    [SerializeField]
    private string dashEaseType;

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
    private bool dashInput = false;

    private bool ignorePlatforms = false;
    private int extraAirActionsUsed = 0;

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

        if (Input.GetButtonDown("Dash"))
        {
            this.dashInput = true;
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
            bool canJumpInAir = this.extraAirActionsUsed < this.extraAirActions;
            if (canJumpFromGround || canJumpInAir)
            {
                if (canJumpFromGround)
                {
                    this.extraAirActionsUsed = 0;
                }
                else
                {
                    this.extraAirActionsUsed++;
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
                this.rb.AddForce(Vector2.up * this.jumpVelocity, ForceMode2D.Impulse);
            }
        }

        // Dashing
        if (this.dashInput)
        {
            bool canDashFromGround = this.groundChecker.touchingGroundTime > this.jumpDelayAfterGrounded && this.rb.velocity.y < 0.00001f;
            bool canDashInAir = this.extraAirActionsUsed < this.extraAirActions;

            if (canDashFromGround || canDashInAir)
            {
                if (canDashFromGround)
                {
                    this.extraAirActionsUsed = 0;
                }
                else
                {
                    this.extraAirActionsUsed++;
                }

                this.StartCoroutine(this.CDash());
            }
        }
        this.dashInput = false;

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

    private IEnumerator CDash()
    {
        var myPos = this.transform.position;
        var mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
        mousePos.z = myPos.z;

        var dashDir = (mousePos - myPos).normalized;

        var startPosition = this.transform.position;
        var targetPosition = this.transform.position;

        bool canDash = false;

        var raycast = Physics2D.Raycast(this.transform.position, dashDir, this.dashDistance + 0.25f, this.borderLayer);
        if (raycast.collider != null)
        {
            float distance = Vector2.Distance(this.transform.position, raycast.point);
            if (distance >= 0.5f * this.dashDistance)
            {
                canDash = true;
                var hit = new Vector3(raycast.point.x, raycast.point.y, 0);
                targetPosition = hit - 0.25f * dashDir;
            }
        }
        else
        {
            canDash = true;
            targetPosition = this.transform.position + this.dashDistance * dashDir;
        }

        if (canDash)
        {
            var velocityBeforeDash = this.rb.velocity;
            this.rb.gravityScale = 0;

            foreach (Platform platform in Object.FindObjectsOfType<Platform>())
            {
                platform.DeactivateCollider();
            }

            iTween.MoveTo(this.gameObject, iTween.Hash("position", targetPosition, "time", this.dashTime, "easeType", this.dashEaseType));
            yield return new WaitForSeconds(this.dashTime);

            this.transform.position = targetPosition;

            foreach (Platform platform in Object.FindObjectsOfType<Platform>())
            {
                platform.ActivateCollider();
            }

            this.rb.gravityScale = 1;

            if (this.groundChecker.touchingGround)
            {
                this.rb.velocity = new Vector2(0, velocityBeforeDash.y);
            }
            else
            {
                this.rb.velocity = this.postDashVelocity * dashDir;
            }
        }
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