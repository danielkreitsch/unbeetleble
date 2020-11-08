using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserRecognizer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collision other)
    {
        Debug.Log("Collision with " + other.gameObject.name);
        if (other.gameObject.GetComponent<LaserCollider>() != null)
        {
            Debug.Log("LASER CONTACT");
        }
    }
}
