using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectibleDisplay : MonoBehaviour
{
	[SerializeField]
	private string collectibleLevel;
	[SerializeField]
	private RawImage rawImage;

	private void OnEnable()
	{
		if (SaveSystem.Instance.LoadCollectiblePassedState(collectibleLevel))
		{
			rawImage.color = Color.white;
		}
		else
		{
			Color newGrey = Color.grey;
			newGrey.a = 0.3f;
			rawImage.color = newGrey;
		}
	}

	// called when the reset button in the collecible page is pressed
	public void Refresh()
	{
		if (SaveSystem.Instance.LoadCollectiblePassedState(collectibleLevel))
		{
			rawImage.color = Color.white;
		}
		else
		{
			Color newGrey = Color.grey;
			newGrey.a = 0.3f;
			rawImage.color = newGrey;
		}
	}
}
