using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pullable : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Anteater's isInCoroutine means that the Anteater is trying to shoot the tongue, rather than the Anteater already shoots out the tongue bridge
        if (collision.gameObject.CompareTag("Tongue") && collision.gameObject.transform.parent.parent.GetComponent<AnteaterController>().isInCoroutine == true)
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

	private void OnCollisionExit2D(Collision2D collision)
	{
        if (collision.gameObject.CompareTag("Tongue") && collision.gameObject.transform.parent.parent.GetComponent<AnteaterController>().isInCoroutine == true)
		{
            FixedJoint2D fixedJoint = gameObject.GetComponent<FixedJoint2D>();
            fixedJoint.enabled = false;
        }
    }
}
