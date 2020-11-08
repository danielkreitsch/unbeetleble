using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

public class PoisonBullet : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D rb;

    [SerializeField]
    private ParticleSystem ps;
    
    private Player player;

    [SerializeField]
    private float speed;

    private ParticleSystem.Particle[] particles;

    private float checkTimer = 0;

    // Start is called before the first frame update
    void Start()
    {
        this.player = Object.FindObjectOfType<Player>().GetComponent<Player>();

        this.particles = new ParticleSystem.Particle[this.ps.main.maxParticles];
    }

    // Update is called once per frame
    void Update()
    {
        if (!this.ps.isEmitting)
        {
            this.Invoke(() =>
            {
                Object.Destroy(this);
            }, 3f);
        }
        
        this.checkTimer += Time.deltaTime;
        if (this.checkTimer >= 0.1f)
        {
            this.checkTimer = 0;
            this.ps.GetParticles(this.particles);

            bool hit = false;
            
            foreach (ParticleSystem.Particle particle in this.particles)
            {
                if (particle.GetCurrentColor(this.ps).a > 100)
                {
                    float distance = Vector2.Distance(new Vector2(this.transform.position.x + particle.position.x, this.transform.position.y + particle.position.y), new Vector2(this.player.transform.position.x, this.player.transform.position.y));
                    if (distance < 0.3f)
                    {
                        this.player.OnPoisonCollision();
                        break;
                    }
                }
            }
        }
    }

    /*private void OnParticleCollision(GameObject other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            other.GetComponent<LivingEntity>().ReceiveDamage(1);
        }
    }*/

    public void ShootTo(Vector3 targetPosition)
    {
        this.rb.velocity = (targetPosition - this.transform.position).normalized * this.speed;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.transform.position.y < this.transform.position.y && this.rb.velocity.y < 1)
        {
            this.rb.bodyType = RigidbodyType2D.Static;
            this.rb.isKinematic = true;
        }
    }
}
