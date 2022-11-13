using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneController : MonoBehaviour
{

    private struct SceneInfo
    {
        /// <summary>
        /// A Struct which holds preset information about LOADED scenes. 
        ///     Note that scenes MUST follow a very specific format.
        /// </summary>
        /// <param name="n">The name of the loaded scene</param>
        public SceneInfo(String n)
        {
            Name = n;

            S = SceneManager.GetSceneByName(n);
            LevelManagerRef = null;
            AttachPos = new Vector3();
            NextAttachPos = new Vector3();
            Cam = null;
            EnableObject = null;
            DisableObject = null;

            if (!S.IsValid())
            {
                Name = null;
                return;
            }

            foreach (GameObject go in S.GetRootGameObjects())
            {
                switch (go.name)
                {
                    case "Level Manager":
                        LevelManagerRef = go.GetComponent<LevelManager>(); 

                        //Find Attachment Points
                        Vector3 aP = LevelManagerRef.transform.Find("Level Attachment Point").transform.position;
                        Vector3 nAP = LevelManagerRef.transform.Find("Next Level Attachment Point").transform.position;
                        //Floor Attachment Points
                        AttachPos = new Vector3(Mathf.Floor(aP.x), Mathf.Floor(aP.y), Mathf.Floor(aP.z));
                        NextAttachPos = new Vector3(Mathf.Floor(nAP.x), Mathf.Floor(nAP.y), Mathf.Floor(nAP.z));
                        break;

                    case "Level Camera":
                        Cam = go.GetComponent<Camera>();
                        break;

                    case "Keep Enabled":
                        EnableObject = go;
                        break;

                    case "Disable Until Play":
                        DisableObject = go;
                        break;
                }
            }
        }

        /// <summary>
        /// Returns if this is a SceneInfo for a valid Scene.
        /// </summary>
        public bool IsValid()
        {
            return Name != null;
        }

        public Scene S { get; }
        public String Name { get; }

        /// <summary>
        /// A GameObject in the root of the scene, with the name "Level Manager"
        /// </summary>
        public LevelManager LevelManagerRef { get; }
        /// <summary>
        /// Associated with the GameObject "Level Attachment Point" which is a Child to "Level Manager".
        ///     Rounded down to a whole number (floored).
        /// </summary>
        public Vector3 AttachPos { get; }
        /// <summary>
        /// Associated with the GameObject "Next Level Attachment Point" which is a Child to "Level Manager".
        ///     Rounded down to a whole number (floored).
        /// </summary>
        public Vector3 NextAttachPos { get; }

        /// <summary>
        /// A GameObject in the root of the scene, with the name "Level Camera"
        /// </summary>
        public Camera Cam { get; }

        /// <summary>
        /// A GameObject in the root of the scene, with the name "Keep Enabled"
        /// </summary>
        public GameObject EnableObject { get; }
        /// <summary>
        /// A GameObject in the root of the scene, with the name "Disable Until Play"
        /// </summary>
        public GameObject DisableObject { get; }
    }

    [Header("Testing")]
    public string NewSceneName;
    public string CurrentSceneName;
    [SerializeField]
    private bool LoadScenePlease = false;
    [SerializeField]
    private bool NextScenePlease = false;
    [SerializeField]
    private bool RestartScenePlease = false;

    [Header("Level Scenes")]
    [SerializeField, Tooltip("Note: Names in equivalent level order.")]
    private string[] SceneNames;

    [SerializeField, Min(1)]
    private int FutureScenesLoaded = 2;
    [SerializeField, Min(1)]
    private int PreviousScenesLoaded = 2;
    private List<SceneInfo> LoadedScenes;
    private int CurrentScene;
    private int SceneCount;

    [SerializeField]
    private Image ScreenCover;

    private bool AllLoaded = false;
    private bool CurrentLoaded = false;

    [Header("References")]
    [SerializeField]
    private GameObject AnteaterPrefab;
    [SerializeField]
    private GameObject ArmadilloPrefab;

    private AnteaterController Anteater;
    private ArmadilloController Armadillo;


    #region Unity Methods
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FirstLoad(0));
    }

    // Update is called once per frame
    void Update()
    {
        if (LoadScenePlease)
        {
            LoadScenePlease = false;

            StartCoroutine(FirstLoad(0));
            //StartCoroutine(LoadScene());
        }
        if (NextScenePlease)
        {
            NextScenePlease = false;

            StartCoroutine(ToNextLevel());
        }
        if (RestartScenePlease)
        {
            RestartScenePlease = false;

            StartCoroutine(RestartLevel());
        }
    }

    #endregion

    #region Level Loading

    /// <summary>
    /// The first load of the levels, centered on LevelIndex. Will load into LevelIndex.
    /// </summary>
    /// <param name="LevelIndex">The index of the level from the SceneNames array. Starts at 0.</param>
    private IEnumerator FirstLoad(int LevelIndex)
    {
        Color TempColor = ScreenCover.color;
        TempColor.a = 1.0f;
        ScreenCover.color = TempColor;

        CurrentScene = LevelIndex;

        SpawnCharacters();

        Anteater.gameObject.SetActive(false);
        Armadillo.gameObject.SetActive(false);


        //Initialize LoadedScenes with null;
        SceneCount = 1 + FutureScenesLoaded + PreviousScenesLoaded;
        LoadedScenes = new List<SceneInfo>(SceneCount);
        for(int i = 0; i < SceneCount; ++i)
        {
            LoadedScenes.Add(new SceneInfo(null));
        }


        //LOAD CURRENT SCENE
        yield return PreloadLevel(CurrentScene);

        SceneInfo curS = new SceneInfo(SceneNames[CurrentScene]);
        LoadedScenes[PreviousScenesLoaded] = curS;

        CurrentLoaded = true;

        Anteater.gameObject.SetActive(true);
        Armadillo.gameObject.SetActive(true);

        curS.DisableObject.SetActive(true);

        curS.LevelManagerRef.LevelActive(this);

        Camera.main.transform.position = curS.Cam.transform.position;
        Camera.main.orthographicSize = curS.Cam.orthographicSize;

        StartCoroutine(FadeScreenCover(0, 1, 2));

        //CURRENT SCENE NOW LOADED

        Vector3 displacement = new Vector3();

        //Load Previous Scenes
        for (int i = CurrentScene - 1; i >= Math.Max(0, CurrentScene - PreviousScenesLoaded); i--)
        {// i is the index within the SceneNames array

            yield return PreloadLevel(i);

            SceneInfo thisS = new SceneInfo(SceneNames[i]);
            int indexInLoaded = (i - CurrentScene) + PreviousScenesLoaded;
            LoadedScenes[indexInLoaded] = thisS;

            //Also remember to move the loaded scene!
            displacement += LoadedScenes[indexInLoaded + 1].AttachPos - LoadedScenes[indexInLoaded].NextAttachPos;
            thisS.EnableObject.transform.position += displacement;

        }

        displacement = new Vector3();

        //Load Future Scenes
        for (int i = CurrentScene + 1; i < Math.Min(CurrentScene + FutureScenesLoaded + 1, SceneNames.Length); i++)
        {// i is the index within the SceneNames array

            yield return PreloadLevel(i);

            SceneInfo thisS = new SceneInfo(SceneNames[i]);
            int indexInLoaded = (i - CurrentScene) + PreviousScenesLoaded;
            LoadedScenes[indexInLoaded] = thisS;

            //Also remember to move the loaded scene!
            displacement += LoadedScenes[indexInLoaded - 1].NextAttachPos - LoadedScenes[indexInLoaded].AttachPos;
            thisS.EnableObject.transform.position += displacement;

        }

        AllLoaded = true;

        /*
        Debug.Log("Finished preloading everything");
        foreach(SceneInfo SI in LoadedScenes)
        {
            Debug.Log(SI.Name);
        }
        */

    }
    
    /// <summary>
    /// Unloads all level scenes, effectively resetting this SceneManager. 
    /// </summary>
    /// <returns></returns>
    private IEnumerable UnloadAll()
    {
        CurrentScene = -1;

        foreach (SceneInfo SI in LoadedScenes)
        {
            if (SI.IsValid())
            {
                yield return SceneManager.UnloadSceneAsync(SI.S);
            }
        }

        Anteater.gameObject.SetActive(false);
        Armadillo.gameObject.SetActive(false);

        DespawnCharacters();

        AllLoaded = false;
        CurrentLoaded = false;
    }

    /// <summary>
    /// Preloads a level in the background.
    /// </summary>
    /// <param name="levelIndex">The level index in the SceneNames array, starting at 0.</param>
    private IEnumerator PreloadLevel(int levelIndex)
    {
        //Load this scene
        var loading = SceneManager.LoadSceneAsync(SceneNames[levelIndex], LoadSceneMode.Additive);

        while (!loading.isDone)
        {
            yield return null;
        }

        SceneInfo thisS = new SceneInfo(SceneNames[levelIndex]);
        thisS.DisableObject.SetActive(false);

        thisS.LevelManagerRef.LevelFullyLoaded(Anteater, Armadillo);

    }

    private IEnumerator ToNextLevel()
    {
        if (!AllLoaded || CurrentScene >= SceneNames.Length - 1)
        { //If there are no more scenes left, dont go to the next scene !
            yield break;
        }

        CurrentScene++;
        AllLoaded = false;

        //Set the next scene to the active one
        SceneInfo cS = LoadedScenes[PreviousScenesLoaded];
        SceneInfo nS = LoadedScenes[PreviousScenesLoaded + 1];
        cS.LevelManagerRef.LevelUnactive();
        cS.DisableObject.SetActive(false);
        nS.DisableObject.SetActive(true);
        nS.LevelManagerRef.LevelActive(this);

        //Transition to the next scene
        Vector3 movement =  cS.NextAttachPos - nS.AttachPos;

        //  this just moves both scenes to the right place :) (over 1000 frames)
        int frames = 1000;
        for (int i = 0; i < frames; ++i)
        {
            //camera
            Camera.main.transform.position += (nS.Cam.transform.position - cS.Cam.transform.position) / frames;
            Camera.main.orthographicSize += (nS.Cam.orthographicSize - cS.Cam.orthographicSize) / frames;

            //Move the scenes/levels (to simulate camera movement)
            foreach (SceneInfo SI in LoadedScenes)
            {
                if (SI.IsValid())
                {
                    SI.EnableObject.transform.position -= movement / frames;
                }
            }

            //Move the players with the scenes
            Anteater.transform.position -= movement / frames;
            Armadillo.transform.position -= movement / frames;


            yield return null;
        }



        //FINALLY, unload the first level loaded and insert the new level that should be loaded

        //Wait to unload first scene
        if (LoadedScenes[0].IsValid())
        {
            yield return SceneManager.UnloadSceneAsync(LoadedScenes[0].S);
        }

        LoadedScenes.RemoveAt(0);
        int lastScene = CurrentScene + FutureScenesLoaded;
        if(lastScene < SceneNames.Length)
        {
            yield return PreloadLevel(lastScene);

            SceneInfo thisS = new SceneInfo(SceneNames[lastScene]);
            int indexInLoaded = SceneCount - 1;
            LoadedScenes.Add(thisS);

            //Also remember to move the loaded scene!

            Vector3 displacement = new Vector3();
            for(int i = PreviousScenesLoaded; i < SceneCount-1; ++i)
            {
                displacement += LoadedScenes[i].NextAttachPos - LoadedScenes[i+1].AttachPos;
            }
            thisS.EnableObject.transform.position += displacement;
        }
        else
        {
            LoadedScenes.Add(new SceneInfo(null));
        }

        AllLoaded = true;

        /*
        Debug.Log("Finished next");
        foreach (SceneInfo SI in LoadedScenes)
        {
            Debug.Log(SI.Name);
        }
        */

    }

    private IEnumerator RestartLevel()
    {
        if (!CurrentLoaded) yield break;

        yield return FadeScreenCover(1.0f, 0, 1f);

        CurrentLoaded = false;
        AllLoaded = false;

        Anteater.gameObject.SetActive(false);
        Armadillo.gameObject.SetActive(false);

        DespawnCharacters();

        SceneInfo curS = LoadedScenes[PreviousScenesLoaded];

        //Unload current scene
        if (curS.IsValid())
        {
            curS.LevelManagerRef.LevelUnactive();
            yield return SceneManager.UnloadSceneAsync(curS.S);
        }

        //LOAD CURRENT SCENE
        yield return PreloadLevel(CurrentScene);

        curS = new SceneInfo(SceneNames[CurrentScene]);
        LoadedScenes[PreviousScenesLoaded] = curS;

        CurrentLoaded = true;
        AllLoaded = true;

        SpawnCharacters();

        Anteater.gameObject.SetActive(true);
        Armadillo.gameObject.SetActive(true);

        curS.DisableObject.SetActive(true);

        curS.LevelManagerRef.LevelActive(this);

        StartCoroutine(FadeScreenCover(0, 1, 2));

        //CURRENT SCENE NOW LOADED
    }

    private IEnumerator FadeScreenCover(float alpha, float secondsBefore, float seconds)
    {
        float curTime = 0f;
        while (curTime < secondsBefore)
        {
            curTime += Time.deltaTime;

            yield return null;
        }

        Color StartColor = ScreenCover.color;
        Color EndColor = StartColor;
        EndColor.a = alpha;

        curTime = 0f;

        while (curTime < seconds)
        {
            curTime += Time.deltaTime;

            ScreenCover.color = Color.Lerp(StartColor, EndColor, (curTime / seconds));

            yield return null;
        }

        ScreenCover.color = EndColor;
        
    }

    #endregion

    #region Characters

    private void SpawnCharacters()
    {
        GameObject AntTemp = Instantiate(AnteaterPrefab, new Vector3(), Quaternion.identity);
        GameObject ArmTemp = Instantiate(ArmadilloPrefab, new Vector3(), Quaternion.identity);

        Anteater = AntTemp.GetComponent<AnteaterController>();
        Armadillo = ArmTemp.GetComponent<ArmadilloController>();

        if (LoadedScenes != null)
        {
            foreach (SceneInfo si in LoadedScenes)
            {
                if (si.IsValid())
                {
                    si.LevelManagerRef.RelinkReferences(Anteater, Armadillo);
                }
            }
        }
    }

    private void DespawnCharacters()
    {
        Destroy(Anteater.gameObject);
        Destroy(Armadillo.gameObject);
        Anteater = null;
        Armadillo = null;
    }

    #endregion

    #region Public Methods


    /// <summary>
    /// Call when a level is completed.
    /// </summary>
    /// <returns>If the SceneManager could successfully process the level complete request.</returns>
    public bool LevelCompleted()
    {
        if (!AllLoaded) return false;

        StartCoroutine(ToNextLevel());

        return true;
    }

    /// <summary>
    /// Call to restart the CURRENT Level
    /// </summary>
    /// <returns>If the level could be successfully restarted.</returns>
    public bool RestartCurrentLevel()
    {
        if (!CurrentLoaded) return false;

        StartCoroutine(RestartLevel());

        return true;
    }

    #endregion

    #region Testing
    private IEnumerator LoadScene()
    {
        //1. Load Scene (In the background)
        var loading = SceneManager.LoadSceneAsync(NewSceneName, LoadSceneMode.Additive);
        
        while(!loading.isDone)
        {
            yield return null;
        }

        //Once the scene is ready, can set a flag (IE: SceneInfo NOT being null)
        SceneInfo nS = new SceneInfo(NewSceneName);
        nS.DisableObject.SetActive(false);

        SceneInfo cS = new SceneInfo(CurrentSceneName);

        //Also remember to move the loaded scene!
        Vector3 displacement = -nS.AttachPos + cS.NextAttachPos;
        nS.EnableObject.transform.position += displacement;

        //2. After some point (here its immediate), transtition into next scene,
        //  this just moves both scenes to the right place :) (over 1000 frames)
        int times = 1000;
        for (int i = 0;  i < times; ++i)
        {
            //camera
            Camera.main.transform.position += (nS.Cam.transform.position - cS.Cam.transform.position) / times;
            Camera.main.orthographicSize += (nS.Cam.orthographicSize - cS.Cam.orthographicSize) / times;

            nS.EnableObject.transform.position -= displacement/times;
            cS.EnableObject.transform.position -= displacement/times;

            yield return null;
        }

        nS.DisableObject.SetActive(true);
        cS.DisableObject.SetActive(false);

        SceneManager.UnloadSceneAsync(cS.S);

        String swap = NewSceneName;
        NewSceneName = CurrentSceneName;
        CurrentSceneName = swap;


    }

    #endregion
}
