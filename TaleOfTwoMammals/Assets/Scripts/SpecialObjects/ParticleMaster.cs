using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleMaster : MonoBehaviour
{

    public static void SpawnParticle(GameObject particle, Vector3 position, Quaternion rotation = default, Transform parent = null)
    {
        GameObject newParticle = Instantiate(particle, position, rotation, parent);
        //newParticle.GetComponent<ParticleController>().Play();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
