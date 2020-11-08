using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserRecognizer : MonoBehaviour
{
    [SerializeField]
    private LivingEntity entity;

    private float cooldown = 0;
    
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (this.cooldown > 0)
        {
            this.cooldown -= Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var laserCollider = other.gameObject.GetComponent<LaserCollider>();
        if (laserCollider != null && this.cooldown <= 0)
        {
            this.entity.ReceiveDamage(1);
            this.cooldown = 1;
        }
    }
}