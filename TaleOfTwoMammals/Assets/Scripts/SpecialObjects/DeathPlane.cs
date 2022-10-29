using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DeathPlane : MonoBehaviour
{

    [SerializeField]
    private GameObject AnteaterSpawn;
    [SerializeField]
    private GameObject ArmadilloSpawn;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Used for when an object enters this deathplane, likely a player
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject go = collision.gameObject;

        //IF the object is the armadillo
        if(go.GetComponent<ArmadilloController>() != null)
        {
            go.transform.position = ArmadilloSpawn.transform.position;
        }
        else if(go.GetComponent<AnteaterController>() != null) //if the object is the anteater
        {
            go.transform.position = AnteaterSpawn.transform.position;
        }
    }
}
