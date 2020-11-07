using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class HoleEntry
{
    public Vector2 position;
    public Facing facing;

    public HoleEntry(Vector2 position, Facing facing)
    {
        this.position = position;
        this.facing = facing;
    }
}