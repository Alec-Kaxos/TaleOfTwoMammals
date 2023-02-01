using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : ButtonUser
{
    [SerializeField]
    private Sprite ClosedSprite;
    [SerializeField]
    private Sprite OpenSprite;
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private PolygonCollider2D gateCollider;

    protected virtual void open()
    {
        spriteRenderer.sprite = OpenSprite;
        gateCollider.enabled = false;
    }

    protected virtual void close()
    {
        spriteRenderer.sprite = ClosedSprite;
        gateCollider.enabled = true;
    }

    protected override void Activated()
    {
        open();
    }

    protected override void Deactivated()
    {
        close();
    }


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
