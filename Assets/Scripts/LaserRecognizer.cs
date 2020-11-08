using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserRecognizer : MonoBehaviour
{
    [SerializeField]
    private LivingEntity entity;
    
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        var laserCollider = other.gameObject.GetComponent<LaserCollider>();
        if (laserCollider != null)
        {
            this.entity.ReceiveDamage(1);
        }
    }
}