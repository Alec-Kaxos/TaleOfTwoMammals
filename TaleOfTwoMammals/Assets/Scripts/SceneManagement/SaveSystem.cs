using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance;
    private string SaveKeyLevelPrefix = "level";
    private string SaveKeyCollectiblePrefix = "collectible";

    private void Awake()
    {
        DontDestroyOnLoad(this);
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    public void SaveLevelPassedState(string levelName, int passedState)
    {
        PlayerPrefs.SetInt(SaveKeyLevelPrefix + levelName, passedState);
    }

    public bool LoadLevelPassedState(string levelName)
    {
        if (PlayerPrefs.GetInt(SaveKeyLevelPrefix + levelName) == 1)
        {
            return true;
        }
        else return false;
    }

    public void SaveCollectiblePassedState(string collectibleName, int passedState)
    {
        PlayerPrefs.SetInt(SaveKeyCollectiblePrefix + collectibleName, passedState);
    }

    public bool LoadCollectiblePassedState(string collectibleName)
    {
        if (PlayerPrefs.GetInt(SaveKeyCollectiblePrefix + collectibleName) == 1)
        {
            return true;
        }
        else return false;
    }

    public void SaveMusicSettings(string parameterName, float value)
    {
        PlayerPrefs.SetFloat(parameterName, value);
    }

    public float LoadMusicSettings(string parameterName)
    {
        return PlayerPrefs.GetFloat(parameterName);
    }

    public void DeleteAllSaves()
    {
        PlayerPrefs.DeleteAll();
    }
        
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
