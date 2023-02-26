using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunctionalities : MonoBehaviour
{
    [SerializeField]
    private Canvas creditsPage;
    [SerializeField]
    private Canvas settingsPage;
    [SerializeField]
    private Canvas collectiblesPage;
    [SerializeField]
    private Canvas levelsPage;
    [SerializeField]
    private Canvas mainPage;

    private string StartScene = "Level Management Scene";

    [SerializeField]
    private SceneController SceneC;

#region Start Menu Button Functionalities

    public void OnStartButtonPressed()
    {
        //I'm (Aaron) sorry for the absolute references :(
        SceneController.FirstLoadWorld = 0;
        SceneController.FirstLoadLevel = 1;
        SceneController.LoadOnStart = true;
        SceneManager.LoadScene(StartScene);
    }

    public void OnCreditsButtonPressed()
    {
        creditsPage.gameObject.SetActive(true);
        mainPage.gameObject.SetActive(false);
    }

    public void OnSettingsButtonPressed()
    {
        settingsPage.gameObject.SetActive(true);
        mainPage.gameObject.SetActive(false);
    }

    public void OnCollectiblesButtonPressed()
	{
        collectiblesPage.gameObject.SetActive(true);
        mainPage.gameObject.SetActive(false);
	}

    public void OnLevelsButtonPressed()
	{
        levelsPage.gameObject.SetActive(true);
        mainPage.gameObject.SetActive(false);
	}

    public void OnReturnButtonPressed()
    {
        creditsPage.gameObject.SetActive(false);
        settingsPage.gameObject.SetActive(false);
        collectiblesPage.gameObject.SetActive(false);
        levelsPage.gameObject.SetActive(false);
        mainPage.gameObject.SetActive(true);
    }

    public void OnQuitButtonPressed()
    {
        Application.Quit();
    }

    public void OnDeleteButtonPressed()
    {
        SaveSystem.Instance.DeleteAllSaves();
    }
    
    #endregion


    #region Level Buttons
    // Temporary
    public void OnRestartButtonPressed()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        if (!SceneC)
            SceneC = GameObject.Find("SceneManager").GetComponent<SceneController>();
        
        SceneC.RestartCurrentLevel();
    }

    public void OnHomeButtonPressed()
    {
        SceneManager.LoadScene("StartMenu");
    }

    public void OnNextButtonPressed()
    {
        if (!SceneC)
            SceneC = GameObject.Find("SceneManager").GetComponent<SceneController>();
        SceneC.LevelCompleted();
    }


    public void OnBackButtonPressed()
    {
        if (!SceneC)
            SceneC = GameObject.Find("SceneManager").GetComponent<SceneController>();
        SceneC.GoBackLevel();
    }

    public void OnWorldButtonPressed()
    {
        if (!SceneC)
            SceneC = GameObject.Find("SceneManager").GetComponent<SceneController>();
        SceneC.GoToNextWorld();
    }

    public void OnWorldBackButtonPressed()
    {
        if (!SceneC)
            SceneC = GameObject.Find("SceneManager").GetComponent<SceneController>();
        SceneC.GoToLastWorld();
    }
    #endregion
}
