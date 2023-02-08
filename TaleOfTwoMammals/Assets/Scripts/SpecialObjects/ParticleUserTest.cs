using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleUserTest : MonoBehaviour
{
    public GameObject particle;

    public bool doParticleThing = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (doParticleThing)
        {
            doParticleThing = false;
            ParticleMaster.SpawnParticle(particle, transform.position);
        }
    }
}
