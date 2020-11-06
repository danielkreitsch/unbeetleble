using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Border : MonoBehaviour
{
    [SerializeField]
    private Transform top;

    [SerializeField]
    private Transform bottom;

    [SerializeField]
    private Transform left;

    [SerializeField]
    private Transform right;

    [SerializeField]
    private float holeSpacing;

    public List<Vector2> holeEntries = new List<Vector2>();

    void Awake()
    {
        for (float x = -(this.top.localScale.x / 2 - 0.5f); x < this.top.localScale.x / 2 - 0.5f; x += this.holeSpacing)
        {
            this.holeEntries.Add(this.top.position + Vector3.right * x + Vector3.down * 0.5f);
        }
        
        for (float x = -(this.bottom.localScale.x / 2 - 0.5f); x < this.bottom.localScale.x / 2 - 0.5f; x += this.holeSpacing)
        {
            this.holeEntries.Add(this.bottom.position + Vector3.right * x + Vector3.up * 0.5f);
        }
        
        for (float y = -(this.left.localScale.y / 2 - 0.5f); y < this.left.localScale.y / 2 - 0.5f; y += this.holeSpacing)
        {
            this.holeEntries.Add(this.left.position + Vector3.up * y + Vector3.right * 0.5f);
        }
        
        for (float y = -(this.right.localScale.y / 2 - 0.5f); y < this.right.localScale.y / 2 - 0.5f; y += this.holeSpacing)
        {
            this.holeEntries.Add(this.right.position + Vector3.up * y + Vector3.left * 0.5f);
        }
        
        // TODO: Das gleiche für links und rechts
    }

    void Update()
    {
    }
}