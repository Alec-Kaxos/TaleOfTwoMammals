using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HingeJoint2D)) ,RequireComponent(typeof(Rigidbody2D))]
public class RopeSegment : MonoBehaviour
{
    [SerializeField]
    public GameObject ConnectedAbove, ConnectedBelow;

    // Start is called before the first frame update
    void Start()
    {
        ConnectedAbove = GetComponent<HingeJoint2D>().connectedBody.gameObject;
        RopeSegment SegementAbove = ConnectedAbove.GetComponent<RopeSegment>();
        if (SegementAbove != null)
        {
            SegementAbove.ConnectedBelow = gameObject;
            float SpriteHeight = GetComponent<SpriteRenderer>().bounds.size.y;
            GetComponent<HingeJoint2D>().connectedAnchor = new Vector2(0, -SpriteHeight);
        }
        else
        {
            GetComponent<HingeJoint2D>().connectedAnchor = new Vector2(0, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
