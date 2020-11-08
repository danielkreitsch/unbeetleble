using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LivingEntity : MonoBehaviour
{
    [SerializeField]
    private UnityEvent<float> damageReceiveEvent;
    
    public void ReceiveDamage(float damage)
    {
        this.damageReceiveEvent.Invoke(damage);
    }
}
