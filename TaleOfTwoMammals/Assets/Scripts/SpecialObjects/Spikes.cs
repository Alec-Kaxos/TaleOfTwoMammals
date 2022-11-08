using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    [SerializeField]
    private LevelManager levelManager;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player" )
        {
            ArmadilloController armadillo = other.GetComponent<ArmadilloController>();
            if (armadillo != null && armadillo.IsInBallForm())
            {
                return;
            }

            other.GetComponent<PlayerController>().OnDeath();
        }
    }
}
