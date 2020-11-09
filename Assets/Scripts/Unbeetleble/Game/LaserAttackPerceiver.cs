using UnityEngine;

namespace Unbeetleble.Game
{
    public class LaserAttackPerceiver : MonoBehaviour
    {
        [SerializeField]
        private LivingEntity entity;

        private float cooldown = 0;

        void Update()
        {
            if (this.cooldown > 0)
            {
                this.cooldown -= Time.deltaTime;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            var laser = other.gameObject.GetComponentInParent<LaserAttack>();
            if (laser != null && this.cooldown <= 0)
            {
                this.entity.ReceiveDamage(1);
                this.cooldown = 1;
            }
        }
    }
}