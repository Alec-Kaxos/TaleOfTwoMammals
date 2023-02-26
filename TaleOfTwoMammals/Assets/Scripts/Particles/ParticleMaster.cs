using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ParticleMaster : MonoBehaviour
{


    public static GameObject SpawnParticle(GameObject particle, Vector3 position)
    {
        if (particle)
        {
            GameObject newParticle = Instantiate(particle, position, particle.transform.rotation, null);
            newParticle.GetComponent<ParticleController>().Play();
            return newParticle;
        }

        return null;
    }
    
    public static GameObject SpawnParticle(GameObject particle, Vector3 position, Transform parent = null)
    {
        if (particle)
        {
            GameObject newParticle = Instantiate(particle, position, particle.transform.rotation, parent);
            newParticle.GetComponent<ParticleController>().Play();
            return newParticle;
        }

        return null;
    }
    
    public static GameObject SpawnParticle(GameObject particle, Vector3 position, Transform parent = null, Color color1 = default, Color color2 = default)
    {

        if (particle)
        {
            GameObject newParticle = Instantiate(particle, position, particle.transform.rotation, parent);
            ParticleSystem particleSystem = newParticle.GetComponent<ParticleSystem>();
            //particleSystem.startColor
            ParticleSystem.MainModule mainModule = particleSystem.main;
            //mainModule.startColor;
            //ParticleSystem.ColorOverLifetimeModule colorOverLifetimeModule = particleSystem.colorOverLifetime;
            //ParticleSystem.MinMaxGradient minMaxGradient = mainModule.startColor;
            //Gradient gradient = minMaxGradient.gradient;
            Gradient gradient = new Gradient();
            //GradientColorKey[] colorKeys = gradient.colorKeys;
            //GradientAlphaKey[] alphaKeys = gradient.alphaKeys;
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(color1, 0.0f), new GradientColorKey(color2, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) }
            );
            Debug.Log(gradient);
            mainModule.startColor = new ParticleSystem.MinMaxGradient(gradient);
            newParticle.GetComponent<ParticleController>().Play();

            return newParticle;
        }

        return null;
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
