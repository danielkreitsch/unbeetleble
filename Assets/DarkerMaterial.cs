using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkerMaterial : MonoBehaviour
{
    [SerializeField]
    private float factor;
    
    // Start is called before the first frame update
    void Start()
    {
        foreach (MeshRenderer renderer in this.GetComponentsInChildren<MeshRenderer>())
        {
            renderer.material.color = new Color(renderer.material.color.r * this.factor, renderer.material.color.g * this.factor, renderer.material.color.b * this.factor);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
