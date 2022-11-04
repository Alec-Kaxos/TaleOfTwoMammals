using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTesting : MonoBehaviour
{
    public string NewSceneName;
    public string CurrentSceneName;
    public bool LoadScenePlease = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (LoadScenePlease)
        {
            LoadScenePlease = false;

            StartCoroutine(LoadScene());
        }
    }

    private IEnumerator LoadScene()
    {
        var loading = SceneManager.LoadSceneAsync(NewSceneName, LoadSceneMode.Additive);
        
        while(!loading.isDone)
        {
            yield return null;
        }

        Scene scene = SceneManager.GetSceneByName(NewSceneName);
        Vector3 camNewP = new Vector3();
        float camNewS = 0f;
        foreach (GameObject go in scene.GetRootGameObjects())
        {
            Camera a = go.GetComponent<Camera>();
            if (a != null)
            {
                camNewP = go.transform.position;
                camNewS = a.orthographicSize;
            }
            go.transform.position += new Vector3(10f, 10f, 0f);
        }

        Scene active = SceneManager.GetSceneByName(CurrentSceneName);
        Vector3 camCurP = Camera.main.transform.position;
        float camCurS = Camera.main.orthographicSize;

        int times = 1000;
        for (int i = 0;  i < times; ++i)
        {
            //camera
            Camera.main.transform.position += (camNewP - camCurP) / times;
            Camera.main.orthographicSize += (camNewS - camCurS) / times;

            foreach (GameObject go in scene.GetRootGameObjects())
            {
                go.transform.position -= new Vector3(10f/ times, 10f/ times, 0f);
            }

            foreach (GameObject go in active.GetRootGameObjects())
            {
                go.transform.position -= new Vector3(10f/ times, 10f/ times, 0f);
            }


            yield return null;
        }

        SceneManager.UnloadSceneAsync(active);


    }

}
