using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor.Events;
#endif
using UnityEngine;

public class DamageTrackerManager : MonoBehaviour
{
    [Tooltip("Stores the latest damage report for each player.")]
    public List<PlayerDamageReport> CurrentTrackedReports { get; private set; }

    [Tooltip("Update frequency for calculating damage reports.")]
    public const float UPDATE_FREQUENCY = 0.2f;

    [Tooltip("Indicates whether damage tracking is currently paused.")]
    public bool IsPaused = false;

    // event that triggers when damage reports are updated.
    public Action<List<PlayerDamageReport>> OnDamageUpdated;

    // event that's triggered when a new damage source is detected.
    public Action<DamageSource> OnNewDamageSourceDetected;

    // calculates damage.
    private DamageCalculator _damageCalculator;

    // tracks time between updates.
    private float _currentTick;

    //our localplayer.
    private Player localPlayer;

    #region Initialization

    /// <summary>
    /// Initializes the damage tracking system for the localplayer.
    /// </summary>
    public void InitializeDPS(Player _player)
    {
        localPlayer = _player;
        localPlayer.combat.onClientDamageTrackerPlayerDealsDamage += ProcessDamage;
    }

    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        _damageCalculator = new DamageCalculator();
        _damageCalculator.Reset();
        _currentTick = 0;

        Player.SubscribeToOnLocalPlayerStarted(InitializeDPS);

    }

    private void Update()
    {
        if (IsPaused || !localPlayer) return;
        UpdateDamageCalculation(Time.deltaTime);
    }

    /// <summary>
    /// Updates the damage calculation based on the elapsed time.
    /// </summary>
    /// <param name="deltaTime">The time elapsed since the last update.</param>
    public void UpdateDamageCalculation(float deltaTime)
    {
        _currentTick += deltaTime;
        if (_currentTick < UPDATE_FREQUENCY) return;

        _damageCalculator.ProcessPendingDamageReports(_currentTick);
        CurrentTrackedReports = _damageCalculator.GetDamageReports();
        OnDamageUpdated?.Invoke(CurrentTrackedReports);
        _currentTick = 0;
    }

    #endregion

    #region Damage Processing

    /// <summary>
    /// Processes a damage event from a given source.
    /// </summary>
    /// <param name="damageSource">The source of the damage.</param>
    /// <param name="damage">The amount of damage dealt.</param>
    public void ProcessDamage(Player damageSource, int damage)
    {
        if (IsPaused) return;

        if (damageSource == localPlayer || (localPlayer.party.InParty() && localPlayer.InSamePartyAs(damageSource.name)))
        {
            int skillIndex = 0; // Placeholder for skill index
            RecordDamageEvent(new DamageSource(damageSource), damage, damageSource.skills.skills[skillIndex].data);
        }
    }

    /// <summary>
    /// Records a damage event for a specific skill.
    /// </summary>
    /// <param name="skillData">The source of the damage.</param>
    /// <param name="damageAmount">The amount of damage dealt.</param>
    /// <param name="data">Additional data related to the skill used.</param>
    public void RecordDamageEvent(DamageSource skillData, double damageAmount, ScriptableSkill data)
    {
        if (IsPaused) return;

        bool isNewDamageSource = _damageCalculator.RecordDamageEvent(skillData, damageAmount, data);
        if (isNewDamageSource)
        {
            OnNewDamageSourceDetected?.Invoke(skillData);
        }
    }


    #endregion

    #region Logging Helpers

    public void PauseTracking()
    {
        IsPaused = true;
    }

    public void ResumeTracking()
    {
        IsPaused = false;
    }

    public void ToggleTracking()
    {
        IsPaused = !IsPaused;
    }

    public void ResetTracking()
    {
        PauseTracking();
        _currentTick = 0;
        _damageCalculator.Reset();
    }

    #endregion

    #region Editor
#if UNITY_EDITOR
    public void SetupDamageRPCToPlayers()
    {
        foreach (GameObject prefab in GameObject.FindFirstObjectByType<NetworkManagerMMO>().spawnPrefabs)
        {
            Debug.Log("Spawn Prefabs Count: " + GameObject.FindFirstObjectByType<NetworkManagerMMO>().spawnPrefabs.Count);

            Player player = prefab.GetComponent<Player>();
            if (player != null)
            {

                if (player)
                {
                    // adds it automatically.
                    if (!player.combat.onServerEntityDealsDamage.ContainsPersistentListener("RPCHandlePlayerDamage"))
                    {
                        // add the messages handling to player chat (IE /dance /wave etc..).
                        UnityEventTools.AddPersistentListener(player.combat.onServerEntityDealsDamage, player.combat.RPCHandlePlayerDamage);
                    }
                }
            }
        }
    }
#endif
    #endregion
}