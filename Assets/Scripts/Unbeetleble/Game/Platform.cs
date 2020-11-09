using UnityEngine;

namespace Unbeetleble.Game
{
    public class Platform : MonoBehaviour
    {
        [SerializeField]
        private new Collider2D collider;

        public void ActivateCollider()
        {
            this.collider.enabled = true;
        }

        public void DeactivateCollider()
        {
            this.collider.enabled = false;
        }
    }
}