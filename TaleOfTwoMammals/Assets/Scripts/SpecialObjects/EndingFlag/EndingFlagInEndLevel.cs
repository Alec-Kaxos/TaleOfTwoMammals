using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingFlagInEndLevel : EndingFlag
{
	[SerializeField] 
	private EndingFlagInEndLevel otherEndingFlag = null;
	[SerializeField]
	private Sprite normalSprite;
	[SerializeField]
	private Sprite spriteWhenPlayerIn;
	[SerializeField]
	private SpriteRenderer spriteRenderer;

	public bool isPlayerIn = false;

	protected override void OnTriggerEnter2D(Collider2D collision)
	{
		isPlayerIn = true;
		spriteRenderer.sprite = spriteWhenPlayerIn;
		if (otherEndingFlag.isPlayerIn == true)
		{
			base.Finished();
		}
	}

	protected override void OnTriggerExit2D(Collider2D collision)
	{
		isPlayerIn = false;
		spriteRenderer.sprite = normalSprite;
	}
}
