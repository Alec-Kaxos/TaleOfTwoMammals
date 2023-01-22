using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldenAnt : Collectable
{
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Tongue"))
        {
            Collect();
        }
    }
}
