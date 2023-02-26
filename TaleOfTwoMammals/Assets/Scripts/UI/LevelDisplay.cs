using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelDisplay : MonoBehaviour
{
    [SerializeField]
    private int world;
	[SerializeField]
	private int level;
    [SerializeField]
    private RawImage rawImage;
	[SerializeField]
	private Button button;

	private string levelString;
	private string StartScene = "Level Management Scene";

	private void Awake()
	{
		levelString = world + "-" + level;
		button.onClick.AddListener(TeleportToLevel);
		button.GetComponentInChildren<Text>().text = levelString;
	}

	private void OnEnable()
	{
		if (SaveSystem.Instance.LoadLevelPassedState(levelString))
		{
			rawImage.color = Color.white;
			button.GetComponent<Image>().color = Color.white;
			button.interactable = true;
		}
		else
		{
			Color newGrey = Color.grey;
			newGrey.a = 0.3f;
			rawImage.color = newGrey;
			button.GetComponent<Image>().color = newGrey;
			button.interactable = false;
		}
	}

	// called when the reset button in the collecible page is pressed
	public void Refresh()
	{
		Color newGrey = Color.grey;
		newGrey.a = 0.3f;
		rawImage.color = newGrey;
		button.GetComponent<Image>().color = newGrey;
		button.interactable = false;
	}

	private void TeleportToLevel()
	{
		SceneController.FirstLoadWorld = world;
		SceneController.FirstLoadLevel = level;
		SceneController.LoadOnStart = true;
		SceneManager.LoadScene(StartScene);
	}
}
