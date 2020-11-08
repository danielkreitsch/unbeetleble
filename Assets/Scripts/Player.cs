using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameController GameController;
    
    public int health;

    private bool poisonTrigger = false;
    private float poisonCheckTimer = 0;

    public void OnDamageReceive(int damage)
    {
        this.health = Math.Max(0, this.health - damage);
        Debug.Log("Player got " + damage + " damage, health now: " + this.health);
    }

    void Start()
    {
    }

    void Update()
    {
        this.poisonCheckTimer += Time.deltaTime;
        if (this.poisonCheckTimer >= 1)
        {
            this.poisonCheckTimer = 0;
            if (this.poisonTrigger)
            {
                this.poisonTrigger = false;
                this.OnDamageReceive(1);
                
            }
        }
    }

    public void OnPoisonCollision()
    {
        this.poisonTrigger = true;
    }
}