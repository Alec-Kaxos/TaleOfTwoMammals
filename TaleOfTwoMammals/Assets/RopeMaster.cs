using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeMaster : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D Hook;
    private Rigidbody2D FirstLink;
    public Rigidbody2D EndPoint { get; private set; }
    [SerializeField]
    private GameObject EndPointPrefab;
    [SerializeField]
    private GameObject PrefabRopeSegment;
    public float SegmentLength { get; private set; }
    [SerializeField]
    public int MaxSegments = 10;

    public int CurSegments { get; private set; }

    public bool linkadd = false;
    public bool linkremove = false;
    public bool shoottest = false;
    public float shootAngle = 20f;
    public float shootForce = 20f;
    public float shootMax = 20f;
    public bool retracttest = false;
    public float retractsec = 10f;

    private void Awake()
    {
        SegmentLength = PrefabRopeSegment.GetComponent<SpriteRenderer>().bounds.size.y;
    }

    // Start is called before the first frame update
    void Start()
    {
        //GenerateRope(NumberOfSegments);
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
        if(shoottest)
        {
            ShootTowards(shootAngle, shootForce, shootMax);
            shoottest = false;
        }
        if(retracttest)
        {
            RetractRope(retractsec);
            retracttest = false;
        }
    }

    private void GenerateRope(int Segments)
    {
        if (Segments < 1)
        {
            Segments = 1;
        }
        else if (Segments > MaxSegments)
        {
            Segments = MaxSegments;
        }

        Rigidbody2D LastSegment = Hook;
        for (int i = 0; i < Segments; ++i)
        {
            GameObject NewSegment = Instantiate(PrefabRopeSegment);
            NewSegment.transform.position = Hook.position;
            NewSegment.transform.parent = transform;
            NewSegment.GetComponent<RopeSegment>().SetSegmentLength(SegmentLength);
            NewSegment.GetComponent<RopeSegment>().SetConnectedAbove(LastSegment.gameObject);
            LastSegment = NewSegment.GetComponent<Rigidbody2D>();
            if (i == 0)
            { //Make sure we know what the first segment is
                FirstLink = LastSegment;
            }
            if (i == Segments - 1)
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

    /// <summary>
    /// Adds a segement initially with the given angle on the hook.
    /// </summary>
    /// <param name="Angle">Angle represented in Degrees</param>
    private void AddSegment(float Angle = 0f)
    {
        if (CurSegments == 0)
        {
            GenerateRope(1);
        }
        else if (CurSegments < MaxSegments)
        {
            GameObject NewSegment = Instantiate(PrefabRopeSegment);
            NewSegment.transform.position = Hook.position;
            NewSegment.transform.eulerAngles = new Vector3(0, 0, Angle);
            NewSegment.transform.parent = transform;
            NewSegment.GetComponent<RopeSegment>().SetSegmentLength(SegmentLength);
            NewSegment.GetComponent<RopeSegment>().SetConnectedAbove(Hook.gameObject);
            FirstLink.GetComponent<RopeSegment>().SetConnectedAbove(NewSegment);

            FirstLink = NewSegment.GetComponent<Rigidbody2D>();

            CurSegments++;
        }
        else
        { //rope too long now :)
            
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
            SecondSegment.GetComponent<RopeSegment>().ConnectToHook(Hook);

            Rigidbody2D FirstLinkTemp = FirstLink;
            FirstLink = SecondSegment.GetComponent<Rigidbody2D>();
            Destroy(FirstLinkTemp.gameObject);

            CurSegments--;
        }
    }

    public float CurrentLength()
    {
        return CurSegments * SegmentLength;
    }

    /// <summary>
    /// Shoots the rope the given direction. Only works if not shot out yet.
    /// </summary>
    /// <returns>If successfully started shooting the rope.</returns>
    public bool ShootTowards(float Angle, float Force, float MaxDistance = 0f)
    {
        if (CurSegments == 0)
        {
            GenerateRope(1);
            FirstLink.GetComponent<BoxCollider2D>().isTrigger = false;
            StartCoroutine(ShootRope(Angle, Force, MaxDistance));
            return true;
        }
        else
        {
            return false;
        }
    }

    private IEnumerator ShootRope(float Angle, float Force, float MaxDistance = 0f)
    {
        float Distance = 0f;
        Vector3 Direction = new Vector3(Mathf.Cos(Angle * Mathf.Deg2Rad), Mathf.Sin(Angle * Mathf.Deg2Rad)) * Force;
        EndPoint.bodyType = RigidbodyType2D.Static;
        
        while (Distance < MaxDistance || MaxDistance == 0f)
        {
            if ( (CurSegments + 1) * SegmentLength < Distance)
            {
                AddSegment(Angle+90f);
                if (CurSegments < MaxSegments)
                {
                    FirstLink.GetComponent<BoxCollider2D>().isTrigger = false;
                }
                else
                { // The closest segment to the anteater
                    break;
                }
            }

            EndPoint.transform.position += Direction * Time.deltaTime;
            Distance += Force * Time.deltaTime;
            yield return null;
        }

        //EndPoint.bodyType = RigidbodyType2D.Dynamic;
    }

    public bool RetractRope(float SegmentsPerSecond)
    {
        if (CurSegments == 0)
        {
            return false;
        }
        else
        {
            GameObject LastSegment = FirstLink.gameObject;
            while(LastSegment)
            {
                LastSegment.GetComponent<BoxCollider2D>().isTrigger = true;
                LastSegment = LastSegment.GetComponent<RopeSegment>().ConnectedBelow;
            }

            StartCoroutine(RetractRopeRoutine(SegmentsPerSecond));
            return true;
        }
    }

    private IEnumerator RetractRopeRoutine(float SegmentsPerSecond)
    {
        EndPoint.bodyType = RigidbodyType2D.Dynamic;

        while (CurSegments > 0)
        {
            RemoveSegment();
            yield return new WaitForSeconds(1f / SegmentsPerSecond);
        }
    }

}
