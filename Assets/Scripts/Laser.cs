using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Laser : MonoBehaviour
{
    public Transform body;

    public Volume volume;

    public string easeTypeStart;

    public string easeTypeEnd;

    public float bloomIntensity;

    private UnityEngine.Rendering.Universal.Bloom bloom;

    // Start is called before the first frame update
    void Start()
    {
        iTween.ValueTo(this.gameObject, iTween.Hash("from", 0.008f, "to", 0.003f, "onupdate", "SetLaserWidth", "time", 1f, "easeType", this.easeTypeStart));
    }

    public void Disappear()
    {
        iTween.ValueTo(this.gameObject, iTween.Hash("from", 0.06f, "to", 0.0f, "onupdate", "SetLaserWidth", "time", 0.4f, "easeType", this.easeTypeEnd));
    }

    public void SetLaserWidth(float width)
    {
        var scale = this.body.transform.localScale;
        scale.y = width;
        this.body.transform.localScale = scale;

        if (this.volume.profile.TryGet(out this.bloom))
        {
            this.bloom.intensity.Override(width * this.bloomIntensity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
