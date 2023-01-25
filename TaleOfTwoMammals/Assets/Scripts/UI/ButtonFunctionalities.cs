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

    public void OnReturnButtonInCreditsPagePressed()
    {
        creditsPage.gameObject.SetActive(false);
        settingsPage.gameObject.SetActive(false);
        mainPage.gameObject.SetActive(true);
    }

    public void OnQuitButtonPressed()
    {
        Application.Quit();
    }

#endregion

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
}
