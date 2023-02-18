using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This component reqiures both animals to be over it's BoxCollider at the same time :)
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class EndingFlag : MonoBehaviour
{
    [SerializeField]
    private LevelManager LM;

    [SerializeField]
    private BoxCollider2D Collider;

    private bool AnteaterOn = false, ArmadilloOn = false;

    protected virtual void Finished()
    {
        LM.LevelWon();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        AnteaterController AntC = collision.gameObject.GetComponent<AnteaterController>();
        if(AntC != null)
        { // Its the anteater!
            AnteaterOn = true;
        }
        ArmadilloController ArmC = collision.gameObject.GetComponent<ArmadilloController>();
        if (ArmC != null)
        { // Its the armadillo!
            ArmadilloOn = true;
        }

        Debug.Log("Anteater "+AnteaterOn+", armadillo " + ArmadilloOn);

        if(AnteaterOn && ArmadilloOn)
        {
            Finished();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        AnteaterController AntC = collision.gameObject.GetComponent<AnteaterController>();
        if (AntC != null)
        { // Its the anteater!
            AnteaterOn = false;
            return;
        }
        ArmadilloController ArmC = collision.gameObject.GetComponent<ArmadilloController>();
        if (ArmC != null)
        { // Its the armadillo!
            ArmadilloOn = false;
            return;
        }

    }
}
