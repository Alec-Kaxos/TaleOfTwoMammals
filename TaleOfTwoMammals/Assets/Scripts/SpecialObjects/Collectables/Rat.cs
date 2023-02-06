using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rat : Collectable
{
	[SerializeField] private float lookingForThreatDuration = 2f;
	private float lookingForThreatTimer = 0f;
    private bool isInFakeDeathState = false;
	private bool lookingForThreat = false;
	private List<PlayerController> potentialThreatList = new List<PlayerController>();

	protected override void OnCollisionEnter2D(Collision2D collision)
	{
		PlayerController player = collision.gameObject.GetComponent<PlayerController>();
		if (player != null)
		{
			Debug.Log("a player just enter, and isInFakeDeathState is: " + isInFakeDeathState);
			lookingForThreat = false;
			lookingForThreatTimer = 0;
			if (isInFakeDeathState == true)
			{
				Collect();
			}
			else
			{
				potentialThreatList.Add(player);
				isInFakeDeathState = true;
				FakeDeath();
			}
		}
	}

	private void OnCollisionExit2D(Collision2D collision)
	{
		PlayerController player = collision.gameObject.GetComponent<PlayerController>();
		if (player != null)
		{
			potentialThreatList.Remove(player);
			if (potentialThreatList.Count == 0)
			{
				lookingForThreat = true;
			}
		}
	}

	protected override void OnTriggerEnter2D(Collider2D collision)
	{
		PlayerController player = collision.gameObject.GetComponent<PlayerController>();
		if (player != null)
		{
			Debug.Log("a player just enter, and isInFakeDeathState is: " + isInFakeDeathState);
			lookingForThreat = false;
			lookingForThreatTimer = 0;
			if (isInFakeDeathState == true)
			{
				Collect();
			}
			else
			{
				potentialThreatList.Add(player);
				isInFakeDeathState = true;
				FakeDeath();
			}
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		PlayerController player = collision.gameObject.GetComponent<PlayerController>();
		if (player != null)
		{
			potentialThreatList.Remove(player);
			if (potentialThreatList.Count == 0)
			{
				lookingForThreat = true;
			}
		}
	}

	private void FakeDeath()
	{
		animator.SetBool("isFakingDeath", true);
		isInFakeDeathState = true;
	}

	private void Update()
	{
		if (lookingForThreat)
		{
			lookingForThreatTimer += Time.fixedDeltaTime;
			if (lookingForThreatTimer >= lookingForThreatDuration)
			{
				lookingForThreatTimer = 0;
				animator.SetBool("isFakingDeath", false);
				isInFakeDeathState = false;
			}
		}
	}
}
