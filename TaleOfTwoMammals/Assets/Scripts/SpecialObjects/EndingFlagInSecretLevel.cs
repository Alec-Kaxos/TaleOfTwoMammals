using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingFlagInSecretLevel : EndingFlag
{
	protected override void Finished()
	{
		SceneManager.LoadScene("3-1");
	}
}
