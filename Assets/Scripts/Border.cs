using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
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
    private float minDistanceToCorner;
    
    [SerializeField]
    private float holeSpacing;

    public List<HoleEntry> holeEntries = new List<HoleEntry>();

    void Awake()
    {
        for (float x = -(this.top.localScale.x / 2 - 0.5f - this.minDistanceToCorner); x < this.top.localScale.x / 2 - 0.5f - this.minDistanceToCorner; x += this.holeSpacing)
        {
            this.holeEntries.Add(new HoleEntry(this.top.position + Vector3.right * x + Vector3.down * 0.5f, Facing.Bottom));
        }
        
        for (float x = -(this.bottom.localScale.x / 2 - 0.5f - this.minDistanceToCorner); x < this.bottom.localScale.x / 2 - 0.5f - this.minDistanceToCorner; x += this.holeSpacing)
        {
            this.holeEntries.Add(new HoleEntry(this.bottom.position + Vector3.right * x + Vector3.up * 0.5f, Facing.Top));
        }
        
        for (float y = -(this.left.localScale.y / 2 - 0.5f - this.minDistanceToCorner); y < this.left.localScale.y / 2 - 0.5f - this.minDistanceToCorner; y += this.holeSpacing)
        {
            this.holeEntries.Add(new HoleEntry(this.left.position + Vector3.up * y + Vector3.right * 0.5f, Facing.Right));
        }
        
        for (float y = -(this.right.localScale.y / 2 - 0.5f - this.minDistanceToCorner); y < this.right.localScale.y / 2 - 0.5f - this.minDistanceToCorner; y += this.holeSpacing)
        {
            this.holeEntries.Add(new HoleEntry(this.right.position + Vector3.up * y + Vector3.left * 0.5f, Facing.Left));
        }
    }
}