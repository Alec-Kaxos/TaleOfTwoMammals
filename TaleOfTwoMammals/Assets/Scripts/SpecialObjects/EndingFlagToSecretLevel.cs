using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingFlagToSecretLevel : EndingFlag
{
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
}
