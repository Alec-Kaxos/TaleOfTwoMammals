using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneController : MonoBehaviour
{

    public static int FirstLoadWorld = 0;
    public static int FirstLoadLevel = 1;
    public static bool LoadOnStart = false;
#if UNITY_EDITOR
    public static bool DEV = true;
#else
    public static bool DEV = false;
#endif

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
    [SerializeField]
    private bool LoadScenePlease = false;
    [SerializeField]
    private bool NextScenePlease = false;
    [SerializeField]
    private bool RestartScenePlease = false;
    [SerializeField]
    private GameObject[] DevObjects;

    [Header("Level Scenes")]
    //private string[] SceneNames;
    [SerializeField, Tooltip("Note: Worlds are 0-indexed.")]
    private int[] LevelsPerWorld;
    [SerializeField, Tooltip("For levels not named as the previous are. Counted as the last world.")]
    private string[] CustomLevels;

    [SerializeField, Min(1)]
    private int FutureScenesLoaded = 2;
    [SerializeField, Min(1)]
    private int PreviousScenesLoaded = 2;
    private List<SceneInfo> LoadedScenes;
    //private int CurrentScene;
    private int SceneCount;

    private int CurrentWorld;
    private int CurrentLevel;

    [Header("World Music")]
    [SerializeField]
    private AudioClip[] WorldMusic;
    [SerializeField, Tooltip("This is fully optional")]
    private AudioClip[] WorldMusicStart;


    [Header("Images")]

    [SerializeField]
    private Image ScreenCover;

    [SerializeField]
    private Color[] BackgroundColorPerWorld;

    private bool AllLoaded = false;
    private bool CurrentLoaded = false;

    [Header("Sounds")]
    [SerializeField]
    private AudioSource WinAudioSource;

    [Header("Transition Timings")]
    [SerializeField]
    private float InitialLoadCoverWait = 1.0f;
    [SerializeField]
    private float InitialLoadCoverFade = 2.0f;
    [SerializeField]
    private float NextLevelTransitionTime = 3.0f;
    [SerializeField]
    private float ResetFadeBlackTime = 1.0f;
    [SerializeField]
    private float ResetHoldTime = 1.0f;
    [SerializeField]
    private float ResetFadeTransparentTime = 2.0f;


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
        //Turn on dev objects (or off)
        foreach (GameObject go in DevObjects)
        {
            go.SetActive(DEV);
        }

        if (LoadOnStart)
        {
            StartCoroutine(FirstLoad(FirstLoadWorld, FirstLoadLevel));
            LoadOnStart = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (LoadScenePlease)
        {
            LoadScenePlease = false;

            StartCoroutine(FirstLoad(FirstLoadWorld, FirstLoadLevel));
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
    /// <param name="World">The world number. Starts at 0.</param>
    /// <param name="Level">The level number. Starts at 1.</param>
    private IEnumerator FirstLoad(int World, int Level)
    {
        Color TempColor = ScreenCover.color;
        TempColor.a = 1.0f;
        ScreenCover.color = TempColor;

        //CurrentScene = LevelIndex;
        CurrentWorld = World;
        CurrentLevel = Level;

        //ADD THE BACKGROUND !!!! :)
        TempColor = Color.cyan;
        if (BackgroundColorPerWorld.Length > CurrentWorld)
        {
            TempColor = BackgroundColorPerWorld[CurrentWorld];
        }
        Camera.main.backgroundColor = TempColor;

        //START THE EPIC WORLD MUSIC !!!! :)
        if (WorldMusic.Length > CurrentWorld && WorldMusic[CurrentWorld] != null)
        {
            if (WorldMusicStart.Length > CurrentWorld && WorldMusicStart[CurrentWorld] != null)
            {
                MusicController.GetMusicController().PlayMusic2(WorldMusicStart[CurrentWorld], WorldMusic[CurrentWorld]);
            }
            else
            {
                MusicController.GetMusicController().PlayMusic2(WorldMusic[CurrentWorld], fadeInTime: .5f);
            }
        }

        SpawnCharacters();

        Anteater.gameObject.SetActive(false);
        Armadillo.gameObject.SetActive(false);


        //Initialize LoadedScenes with null;
        SceneCount = 1 + FutureScenesLoaded + PreviousScenesLoaded;
        LoadedScenes = new List<SceneInfo>(SceneCount);
        for (int i = 0; i < SceneCount; ++i)
        {
            LoadedScenes.Add(new SceneInfo(null));
        }


        //LOAD CURRENT SCENE
        yield return PreloadLevel(CurrentWorld, CurrentLevel);

        SceneInfo curS = new SceneInfo(GetSceneName(World, Level));
        LoadedScenes[PreviousScenesLoaded] = curS;

        CurrentLoaded = true;

        //THIS WAS WHERE SETACTIVE TRUE WAS FOR CHARACTERS

        curS.DisableObject.SetActive(true);

        curS.LevelManagerRef.LevelActive(this);

        //MOVE THE CAMERA
        Camera.main.transform.position = curS.Cam.transform.position;
        Camera.main.orthographicSize = curS.Cam.orthographicSize;

        StartCoroutine(FadeScreenCover(0, InitialLoadCoverWait, InitialLoadCoverFade));

        //CURRENT SCENE NOW LOADED

        Vector3 displacement = new Vector3();

        //Load Previous Scenes
        for (int i = CurrentLevel - 1; i >= Math.Max(1, CurrentLevel - PreviousScenesLoaded); i--)
        {// i is the index within the SceneNames array

            yield return PreloadLevel(CurrentWorld, i);

            SceneInfo thisS = new SceneInfo(GetSceneName(CurrentWorld, i));
            int indexInLoaded = (i - CurrentLevel) + PreviousScenesLoaded;
            LoadedScenes[indexInLoaded] = thisS;

            //Also remember to move the loaded scene!
            displacement += LoadedScenes[indexInLoaded + 1].AttachPos - LoadedScenes[indexInLoaded].NextAttachPos;
            thisS.EnableObject.transform.position += displacement;
            thisS.DisableObject.transform.position += displacement;

        }

        displacement = new Vector3();

        //Load Future Scenes
        for (int i = CurrentLevel + 1; i <= Math.Min(CurrentLevel + FutureScenesLoaded, LevelsInWorld(World)); i++)
        {// i is the index within the SceneNames array

            yield return PreloadLevel(CurrentWorld, i);

            SceneInfo thisS = new SceneInfo(GetSceneName(CurrentWorld, i));
            int indexInLoaded = (i - CurrentLevel) + PreviousScenesLoaded;
            LoadedScenes[indexInLoaded] = thisS;

            //Also remember to move the loaded scene!
            displacement += LoadedScenes[indexInLoaded - 1].NextAttachPos - LoadedScenes[indexInLoaded].AttachPos;
            thisS.EnableObject.transform.position += displacement;
            thisS.DisableObject.transform.position += displacement;

        }
        
        Anteater.gameObject.SetActive(true);
        Armadillo.gameObject.SetActive(true);

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
    private IEnumerator UnloadAll()
    {
        //CurrentWorld = -1;
        //CurrentLevel = -1;

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
    /// <param name="World">The world number. Starts at 0.</param>
    /// <param name="Level">The level number. Starts at 1.</param>
    private IEnumerator PreloadLevel(int World, int Level)
    {
        //Load this scene
        var loading = SceneManager.LoadSceneAsync(GetSceneName(World, Level), LoadSceneMode.Additive);

        while (!loading.isDone)
        {
            yield return null;
        }

        SceneInfo thisS = new SceneInfo(GetSceneName(World, Level));
        thisS.DisableObject.SetActive(false);

        thisS.LevelManagerRef.LevelFullyLoaded(Anteater, Armadillo);

    }

    private IEnumerator ToNextLevel()
    {
        if (!AllLoaded)
        { //If there are no more levels left, dont go to the next level !
            yield break;
        }

        if (CurrentLevel >= LevelsInWorld(CurrentWorld))
        {
            yield return ToNextWorld();

            yield break;
        }

        CurrentLevel++;
        AllLoaded = false;
        CurrentLoaded = false;

        //Set the next scene to the active one
        SceneInfo cS = LoadedScenes[PreviousScenesLoaded];
        SceneInfo nS = LoadedScenes[PreviousScenesLoaded + 1];
        cS.LevelManagerRef.LevelUnactive();
        cS.DisableObject.SetActive(false);
        nS.DisableObject.SetActive(true);
        nS.LevelManagerRef.LevelActive(this);

        //Transition to the next scene
        Vector3 movement = cS.NextAttachPos - nS.AttachPos;
        
        //  this just moves both scenes to the right place :)
        float CurSeconds = 0;
        float timeDiff;
        while (CurSeconds < NextLevelTransitionTime)
        {
            timeDiff = Time.deltaTime / NextLevelTransitionTime;
            //camera
            Camera.main.transform.position += (nS.Cam.transform.position - cS.Cam.transform.position) * timeDiff;
            Camera.main.orthographicSize += (nS.Cam.orthographicSize - cS.Cam.orthographicSize) * timeDiff;

            //Move the scenes/levels (to simulate camera movement)
            foreach (SceneInfo SI in LoadedScenes)
            {
                if (SI.IsValid())
                {
                    SI.EnableObject.transform.position -= movement * timeDiff;
                    SI.DisableObject.transform.position -= movement * timeDiff;
                }
            }

            //Move the players with the scenes
            Anteater.transform.position -= movement * timeDiff;
            Armadillo.transform.position -= movement * timeDiff;

            CurSeconds += Time.deltaTime;

            yield return null;
        }

        //Resolve the leftover movement
        timeDiff = -(CurSeconds - NextLevelTransitionTime) / NextLevelTransitionTime;
        //camera
        Camera.main.transform.position += (nS.Cam.transform.position - cS.Cam.transform.position) * timeDiff;
        Camera.main.orthographicSize += (nS.Cam.orthographicSize - cS.Cam.orthographicSize) * timeDiff;

        //Move the scenes/levels (to simulate camera movement)
        foreach (SceneInfo SI in LoadedScenes)
        {
            if (SI.IsValid())
            {
                SI.EnableObject.transform.position -= movement * timeDiff;
                SI.DisableObject.transform.position -= movement * timeDiff;
            }
        }

        //Move the players with the scenes
        Anteater.transform.position -= movement * timeDiff;
        Armadillo.transform.position -= movement * timeDiff;


        yield return null;

        CurrentLoaded = true;


        //FINALLY, unload the first level loaded and insert the new level that should be loaded

        //Wait to unload first scene
        if (LoadedScenes[0].IsValid())
        {
            yield return SceneManager.UnloadSceneAsync(LoadedScenes[0].S);
        }

        LoadedScenes.RemoveAt(0);
        int lastLevel = CurrentLevel + FutureScenesLoaded;
        if (lastLevel <= LevelsInWorld(CurrentWorld))
        {
            yield return PreloadLevel(CurrentWorld, lastLevel);

            SceneInfo thisS = new SceneInfo(GetSceneName(CurrentWorld, lastLevel));
            int indexInLoaded = SceneCount - 1;
            LoadedScenes.Add(thisS);

            //Also remember to move the loaded scene!

            Vector3 displacement = new Vector3();
            for (int i = PreviousScenesLoaded; i < SceneCount - 1; ++i)
            {
                displacement += LoadedScenes[i].NextAttachPos - LoadedScenes[i + 1].AttachPos;
            }
            thisS.EnableObject.transform.position += displacement;
            thisS.DisableObject.transform.position += displacement;
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

    /// <summary>
    /// Currently a dummy implementation duplicate of ToNextLevel.
    /// </summary>
    /// <returns></returns>
    private IEnumerator ToPreviousLevel()
    {
        if (!AllLoaded)
        { //If there are no more levels left, dont go to the next level !
            yield break;
        }

        if (CurrentLevel <= 1)
        {
            yield return ToPreviousWorld();

            yield break;
        }

        CurrentLevel--;
        AllLoaded = false;
        CurrentLoaded = false;

        //Set the next scene to the active one
        SceneInfo cS = LoadedScenes[PreviousScenesLoaded];
        SceneInfo nS = LoadedScenes[PreviousScenesLoaded - 1];
        cS.LevelManagerRef.LevelUnactive();
        cS.DisableObject.SetActive(false);
        nS.DisableObject.SetActive(true);
        nS.LevelManagerRef.LevelActive(this);

        //Transition to the next scene
        Vector3 movement = cS.AttachPos - nS.NextAttachPos;

        //  this just moves both scenes to the right place :)
        float CurSeconds = 0;
        float timeDiff;
        while (CurSeconds < NextLevelTransitionTime)
        {
            timeDiff = Time.deltaTime / NextLevelTransitionTime;
            //camera
            Camera.main.transform.position += (nS.Cam.transform.position - cS.Cam.transform.position) * timeDiff;
            Camera.main.orthographicSize += (nS.Cam.orthographicSize - cS.Cam.orthographicSize) * timeDiff;

            //Move the scenes/levels (to simulate camera movement)
            foreach (SceneInfo SI in LoadedScenes)
            {
                if (SI.IsValid())
                {
                    SI.EnableObject.transform.position -= movement * timeDiff;
                    SI.DisableObject.transform.position -= movement * timeDiff;
                }
            }

            //Move the players with the scenes
            Anteater.transform.position -= movement * timeDiff;
            Armadillo.transform.position -= movement * timeDiff;

            CurSeconds += Time.deltaTime;

            yield return null;
        }

        //Resolve the leftover movement
        timeDiff = -(CurSeconds - NextLevelTransitionTime) / NextLevelTransitionTime;
        //camera
        Camera.main.transform.position += (nS.Cam.transform.position - cS.Cam.transform.position) * timeDiff;
        Camera.main.orthographicSize += (nS.Cam.orthographicSize - cS.Cam.orthographicSize) * timeDiff;

        //Move the scenes/levels (to simulate camera movement)
        foreach (SceneInfo SI in LoadedScenes)
        {
            if (SI.IsValid())
            {
                SI.EnableObject.transform.position -= movement * timeDiff;
                SI.DisableObject.transform.position -= movement * timeDiff;
            }
        }

        //Move the players with the scenes
        Anteater.transform.position -= movement * timeDiff;
        Armadillo.transform.position -= movement * timeDiff;

        yield return null;

        CurrentLoaded = true;


        //FINALLY, unload the last level loaded and insert the new level that should be loaded

        //Wait to unload first scene
        if (LoadedScenes[SceneCount-1].IsValid())
        {
            yield return SceneManager.UnloadSceneAsync(LoadedScenes[SceneCount - 1].S);
        }

        LoadedScenes.RemoveAt(SceneCount - 1);
        int loadLevel = CurrentLevel - PreviousScenesLoaded;
        if (loadLevel >= 1)
        {
            yield return PreloadLevel(CurrentWorld, loadLevel);

            SceneInfo thisS = new SceneInfo(GetSceneName(CurrentWorld, loadLevel));
            LoadedScenes.Insert(0, thisS);

            //Also remember to move the loaded scene!

            Vector3 displacement = new Vector3();
            for (int i = PreviousScenesLoaded; i > 0; --i)
            {
                displacement += LoadedScenes[i].AttachPos - LoadedScenes[i - 1].NextAttachPos;
            }
            thisS.EnableObject.transform.position += displacement;
            thisS.DisableObject.transform.position += displacement;
        }
        else
        {
            LoadedScenes.Insert(0, new SceneInfo(null));
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

        yield return FadeScreenCover(1.0f, 0, ResetFadeBlackTime);

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
        yield return PreloadLevel(CurrentWorld, CurrentLevel);

        curS = new SceneInfo(GetSceneName(CurrentWorld, CurrentLevel));
        LoadedScenes[PreviousScenesLoaded] = curS;

        CurrentLoaded = true;
        AllLoaded = true;

        SpawnCharacters();

        Anteater.gameObject.SetActive(true);
        Armadillo.gameObject.SetActive(true);

        curS.DisableObject.SetActive(true);

        curS.LevelManagerRef.LevelActive(this);

        StartCoroutine(FadeScreenCover(0, ResetHoldTime, ResetFadeTransparentTime));

        //CURRENT SCENE NOW LOADED
    }

    private IEnumerator ToNextWorld()
    {
        //If we are at the last level of the world, go to the next world
        if (CurrentWorld >= LevelsPerWorld.Length - (CustomLevels.Length == 0 ? 1 : 0))
        {
            //If we are at the last world, cant progress
            yield break;
        }

        AllLoaded = false;

        //Go to the next world
        CurrentWorld++;
        CurrentLevel = 1;

        yield return FadeScreenCover(1, .3f, 0.5f);

        yield return UnloadAll();

        yield return FirstLoad(CurrentWorld, CurrentLevel);

        //yield return FadeScreenCover(0, 0, 0.5f);

        AllLoaded = true; //redundant, but just in case
    }

    /// <summary>
    /// Goes to the LAST level in the previous world (if it is able to, of course)
    /// </summary>
    /// <returns></returns>
    private IEnumerator ToPreviousWorld()
    {
        //If we are at the last level of the world, go to the next world
        if (CurrentWorld <= 0)
        {
            //If we are at the first world, cant progress
            yield break;
        }

        AllLoaded = false;

        //Go to the next world
        CurrentWorld--;
        CurrentLevel = LevelsInWorld(CurrentWorld);

        yield return FadeScreenCover(1, .3f, 0.5f);

        yield return UnloadAll();

        yield return FirstLoad(CurrentWorld, CurrentLevel);

        //yield return FadeScreenCover(0, 0, 0.5f);

        AllLoaded = true; //redundant, but just in case
    }


    /// <summary>
    /// Fades the screen cover (ScreenCover) from the current alpha to the target alpha.
    /// </summary>
    /// <param name="alpha">TargetAlpha</param>
    /// <param name="secondsBefore">Seconds to wait before changing the screen cover (can be 0).</param>
    /// <param name="seconds">Seconds to take during scene cover transition.</param>
    private IEnumerator FadeScreenCover(float alpha, float secondsBefore, float seconds)
    {
        //Wait for secondsBefore seconds before doing anything
        float curTime = 0f;
        while (curTime < secondsBefore)
        {
            curTime += Time.deltaTime;

            yield return null;
        }

        //Grab initial values
        Color StartColor = ScreenCover.color;
        Color EndColor = StartColor;
        EndColor.a = alpha;

        //Change the screen cover :)
        curTime = 0f;

        while (curTime < seconds)
        {
            curTime += Time.deltaTime;

            ScreenCover.color = Color.Lerp(StartColor, EndColor, (curTime / seconds));

            yield return null;
        }

        ScreenCover.color = EndColor;

    }


    private int LevelsInWorld(int World)
    {
        if (World >= LevelsPerWorld.Length) return CustomLevels.Length;
        return LevelsPerWorld[World];
    }

    /// <summary>
    /// Returns the scene name of a level.
    /// </summary>
    /// <param name="World">The world number. Starts at 0.</param>
    /// <param name="Level">The level number. Starts at 1.</param>
    private String GetSceneName(int World, int Level)
    {
        if (World >= LevelsPerWorld.Length) return CustomLevels[Level - 1];
        return "" + World + "-" + Level;
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

        //Play Win Audio Source
        if (WinAudioSource) WinAudioSource.Play();

        StartCoroutine(ToNextLevel());

        return true;
    }

    public bool GoBackLevel()
    {
        if (!AllLoaded) return false;

        StartCoroutine(ToPreviousLevel());

        return true;
    }

    public bool GoToNextWorld()
    {
        if (!AllLoaded) return false;

        StartCoroutine(ToNextWorld());

        return true;
    }

    public bool GoToLastWorld()
    {
        if (!AllLoaded) return false;

        StartCoroutine(ToPreviousWorld());

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

    /*
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
    */
}