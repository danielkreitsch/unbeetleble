using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LivingEntity : MonoBehaviour
{
    [SerializeField]
    private UnityEvent<int> damageReceiveEvent;
    
    public void ReceiveDamage(int damage)
    {
        this.damageReceiveEvent.Invoke(damage);
    }
}
