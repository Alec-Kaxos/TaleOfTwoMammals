using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeMaster : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D Hook;
    private Rigidbody2D FirstLink;
    private Rigidbody2D EndPoint;
    [SerializeField]
    private GameObject EndPointPrefab;
    [SerializeField]
    private GameObject[] PrefabRopeSegments;
    [SerializeField]
    private int NumberOfSegments = 10;

    public int CurSegments { get; private set; }

    public bool linkadd = false;
    public bool linkremove = false;

    // Start is called before the first frame update
    void Start()
    {
        GenerateRope(NumberOfSegments);
    }

    // Update is called once per frame
    void Update()
    {
        if (linkadd)
        {
            AddSegment();
            linkadd = false;
        }
        if(linkremove)
        {
            RemoveSegment();
            linkremove = false;
        }

    }

    private void GenerateRope(int Segments)
    {
        if (Segments < 1)
        {
            Segments = 1;
        }
        Rigidbody2D LastSegment = Hook;
        for (int i = 0; i < Segments; ++i)
        {
            GameObject NewSegment = Instantiate(PrefabRopeSegments[Random.Range(0, PrefabRopeSegments.Length)]);
            NewSegment.transform.position = Hook.position;
            NewSegment.transform.parent = transform;
            NewSegment.GetComponent<HingeJoint2D>().connectedBody = LastSegment;
            LastSegment = NewSegment.GetComponent<Rigidbody2D>();
            if (i == 0)
            { //Make sure we know what the first segment is
                FirstLink = LastSegment;
            }
            else if (i == Segments - 1)
            { //Give the last segment an endpoint thing...
                GameObject EndPt = Instantiate(EndPointPrefab);
                EndPt.transform.position = Hook.position;
                EndPt.transform.parent = transform;
                EndPt.GetComponent<HingeJoint2D>().connectedBody = LastSegment;
                EndPt.GetComponent<HingeJoint2D>().connectedAnchor = new Vector2(0, -NewSegment.GetComponent<SpriteRenderer>().bounds.size.y);
                EndPoint = EndPt.GetComponent<Rigidbody2D>();
            }
        }
        CurSegments = Segments;
    }

    private void AddSegment()
    {
        if (CurSegments == 0)
        {
            GenerateRope(1);
        }
        else
        {
            GameObject NewSegment = Instantiate(PrefabRopeSegments[Random.Range(0, PrefabRopeSegments.Length)]);
            NewSegment.transform.position = Hook.position;
            NewSegment.transform.parent = transform;
            NewSegment.GetComponent<HingeJoint2D>().connectedBody = Hook;
            FirstLink.GetComponent<HingeJoint2D>().connectedBody = NewSegment.GetComponent<Rigidbody2D>();
            FirstLink.GetComponent<HingeJoint2D>().connectedAnchor = new Vector2(0, -NewSegment.GetComponent<SpriteRenderer>().bounds.size.y);
            FirstLink.GetComponent<RopeSegment>().ConnectedAbove = NewSegment;
            NewSegment.GetComponent<RopeSegment>().ConnectedBelow = FirstLink.gameObject;

            FirstLink = NewSegment.GetComponent<Rigidbody2D>();

            CurSegments++;
        }
    }

    private void RemoveSegment()
    {
        if (CurSegments == 0)
        {
            
        }
        else if (CurSegments == 1)
        {
            Destroy(FirstLink.gameObject);
            FirstLink = null;
            Destroy(EndPoint.gameObject);
            EndPoint = null;
            CurSegments = 0;
        }
        else
        {//2+ segments
            GameObject SecondSegment = FirstLink.GetComponent<RopeSegment>().ConnectedBelow;
            SecondSegment.GetComponent<HingeJoint2D>().connectedBody = Hook;
            SecondSegment.GetComponent<HingeJoint2D>().connectedAnchor = new Vector2(0, 0);
            SecondSegment.GetComponent<RopeSegment>().ConnectedAbove = null;

            Rigidbody2D FirstLinkTemp = FirstLink;
            FirstLink = SecondSegment.GetComponent<Rigidbody2D>();
            Destroy(FirstLinkTemp.gameObject);

            CurSegments--;
        }
    }
}
