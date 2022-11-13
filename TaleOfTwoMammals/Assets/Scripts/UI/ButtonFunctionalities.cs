using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunctionalities : MonoBehaviour
{
    [SerializeField]
    private Canvas creditsPage;
    [SerializeField]
    private Canvas thingsOtherThanCreditsPage;

    private string StartScene = "Level Management Scene";

    [SerializeField]
    private SceneController SceneC;

#region Start Menu Button Functionalities

    public void OnStartButtonPressed()
    {
        // Good enough for now
        SceneManager.LoadScene(StartScene);
    }

    public void OnCreditsButtonPressed()
    {
        creditsPage.gameObject.SetActive(true);
        thingsOtherThanCreditsPage.gameObject.SetActive(false);
    }

    public void OnReturnButtonInCreditsPagePressed()
    {
        creditsPage.gameObject.SetActive(false);
        thingsOtherThanCreditsPage.gameObject.SetActive(true);
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
        SceneC = GameObject.Find("SceneManager").GetComponent<SceneController>();
        
        SceneC.RestartCurrentLevel();
    }

    public void OnHomeButtonPressed()
    {
        SceneManager.LoadScene("StartMenu");
    }
}
