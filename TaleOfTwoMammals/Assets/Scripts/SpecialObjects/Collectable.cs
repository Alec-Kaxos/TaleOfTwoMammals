using UnityEngine;

public class Collectable : MonoBehaviour
{
    private bool isCollected = false;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject collisionDetector;

    private void Awake()
    {
        LoadData();
    }

    private void OnDestroy()
    {
        SaveData();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") == true)
        {
            Collect();
        }
    }

    private void OnCollisionEnter(Collision collision)
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

        Destroy(collisionDetector);

        SaveData();
    }

    private void SetCollectedState()
    {
        if (isCollected == true)
        {
            animator.Play("Banana_Collected", 0, 1);
            Destroy(collisionDetector);
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
