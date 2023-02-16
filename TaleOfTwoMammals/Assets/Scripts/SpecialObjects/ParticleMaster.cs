using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ParticleMaster : MonoBehaviour
{


    public static void SpawnParticle(GameObject particle, Vector3 position)
    {
        if (particle)
        {
            GameObject newParticle = Instantiate(particle, position, particle.transform.rotation, null);
            //newParticle.GetComponent<ParticleController>().Play();
        }
    }
    
    public static void SpawnParticle(GameObject particle, Vector3 position, Transform parent = null)
    {
        if (particle)
        {
            GameObject newParticle = Instantiate(particle, position, particle.transform.rotation, parent);
            //newParticle.GetComponent<ParticleController>().Play();
        }
    }
    
    public static void SpawnParticle(GameObject particle, Vector3 position, Transform parent = null, Color color1 = default, Color color2 = default)
    {

        if (particle)
        {
            GameObject newParticle = Instantiate(particle, position, particle.transform.rotation, parent);
            ParticleSystem particleSystem = newParticle.GetComponent<ParticleSystem>();
            ParticleSystem.MainModule mainModule = particleSystem.main;
            ParticleSystem.ColorOverLifetimeModule colorOverLifetimeModule = particleSystem.colorOverLifetime;
            ParticleSystem.MinMaxGradient minMaxGradient = colorOverLifetimeModule.color;
            Gradient gradient = minMaxGradient.gradient;
            GradientColorKey[] colorKeys = gradient.colorKeys;
            GradientAlphaKey[] alphaKeys = gradient.alphaKeys;
            if (color1 != null)
            {
                colorKeys[0].color = color1;
            }
            if (color2 != null)
            {
                colorKeys[1].color = color2;
            }
            //newParticle.GetComponent<ParticleController>().Play();
        }

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
