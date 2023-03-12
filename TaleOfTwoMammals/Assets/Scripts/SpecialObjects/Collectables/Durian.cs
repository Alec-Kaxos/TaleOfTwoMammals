using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Durian : Collectable
{
    [SerializeField] Sprite crackedSprite;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] protected AudioSource CollectingSound;

    private bool hasBeenCracked = false;

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (hasBeenCracked == false)
            {
                ArmadilloController Armadillo = collision.gameObject.GetComponent<ArmadilloController>();

                if (Armadillo != null)
                {
                    if (Armadillo.IsPounding())
                    {
                        animator.SetBool("isCracked", true);
                        spriteRenderer.sprite = crackedSprite;
                        hasBeenCracked = true;
                    }

                }
            }
            else
            {
                CollectingSound.Play();
                Collect();
            }
        }
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (hasBeenCracked == false)
            {
                ArmadilloController Armadillo = collision.gameObject.GetComponent<ArmadilloController>();

                if (Armadillo != null)
                {
                    if (Armadillo.IsPounding())
                    {
                        animator.SetBool("isCracked", true);
                        spriteRenderer.sprite = crackedSprite;
                        hasBeenCracked = true;
                    }

                }
            }
            else
            {
                CollectingSound.Play();
                Collect();
            }
        }
    }
}
