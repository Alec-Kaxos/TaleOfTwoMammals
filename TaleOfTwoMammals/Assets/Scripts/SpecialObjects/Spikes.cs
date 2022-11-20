using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    [SerializeField]
    private LevelManager levelManager;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            ArmadilloController armadillo = collision.gameObject.GetComponent<ArmadilloController>();
            if (armadillo != null && armadillo.IsInBallForm())
            {
                return;
            }

            collision.gameObject.GetComponent<PlayerController>().OnDeath();
        }
    }
}
