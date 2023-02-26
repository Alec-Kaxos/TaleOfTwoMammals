using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LandParticles : MonoBehaviour
{
    
    private Rigidbody2D RB;

    private float lastVelocityY = 0;

    [SerializeField]
    private GameObject LandParticle;
    [SerializeField]
    private Vector3 LandParticleDisplacement = new Vector3(0, -1.5f, 0);

    [SerializeField]
    private float VelocityThreshold = 5f;

    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (lastVelocityY < -.01 && (RB.velocity.y - lastVelocityY > VelocityThreshold))
        {
            Landed(RB.velocity.y - lastVelocityY);
        }
        lastVelocityY = RB.velocity.y;
    }

    private void Landed(float speed)
    {
        Debug.Log(speed);
        //Instantiate(LandParticle, transform.position, Quaternion.identity);
        GameObject pGO = ParticleMaster.SpawnParticle(LandParticle, transform.position + LandParticleDisplacement, play: false);
        ParticleSystem p = pGO.GetComponent<ParticleSystem>();
        ParticleSystem.MainModule main = p.main;

        //Change size of particles based on speed, as a minmaxcurve from speed/10 to speed/5
        main.startSize = new ParticleSystem.MinMaxCurve(speed / 100, speed / 50);

        //Change speed of particles based on speed, as a minmaxcurve from speed/10 to speed/5
        main.startSpeed = new ParticleSystem.MinMaxCurve(speed / 10, speed / 5);

        //change max particles to be speed + 2
        main.maxParticles = (int)speed + 2;
        //change emission rate over time to be speed/2
        ParticleSystem.EmissionModule emission = p.emission;
        emission.rateOverTime = speed / 2;

        p.Play();
    }
}
