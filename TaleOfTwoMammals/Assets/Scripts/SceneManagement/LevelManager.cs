using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private bool IsActiveLevel = false;

    [SerializeField, Tooltip("This is only seralized for testing")]
    private AnteaterController Anteater;
    [SerializeField, Tooltip("This is only seralized for testing")]
    private ArmadilloController Armadillo;

    [SerializeField]
    private GameObject AnteaterRespawn;
    [SerializeField]
    private GameObject ArmadilloRespawn;

    [SerializeField]
    private AudioClip _winClip;
    private AudioSource _audioSource;

    /// <summary>
    /// When it is NOT NULL, this level is active.
    /// </summary>
    private SceneController SC;

    //public static LevelManager Instance;

    #region Management Methods

    /// <summary>
    /// A function so the SceneManager can initialize this level
    /// </summary>
    /// <param name="Anteater">A reference to the Anteater GameObject.</param>
    /// <param name="Armadillo">A reference to the Armadillo GameObject.</param>
    public void LevelFullyLoaded(AnteaterController Ant, ArmadilloController Arm)
    {
        Anteater = Ant;
        Armadillo = Arm;
    }

    /// <summary>
    /// A function so the SceneManager can relink Anteater and Armadillo references if they were deleted
    /// </summary>
    /// <param name="Anteater">A reference to the Anteater GameObject.</param>
    /// <param name="Armadillo">A reference to the Armadillo GameObject.</param>
    public void RelinkReferences(AnteaterController Ant, ArmadilloController Arm)
    {
        Anteater = Ant;
        Armadillo = Arm;
    }

    /// <summary>
    /// Only called after LevelFullyLoaded. Notifies the LevelManager that this is now the level being played.
    /// </summary>
    public void LevelActive(SceneController s)
    {
        IsActiveLevel = true;
        SC = s;

        Anteater.SetLevelManager(this);
        Armadillo.SetLevelManager(this);
        RespawnAnteater();
        RespawnArmadillo();
    }

    public void LevelUnactive()
    {
        IsActiveLevel = false;
        SC = null;  
    }

    /// <summary>
    /// Call only when the level is finished. Will then notify the scene controller to go to the next level.
    /// </summary>
    public void LevelWon()
    {
        Debug.Log("Level Won !!!!");
        if (_audioSource != null)
		{
            _audioSource.Play();
		}
        if(SC != null)
        {
            //Do next level code

            SC.LevelCompleted();

        
        }
    }

    #endregion

    #region Utility Methods

    public void RespawnAnteater()
    {
        Anteater.transform.position = AnteaterRespawn.transform.position;
        Anteater.StopCharacter();
        Anteater.OnRespawn();
    }

    public void RespawnArmadillo()
    {
        Armadillo.transform.position = ArmadilloRespawn.transform.position;
        Armadillo.StopCharacter();
        Armadillo.OnRespawn();
    }

    public void Respawn()
    {
        RespawnAnteater();
        RespawnArmadillo();
    }


    #endregion


    #region Unity Methods
    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        //Instance = this;
    }

    private void OnDestroy()
    {
        //Instance = null;
    }
    #endregion  
}
