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
        if (lastVelocityY < 0 && RB.velocity.y - lastVelocityY > VelocityThreshold)
        {
            Landed(RB.velocity.y - lastVelocityY);
        }
        lastVelocityY = RB.velocity.y;
    }

    private void Landed(float speed)
    {
        Debug.Log(speed);
        //Instantiate(LandParticle, transform.position, Quaternion.identity);
        ParticleMaster.SpawnParticle(LandParticle, transform.position + LandParticleDisplacement);
    }
}
