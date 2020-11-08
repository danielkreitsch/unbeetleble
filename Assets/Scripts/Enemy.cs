using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health;

    public void OnDamageReceive(int damage)
    {
        this.health -= damage;
        Debug.Log("Enemy got " + damage + " damage, health now: " + this.health);
    }
    
    
}
