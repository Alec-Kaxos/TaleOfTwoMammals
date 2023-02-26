using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveParticles : MonoBehaviour
{
    private Rigidbody2D RB;

    [SerializeField]
    private GameObject MoveParticle;
    [SerializeField]
    private Vector3 MoveParticleDisplacement = new Vector3(0, -.5f, 0);

    [SerializeField]
    private float VelocityThreshold = 1f;

    private GameObject LeftParticles;
    private GameObject RightParticles;

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
        if (Mathf.Abs(RB.velocity.x) > VelocityThreshold)
        {
            if (RB.velocity.x > 0)
            {//Moving right
                if (RightParticles == null)
                {
                    RightParticles = ParticleMaster.SpawnParticle(MoveParticle, transform.position + MoveParticleDisplacement, gameObject.transform);
                    RightParticles.transform.localScale = new Vector3(-1, 1, 1);
                }
                
            }
            else
            {//Moving left
                if (LeftParticles == null)
                {
                    LeftParticles = ParticleMaster.SpawnParticle(MoveParticle, transform.position + MoveParticleDisplacement, gameObject.transform);
                }

            }    
        }
        else
        {//Stop any current particles
            if(LeftParticles)
            {
                LeftParticles.GetComponent<ParticleSystem>().Stop();
                LeftParticles = null;
            }
            if (RightParticles)
            {
                RightParticles.GetComponent<ParticleSystem>().Stop();
                RightParticles = null;
            }
        }
    }
}
