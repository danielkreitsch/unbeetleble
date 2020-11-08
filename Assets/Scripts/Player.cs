using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameController GameController;
    
    public float health;

    private bool poisonTrigger = false;
    private float poisonCheckTimer = 0;
    private float poisonTimeout = 0;

    public void OnDamageReceive(float damage)
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
        if (this.poisonCheckTimer >= 0.1f)
        {
            this.poisonCheckTimer = 0;
            if (this.poisonTrigger)
            {
                this.poisonTimeout = 0.2f;
                this.poisonTrigger = false;
                this.OnDamageReceive(0.1f);
            }
        }
        
        if (this.poisonTimeout > 0)
        {
            this.poisonTimeout -= Time.deltaTime;
            this.GameController.SetVignette(new Color(0, 5, 0));
        }
        else
        {
            this.GameController.SetVignette(new Color(0, 0, 0));
        }
    }

    public void OnPoisonCollision()
    {
        this.poisonTrigger = true;
    }
}