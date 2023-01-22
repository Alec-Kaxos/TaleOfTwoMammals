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
        int collectedState = isCollected ? 1 : 0;
        SaveSystem.Instance.SaveCollectiblePassedState(gameObject.scene.name, collectedState);
    }

    private void LoadData()
    {
        isCollected = SaveSystem.Instance.LoadCollectiblePassedState(gameObject.scene.name);
        SetCollectedState();
    }
}
