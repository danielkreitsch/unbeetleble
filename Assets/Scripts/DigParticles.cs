using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DigParticles : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem particleSystem;

    private float intensity = 0;

    private float timer = 0;

    private bool emitting = true;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (this.emitting)
        {
            this.timer += this.intensity * Time.deltaTime;
            if (this.timer > 0.01f)
            {
                this.timer -= 0.01f;
                this.particleSystem.Emit(1);
            }
        }
    }
    
    public void SetIntensity(float intensity)
    {
        this.intensity = intensity;
    }

    public void Remove()
    {
        this.StartCoroutine(this.CRemove());
    }

    private IEnumerator CRemove()
    {
        this.emitting = false;
        yield return new WaitForSeconds(2);
        Object.Destroy(this.gameObject);
    }
}
