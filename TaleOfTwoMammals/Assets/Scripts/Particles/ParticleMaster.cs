using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ParticleMaster : MonoBehaviour
{

    
    public static GameObject SpawnParticle(GameObject particle, Vector3 position, Transform parent = null, bool play = true)
    {
        if (particle)
        {
            GameObject newParticle = Instantiate(particle, position, particle.transform.rotation, parent);
            if(play) newParticle.GetComponent<ParticleController>().Play();
            return newParticle;
        }

        return null;
    }
    
    public static GameObject SpawnParticle(GameObject particle, Vector3 position, Color color1, Color color2, Transform parent = null, bool play = true)
    {

        if (particle)
        {
            GameObject newParticle = Instantiate(particle, position, particle.transform.rotation, parent);
            ParticleSystem particleSystem = newParticle.GetComponent<ParticleSystem>();
            ParticleSystem.MainModule mainModule = particleSystem.main;
            
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(color1, 0.0f), new GradientColorKey(color2, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) }
            );
            Debug.Log(gradient);
            mainModule.startColor = new ParticleSystem.MinMaxGradient(gradient);
            if (play) newParticle.GetComponent<ParticleController>().Play();

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
