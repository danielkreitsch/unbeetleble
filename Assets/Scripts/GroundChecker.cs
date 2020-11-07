using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GroundChecker : MonoBehaviour
{
    /**
         * A position marking where to check if the player is grounded.
         */
    [SerializeField]
    private Transform pivot = null;

    [SerializeField]
    private float offsetY = 0;

    [SerializeField]
    private float radius = 0;

    /**
     ** A mask determining what is ground to the character.
    */
    [SerializeField]
    private LayerMask layer = default;

    [Header("Events")]
    public UnityEvent OnLandEvent;

    public bool touchingGround;

    public float touchingGroundTime;

    public GameObject touchingObj;

    private bool touchingGroundBefore;

    private Vector3 positionBefore;

    public bool TouchingGroundBefore => this.touchingGroundBefore;

    public void FixedUpdate()
    {
        this.touchingGroundBefore = this.touchingGround;
        this.touchingGround = false;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(new Vector2(this.pivot.position.x, this.pivot.position.y) + Vector2.up * this.offsetY, this.radius, this.layer);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != this.gameObject)
            {
                this.touchingGround = true;
                this.touchingObj = colliders[i].gameObject;
                this.touchingGroundTime += Time.deltaTime;
                if (!this.touchingGroundBefore && this.transform.position.y < this.positionBefore.y)
                {
                    this.OnLandEvent.Invoke();
                    break;
                }
            }
        }

        if (!this.touchingGround)
        {
            this.touchingGroundTime = 0;
        }

        this.positionBefore = this.transform.position;
    }
}