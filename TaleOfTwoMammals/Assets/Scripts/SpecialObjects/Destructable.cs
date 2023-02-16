using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Destructable : MonoBehaviour
{
    [SerializeField] private bool destroyEntireBlock = false;
    [SerializeField] private string destroyParticleName = "Break Block";
    private GameObject destroyParticles;
    private Tilemap destructableTilemap;
    private ArmadilloController Armadillo;

    private void Start()
    {
        destructableTilemap = GetComponent<Tilemap>();
        destroyParticles = Resources.Load<GameObject>("Particles/" + destroyParticleName);
        Debug.Log(destroyParticles);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Armadillo = collision.gameObject.GetComponent<ArmadilloController>();

        if (Armadillo != null)
        {
            if(Armadillo.IsPounding())
            {
                Vector3 hitPosition = Vector3.zero;
                foreach (ContactPoint2D hit in collision.contacts)
                {
                    hitPosition.x = hit.point.x - 0.01f * hit.normal.x;
                    hitPosition.y = hit.point.y - 0.01f * hit.normal.y;
                    ParticleMaster.SpawnParticle(destroyParticles, hit.point);
                    if (destroyEntireBlock == true)
                    {
                        Destroy(this.gameObject);
                    }
                    else
                    {
                        destructableTilemap.SetTile(destructableTilemap.WorldToCell(hitPosition), null);
                    }
                }

            }
            
        }
    }

}
