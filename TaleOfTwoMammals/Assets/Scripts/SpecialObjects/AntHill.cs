using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntHill : MonoBehaviour
    
{
    [SerializeField]
    protected AudioSource AntHillDestroyed;
    

    protected virtual void Start()
    {
        AntHillDestroyed = GetComponent<AudioSource>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Tongue"))
        {
            AntHillDestroyed.Play();
            Destroy(gameObject);           
        }
    }
}
