using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SaveSystem : MonoBehaviour
{
    [SerializeField]
    private int collectableAmount = 0;
    private int collectableCollected = 0;

    public static SaveSystem Instance;
    private string SaveKeyLevelPrefix = "level";
    private string SaveKeyCollectiblePrefix = "collectible";

    [SerializeField]
    private string[] AudioParamaters;
    [SerializeField] 
    private AudioMixer myMixer;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        DoAudioVolume();
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    public void SaveLevelPassedState(string levelName, int passedState)
    {
        string[] WorldAndLevel = levelName.Split("-");
        int level = int.Parse(WorldAndLevel[1]);
       if (level == 1)
        {
            PlayerPrefs.SetInt(SaveKeyLevelPrefix + levelName, passedState);
        }
        level = level + 1;
        WorldAndLevel[1] = level.ToString();

        levelName = WorldAndLevel[0] +"-"+ WorldAndLevel[1];
        Debug.Log(levelName);

        PlayerPrefs.SetInt(SaveKeyLevelPrefix + levelName, passedState);
    }

    public bool LoadLevelPassedState(string levelName)
    {
        if (PlayerPrefs.GetInt(SaveKeyLevelPrefix + levelName, 0) == 1)
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
        if (PlayerPrefs.GetInt(SaveKeyCollectiblePrefix + collectibleName, 0) == 1)
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
        
    public bool CanUnlockSecretLevel()
	{
        return collectableCollected == collectableAmount;
	}

    public void IncrementCollectedAmount(int i)
	{
        collectableCollected += i;
	}

    private void DoAudioVolume()
    {
        foreach (string parameter in AudioParamaters)
        {
            Debug.Log(parameter + SaveSystem.Instance.LoadMusicSettings(parameter));
            myMixer.SetFloat(parameter, SaveSystem.Instance.LoadMusicSettings(parameter));
        }
    }
}
