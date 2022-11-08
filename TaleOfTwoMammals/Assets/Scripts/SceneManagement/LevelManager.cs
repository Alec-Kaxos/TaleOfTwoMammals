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

    public static LevelManager Instance;

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
    /// Only called after LevelFullyLoaded. Notifies the LevelManager that this is now the level being played.
    /// </summary>
    public void LevelActive()
    {
        IsActiveLevel = true;
        RespawnAnteater();
        RespawnArmadillo();
    }

    public void LevelUnactive()
    {
        IsActiveLevel = false;
    }

    #endregion

    #region Utility Methods

    public void RespawnAnteater()
    {
        Anteater.OnRespawn();
        Anteater.transform.position = AnteaterRespawn.transform.position;
        Anteater.StopCharacter();
    }

    public void RespawnArmadillo()
    {
        Armadillo.OnRespawn();
        Armadillo.transform.position = ArmadilloRespawn.transform.position;
        Armadillo.StopCharacter();
    }

    public void Respawn()
    {
        RespawnAnteater();
        RespawnArmadillo();
    }


    #endregion

    private void Start()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        Instance = null;
    }
}
