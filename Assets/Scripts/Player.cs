using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int health;

    public void OnDamageReceive(int damage)
    {
        this.health -= damage;
        Debug.Log("Player got " + damage + " damage, health now: " + this.health);
    }
    
    void Start()
    {
        
    }
    
    void Update()
    {
        
    }
}
