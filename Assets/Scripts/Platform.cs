using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    [SerializeField]
    private Collider2D collider;

    public void ActivateCollider()
    {
        this.collider.enabled = true;
    }

    public void DeactivateCollider()
    {
        this.collider.enabled = false;
    }
}