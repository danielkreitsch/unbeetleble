using UnityEngine;
using UnityEngine.Events;

namespace Unbeetleble.Game
{
    public class LivingEntity : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent<float> damageReceiveEvent;

        public void ReceiveDamage(float damage)
        {
            this.damageReceiveEvent.Invoke(damage);
        }
    }
}