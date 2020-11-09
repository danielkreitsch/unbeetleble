using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private GameObject trailPrefab;
    
    [SerializeField]
    private Player player;

    [SerializeField]
    private Rigidbody2D rb = default;

    [SerializeField]
    private Transform model = default;

    [SerializeField]
    private Transform laserOrigin;

    [SerializeField]
    private Collider2D collider;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private GroundChecker groundChecker = default;

    [SerializeField]
    private LayerMask borderLayer;

    [SerializeField]
    private GameObject laserPrefab;

    [SerializeField]
    private MusicController musicController;

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

    [Header("Attacking")]
    [SerializeField]
    private float laserPrepareTime;

    [SerializeField]
    private float laserSoundDelay;
    
    [SerializeField]
    private float laserBuildupTime = 1.2f;

    [SerializeField]
    private float laserDisappearTime = 0.4f;

    [Header("Gravity")]
    [SerializeField]
    private float fallGravityMultiplier = 1;

    [Range(0, 25)]
    [SerializeField]
    private float gravityLimit = 0;

    [SerializeField]
    private GameObject testPrefab;

    [Header("Misc")]
    [SerializeField]
    private float knockbackTime;

    private Vector2 velocity = new Vector2();

    private float horizontalInput = 0;
    private float jumpInputTimeout = 0;
    private bool dropInput = false;
    private bool attackInput = false;
    private bool dashInput = false;

    private bool attacking;
    private bool lookRight = true;
    private int extraAirActionsUsed = 0;
    private bool died = false;
    private bool outOfControl;

    // Start is called before the first frame update
    void Start()
    {
        iTween.RotateTo(this.model.gameObject, new Vector3(0, 90 + 17.5f, 0), 0.2f);
    }

    // Update is called once per frame
    void Update()
    {
        if (this.player.health <= 0)
        {
            if (!this.died)
            {
                this.died = true;
                this.StartCoroutine(this.CDeath());
            }

            return;
        }

        if (this.died)
        {
            this.animator.SetFloat("WalkSpeed", 0);
            return;
        }

        this.horizontalInput = Input.GetAxisRaw("Horizontal");

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

        if (Input.GetButton("Attack"))
        {
            this.attackInput = true;
        }

        if (Input.GetButtonDown("Dash"))
        {
            this.dashInput = true;
        }

        if (!this.attacking && !this.outOfControl)
        {
            if (this.groundChecker.touchingGround)
            {
                if (!this.lookRight && this.horizontalInput > 0.01f)
                {
                    this.lookRight = true;
                    iTween.RotateTo(this.model.gameObject, new Vector3(0, 90 + 17.5f, 0), 0.2f);
                }
                else if (this.lookRight && this.horizontalInput < -0.01f)
                {
                    this.lookRight = false;
                    iTween.RotateTo(this.model.gameObject, new Vector3(0, 270 - 17.5f, 0), 0.2f);
                }
            }
            else
            {
                if (!this.lookRight && this.rb.velocity.x > 0.01f)
                {
                    this.lookRight = true;
                    iTween.RotateTo(this.model.gameObject, new Vector3(0, 90 + 17.5f, 0), 0.2f);
                }
                else if (this.lookRight && this.rb.velocity.x < -0.01f)
                {
                    this.lookRight = false;
                    iTween.RotateTo(this.model.gameObject, new Vector3(0, 270 - 17.5f, 0), 0.2f);
                }
            }
        }

        this.animator.SetFloat("WalkSpeed", Mathf.Abs(this.rb.velocity.x));
        this.animator.SetBool("TouchingGround", this.groundChecker.touchingGround);
        this.animator.SetBool("Attacking", this.attacking);

        if (!this.attacking && !this.outOfControl)
        {
            if (this.attackInput)
            {
                this.animator.SetBool("Attacking", true);
                this.animator.Play("Attack");
                this.StartCoroutine(this.CAttack());
            }
        }
    }

    void FixedUpdate()
    {
        if (this.died)
        {
            return;
        }

        var targetVel = new Vector2(this.rb.velocity.x, this.rb.velocity.y);

        if (!this.attacking && !this.outOfControl)
        {
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
            
            if (this.dropInput && this.groundChecker.touchingGround)
            {
                this.DropFromPlatfrom();
            }

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

                    if (canJumpFromGround)
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
                    else
                    {
                        var direction = Vector3.up;
                        if (this.horizontalInput > 0.01f)
                        {
                            direction = new Vector3(1, 1, 0);
                        }
                        else if (this.horizontalInput < -0.01f)
                        {
                            direction = new Vector3(-1, 1, 0);
                        }
                        this.StartCoroutine(this.CDash(direction));
                    }
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

                    var myPos = this.transform.position;
                    var mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
                    mousePos.z = myPos.z;
                    var direction = (mousePos - myPos).normalized;
                    this.StartCoroutine(this.CDash(direction));
                }
            }
        }

        this.attackInput = false;
        this.dropInput = false;
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
        if (!this.attacking && !this.outOfControl)
        {
            this.rb.velocity = Vector2.SmoothDamp(this.rb.velocity, targetVel, ref this.velocity, this.movementSmoothing);
        }
    }

    public void OnDamageReceive(float damage)
    {
        if (damage > 1)
        {
            if (this.attacking)
            {
                this.attacking = false;
            }
            this.StartCoroutine(this.CKnockback());
        }
    }

    private IEnumerator CAttack()
    {
        this.attacking = true;
        this.rb.velocity = Vector2.zero;
        this.rb.bodyType = RigidbodyType2D.Static;

        var myPos = this.transform.position;
        var mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
        mousePos.z = myPos.z;
        
        this.model.transform.LookAt(mousePos);
        this.model.transform.eulerAngles = new Vector3(0,    this.model.transform.eulerAngles.y,    this.model.transform.eulerAngles.z);

        yield return new WaitForSeconds(this.laserSoundDelay);
        this.musicController.PlayLaser();
        yield return new WaitForSeconds(this.laserPrepareTime - this.laserSoundDelay);

        GameObject laserObj = Object.Instantiate(this.laserPrefab, this.laserOrigin.position, Quaternion.identity);
        Laser laser = laserObj.GetComponent<Laser>();

        /* var laserDir = (mousePos - myPos).normalized;
         float angle = Quaternion.Angle(Quaternion.Euler(laserDir), Quaternion.Euler(0, 0, 0));
         
         laser.transform.eulerAngles = new Vector3(angle, 90, -90);*/
        laser.transform.LookAt(mousePos);

        laser.Buildup(this.laserBuildupTime);
        this.musicController.PlayThunder();

        for (float time = 0; time < this.laserBuildupTime; time += Time.deltaTime)
        {
            if (!this.attacking)
            {
                this.CancelAttack(laser, true);
                yield break;
            }
            yield return null;
        }

        laser.SetLaserWidth(0.06f);
        laser.Disappear(this.laserDisappearTime);

        for (float time = 0; time < this.laserDisappearTime; time += Time.deltaTime)
        {
            if (!this.attacking)
            {
                this.CancelAttack(laser, false);
                yield break;
            }
            yield return null;
        }

        Object.Destroy(laser.gameObject);
        this.rb.bodyType = RigidbodyType2D.Dynamic;
        this.attacking = false;
    }

    private void CancelAttack(Laser laser, bool cancelSound)
    {
        if (cancelSound)
        {
            this.musicController.CancelLaserAndThunder();
        }
        Object.Destroy(laser.gameObject);
        this.rb.bodyType = RigidbodyType2D.Dynamic;
        this.attacking = false;
    }

    private IEnumerator CKnockback()
    {
        foreach (Platform platform in Object.FindObjectsOfType<Platform>())
        {
            platform.DeactivateCollider();
        }

        this.outOfControl = true;

        this.rb.AddForce(Vector2.up * 0.5f * this.jumpVelocity, ForceMode2D.Impulse);
        yield return new WaitForSeconds(this.knockbackTime);

        this.outOfControl = false;

        foreach (Platform platform in Object.FindObjectsOfType<Platform>())
        {
            platform.ActivateCollider();
        }
        yield return null;
    }

    private IEnumerator CDeath()
    {
        this.rb.constraints = RigidbodyConstraints2D.None;
        this.rb.angularVelocity = 1;
        this.rb.drag = 0.5f;
        this.rb.velocity = 0.5f * this.rb.velocity;
        this.musicController.OnDeath();
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private IEnumerator CDash(Vector3 direction)
    {
        direction = direction.normalized;
        
        var trailObj = Object.Instantiate(this.trailPrefab, this.transform.position, Quaternion.identity);
        trailObj.transform.parent = this.transform;
        
        this.musicController.PlayDash();

        var startPosition = this.transform.position;
        var targetPosition = this.transform.position;

        bool canDash = false;

        var raycast = Physics2D.Raycast(this.transform.position, direction, this.dashDistance + 0.25f, this.borderLayer);
        if (raycast.collider != null)
        {
            float distance = Vector2.Distance(this.transform.position, raycast.point);
            if (distance >= 0.5f * this.dashDistance)
            {
                canDash = true;
                var hit = new Vector3(raycast.point.x, raycast.point.y, 0);
                targetPosition = hit - 0.25f * direction;
            }
        }
        else
        {
            canDash = true;
            targetPosition = this.transform.position + this.dashDistance * direction;
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
                this.rb.velocity = this.postDashVelocity * direction;
            }
        }

        trailObj.transform.parent = null;
        trailObj.GetComponent<Trail>().Disappear();
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