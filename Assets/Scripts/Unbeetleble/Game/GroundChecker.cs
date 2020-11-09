using UnityEngine;
using UnityEngine.Events;

namespace Unbeetleble.Game
{
    public class GroundChecker : MonoBehaviour
    {
        /**
     * A position marking where to check if the player is grounded.
     */
        [SerializeField]
        private Transform pivot = null;

        [SerializeField]
        private float pivotOffsetY = 0;

        [SerializeField]
        private float checkRadius = 0;

        /**
     ** A mask determining what is ground to the character.
    */
        [SerializeField]
        private LayerMask groundLayer = default;

        [Header("Events")]
        public UnityEvent OnLandEvent;

        public bool TouchingGround { get; private set; }

        public float TouchingGroundTime { get; private set; }

        public GameObject TouchingGroundObject { get; private set; }

        public bool TouchedGroundBefore { get; private set; }

        private Vector3 positionBefore;

        public void FixedUpdate()
        {
            this.TouchedGroundBefore = this.TouchingGround;
            this.TouchingGround = false;
            this.TouchingGroundObject = null;

            Collider2D[] colliders = Physics2D.OverlapCircleAll(new Vector2(this.pivot.position.x, this.pivot.position.y) + Vector2.up * this.pivotOffsetY, this.checkRadius, this.groundLayer);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject != this.gameObject)
                {
                    this.TouchingGround = true;
                    this.TouchingGroundObject = colliders[i].gameObject;
                    this.TouchingGroundTime += Time.deltaTime;
                    if (!this.TouchedGroundBefore && this.transform.position.y < this.positionBefore.y)
                    {
                        this.OnLandEvent.Invoke();
                        break;
                    }
                }
            }

            if (!this.TouchingGround)
            {
                this.TouchingGroundTime = 0;
            }

            this.positionBefore = this.transform.position;
        }
    }
}