using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarvingPeople : Collectable
{
	protected override void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.GetComponent<Pullable>())
		{
			Collect();
		}
	}
}
