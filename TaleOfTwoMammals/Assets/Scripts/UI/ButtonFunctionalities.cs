using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunctionalities : MonoBehaviour
{
#region Start Menu Button Functionalities

    public void OnStartButtonPressed()
    {
        // Good enough for now
        SceneManager.LoadScene("SampleScene");
    }

    public void OnCreditsButtonPressed()
    {

    }

    public void OnQuitButtonPressed()
    {
        Application.Quit();
    }

#endregion

    // Temporary
    public void OnRestartButtonPressed()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
