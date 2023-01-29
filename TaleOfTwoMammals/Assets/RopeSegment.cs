using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HingeJoint2D)) ,RequireComponent(typeof(Rigidbody2D))]
public class RopeSegment : MonoBehaviour
{
    [SerializeField]
    public GameObject ConnectedAbove { get; private set; }
    public GameObject ConnectedBelow { get; private set; }

    private HingeJoint2D _hinge;
    private Rigidbody2D _rigidbody;
    private SpriteRenderer _spriteRenderer;

    private float _segmentLength;

    // Start is called before the first frame update
    void Awake()
    {
        _hinge = GetComponent<HingeJoint2D>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetSegmentLength(float length)
    {
        _segmentLength = length;
    }

    public float GetSegmentLength()
    {
        //float SpriteHeight = _spriteRenderer.bounds.size.y;
        //return SpriteHeight;
        return _segmentLength;
    }

    public void SetConnectedAbove(GameObject Connected)
    {
        ConnectedAbove = Connected;
        _hinge.connectedBody = ConnectedAbove.GetComponent<Rigidbody2D>();
        RopeSegment SegementAbove = ConnectedAbove.GetComponent<RopeSegment>();
        if (SegementAbove != null)
        {
            SegementAbove.ConnectedBelow = gameObject;
            _hinge.connectedAnchor = new Vector2(0, -SegementAbove.GetSegmentLength());
        }
        else
        {
            _hinge.connectedAnchor = new Vector2(0, 0);
        }
    }

    public void ConnectToHook(Rigidbody2D Hook)
    {
        _hinge.connectedBody = Hook;
        _hinge.connectedAnchor = new Vector2(0, 0);
        ConnectedAbove = Hook.gameObject;
    }
}
