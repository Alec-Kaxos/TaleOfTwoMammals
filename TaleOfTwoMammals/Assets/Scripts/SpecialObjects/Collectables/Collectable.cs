using UnityEngine;

public class Collectable : MonoBehaviour
{
    protected bool isCollected = false;
    [SerializeField] protected Animator animator;
    [SerializeField] protected Collider2D collider;

    private void Awake()
    {
        LoadData();
    }

    private void OnDestroy()
    {
        SaveData();
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") == true)
        {
            Collect();
        }
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") == true)
        {
            Collect();
        }
    }

    protected virtual void Collect()
    {
        Debug.Log("collect");
        isCollected = true;

        animator.SetBool("isCollected", true);

        collider.enabled = false;

        SaveData();
    }

    protected virtual void SetCollectedState()
    {
        if (isCollected == true)
        {
            animator.Play("Collected", 0, 1);
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
