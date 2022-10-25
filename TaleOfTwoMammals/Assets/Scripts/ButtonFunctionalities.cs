using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunctionalities : MonoBehaviour
{
    public void OnStartButtonPressed()
    {
        // Good enough for now
        SceneManager.LoadScene("SampleScene");
    }

    public void OnQuitButtonPressed()
    {
        Application.Quit();
    }
}
