using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DeathPlane : MonoBehaviour
{

    [SerializeField] 
    private LevelManager LM;


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
            LM.RespawnArmadillo();
        }
        else if(go.GetComponent<AnteaterController>() != null) //if the object is the anteater
        {
            LM.RespawnAnteater();
        }
    }
}
