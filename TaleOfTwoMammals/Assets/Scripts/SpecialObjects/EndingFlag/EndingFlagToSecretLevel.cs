using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingFlagToSecretLevel : EndingFlag
{
	[SerializeField]
	private SpriteRenderer spriteRenderer;
	[SerializeField]
	private Sprite errorRoadSign;

	protected override void Finished()
	{
		if (SaveSystem.Instance.CanUnlockSecretLevel())
		{
			SceneManager.LoadScene("Secret Level");
		}
		else
		{
			base.Finished();
		}
	}

	private void Update()
	{
		if (SaveSystem.Instance.CanUnlockSecretLevel())
		{
			spriteRenderer.sprite = errorRoadSign;
		}
	}
}
