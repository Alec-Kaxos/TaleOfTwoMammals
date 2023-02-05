using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pullable : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Tongue"))
        {
            FixedJoint2D fixedJoint = gameObject.GetComponent<FixedJoint2D>();
            fixedJoint.enabled = true;
            fixedJoint.connectedBody = collision.gameObject.GetComponent<Rigidbody2D>();
            AnteaterController anteater = collision.transform.parent.parent.GetComponent<AnteaterController>();
            // Debug.Log(new Vector2(transform.position.x, transform.position.y) - anteater.tongueEndPoint);
            // I don't know why but this arbitrary number works :)
            fixedJoint.connectedAnchor = new Vector2(1f, 0.5f);
        }
    }
}
