using UnityEngine;

public class Collectable : MonoBehaviour
{
    protected bool isCollected = false;
    [SerializeField] protected Animator animator;
    [SerializeField] protected Collider2D collider;
    [SerializeField] protected string collectParticlesName = "Collect Collectable";
    [SerializeField] protected Color collectableColor1 = Color.yellow;
    //Gold color by default
    [SerializeField] protected Color collectableColor2 = new Color(1f, 0.8431373f, 0f);
    private GameObject collectParticles;

    private void Awake()
    {
        LoadData();
        collectParticles = Resources.Load<GameObject>("Particles/" + collectParticlesName);
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
        IncrementCollectedAmount();

        isCollected = true;

        animator.SetBool("isCollected", true);

        //Particles
        ParticleMaster.SpawnParticle(collectParticles, transform.position, color1: collectableColor1, color2: collectableColor2);

        collider.enabled = false;

        SaveData();
    }

    protected virtual void SetCollectedState()
    {
        if (isCollected == true)
        {
			animator.Play("Collected", 0, 1);
            collider.enabled = false;
			animator.SetBool("isCollected", true);
			IncrementCollectedAmount();
        }
    }

    private void IncrementCollectedAmount()
	{
        SaveSystem.Instance.IncrementCollectedAmount(1);
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
