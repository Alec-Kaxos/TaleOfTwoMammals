using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingFlagForSecretLevel : EndingFlag
{
	protected override void Finished()
	{
		if (SaveSystem.Instance.CanUnlockSecretLevel())
		{
			base.Finished();
		}
		else
		{
			// congrat screen?
		}
	}
}
