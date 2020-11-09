using UnityEngine;

namespace Unbeetleble.Game
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField]
        private float health;

        public float Health
        {
            get => this.health;
            private set => this.health = value;
        }

        public void OnDamageReceive(float damage)
        {
            this.Health -= damage;
        }
    }
}