using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float health;

    public void OnDamageReceive(float damage)
    {
        this.health -= damage;
        Debug.Log("Enemy got " + damage + " damage, health now: " + this.health);
    }
    
    
}
