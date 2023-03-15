using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewTaker : MonoBehaviour
{
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.C))
		{
			ScreenCapture.CaptureScreenshot(Application.dataPath + "/Sprites/Level Preview/Level Preview.png");
		}
	}
}
