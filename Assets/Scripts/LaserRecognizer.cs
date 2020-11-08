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
        if (other.gameObject.GetComponent<LaserCollider>() != null)
        {
            this.entity.ReceiveDamage(1);
        }
    }
}