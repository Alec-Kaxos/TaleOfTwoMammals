using UnityEngine;

public class Collectable : MonoBehaviour
{
    private bool isCollected = false;
    [SerializeField] private Animator animator;
    [SerializeField] private Collider2D collider;

    private void Awake()
    {
        LoadData();
    }

    private void OnDestroy()
    {
        SaveData();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") == true)
        {
            Collect();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") == true)
        {
            Collect();
        }
    }

    private void Collect()
    {
        isCollected = true;

        animator.SetBool("isCollected", true);

        collider.enabled = false;

        SaveData();
    }

    private void SetCollectedState()
    {
        if (isCollected == true)
        {
            animator.Play("Banana_Collected", 0, 1);
            collider.enabled = false;
        }
    }

    private void SaveData()
    {
        // save data to the save file
    }

    private void LoadData()
    {
        // load data from the save file
    }
}
