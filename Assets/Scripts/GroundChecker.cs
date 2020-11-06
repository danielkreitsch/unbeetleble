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
         * A mask determining what is ground to the character.
         */
    [SerializeField]
    private LayerMask layer = default;

    [Header("Events")]
    public UnityEvent OnLandEvent;

    private bool touchingGround;

    private bool touchingGroundBefore;

    private Vector3 positionBefore;

    public bool TouchingGround => this.touchingGround;

    public bool TouchingGroundBefore => this.touchingGroundBefore;

    public void FixedUpdate()
    {
        this.touchingGroundBefore = this.touchingGround;
        this.touchingGround = false;

        Collider[] colliders = Physics.OverlapSphere(this.pivot.position + Vector3.up * this.offsetY, this.radius, this.layer);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != this.gameObject)
            {
                this.touchingGround = true;
                if (!this.touchingGroundBefore && this.transform.position.y < this.positionBefore.y)
                {
                    this.OnLandEvent.Invoke();
                    break;
                }
            }
        }

        this.positionBefore = this.transform.position;
    }
}
