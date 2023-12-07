using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class DamageCalculator
{
    // Stores damage reports for each player
    private Dictionary<Player, PlayerDamageReport> _playerDamageReports;
    // List of damage events yet to be processed
    private List<DamageDealtReport> _pendingDamageReports;

    // Tracks the total time for DPS calculations
    public float TotalSeconds { get; private set; }


    /// <summary>
    /// Resets the calculator, clearing existing data and timers.
    /// </summary>
    public void Reset()
    {
        TotalSeconds = 0;
        _playerDamageReports = new Dictionary<Player, PlayerDamageReport>();
        _pendingDamageReports = new List<DamageDealtReport>();
    }

    /// <summary>
    /// Records a damage event for further processing.
    /// </summary>
    /// <param name="playerData">The source of the damage.</param>
    /// <param name="damageAmount">The amount of damage dealt.</param>
    /// <param name="skill">The skill associated with the damage.</param>
    /// <returns>True if it's a new damage source; false otherwise.</returns>
    public bool RecordDamageEvent(DamageSource playerData, double damageAmount, ScriptableSkill skillData)
    {
        _pendingDamageReports.Add(new DamageDealtReport(playerData.Player, damageAmount, skillData));

        if (!_playerDamageReports.ContainsKey(playerData.Player))
        {
            var wrapper = new PlayerDamageReport(playerData);
            _playerDamageReports.Add(playerData.Player, wrapper);

            // new damagesource, report.
            return true;
        }

        // existing player, no new report needed.
        return false;
    }

    /// <summary>
    /// Processes and updates damage statistics for all pending reports.
    /// </summary>
    /// <param name="elapsedTime">Elapsed time since last update.</param>
    public void ProcessPendingDamageReports(float elapsedTime)
    {
        TotalSeconds += elapsedTime;
        foreach (var report in _pendingDamageReports)
        {
            var playerReport = _playerDamageReports[report.DamageDealer];

            // Updating statistics based on each report
            playerReport.Reports.Add(report);
            UpdateTotalDamageStatistics(playerReport, report);

            // Skill-specific updates
            if (report.SkillData)
            {
                UpdateTotalDamageSkillStatistics(playerReport, report);
            }
        }

        _pendingDamageReports.Clear();
    }

    /// <summary>
    /// Retrieves sorted damage reports based on accumulated damage.
    /// </summary>
    public List<PlayerDamageReport> GetDamageReports()
    {
        // sorting reports by total accumulated damage in descending order.
        return _playerDamageReports.Values.OrderByDescending(report => report.TotalDamageStats.AccumulatedDamage).ToList();
    }

    /// <summary>
    /// Updates total damage statistics for a player.
    /// </summary>
    private void UpdateTotalDamageStatistics(PlayerDamageReport playerReport, DamageDealtReport report)
    {
        // accumulating total damage.
        playerReport.TotalDamageStats.AccumulatedDamage += report.DamageAmount;

        // calculating DPS.
        playerReport.TotalDamageStats.DamagePerSecond = playerReport.TotalDamageStats.AccumulatedDamage / TotalSeconds;
        playerReport.TotalDamageStats.DamageEventsCount++;
    }

    /// <summary>
    /// Updates skill-specific damage statistics for a player.
    /// </summary>
    private void UpdateTotalDamageSkillStatistics(PlayerDamageReport playerReport, DamageDealtReport report)
    {       
        // if the skill is new, initialize its damage statistics.
        if (!playerReport.DamageStatsBySkill.ContainsKey(report.SkillData))
        {
            playerReport.DamageStatsBySkill.Add(report.SkillData, new TotalDamageStatistics());
        }

        // updating the skill-specific damage statistics.
        var skillStats = playerReport.DamageStatsBySkill[report.SkillData];
        skillStats.AccumulatedDamage += report.DamageAmount;
        skillStats.DamagePerSecond = skillStats.AccumulatedDamage / TotalSeconds;
        skillStats.DamageEventsCount++;
    }
}

[Serializable]
public class TotalDamageStatistics
{
    public double AccumulatedDamage { get; set; }
    public double DamagePerSecond { get; set; }
    public int DamageEventsCount { get; set; }
}

[Serializable]
public class DamageSource
{
    public Player Player { get; private set; }
    public string Name { get; private set; }
    public Color Color { get; private set; }

    public DamageSource(Player source)
    {
        this.Player = source;
        this.Name = source.name;
        // we base this on the classColor.
        this.Color = source.classColor.GetColor();
    }
}

[Serializable]
public class PlayerDamageReport
{
    [Tooltip("The source of the damage.")]
    public DamageSource DamageSource;

    [Tooltip("List of individual damage events.")]
    public List<DamageDealtReport> Reports;

    [Tooltip("Aggregated damage metrics.")]
    public TotalDamageStatistics TotalDamageStats;

    [Tooltip("Damage metrics categorized by skill.")]
    public Dictionary<ScriptableSkill, TotalDamageStatistics> DamageStatsBySkill;

    /// <summary>
    /// Initializes a new instance of the PlayerDamageReport class.
    /// </summary>
    /// <param name="damageProfileData">The damage source data.</param>
    public PlayerDamageReport(DamageSource damageProfileData)
    {
        DamageSource = damageProfileData;
        Reports = new List<DamageDealtReport>();
        TotalDamageStats = new TotalDamageStatistics();
        DamageStatsBySkill = new Dictionary<ScriptableSkill, TotalDamageStatistics>();
    }
}

[Serializable]
public class DamageDealtReport
{
    public Player DamageDealer { get; }
    public double DamageAmount { get; }
    public DateTime EventTimestamp { get; }
    public ScriptableSkill SkillData { get; }

    public DamageDealtReport(Player damageDealer, double damageAmount, ScriptableSkill skillData)
    {
        DamageDealer = damageDealer;
        DamageAmount = damageAmount;
        EventTimestamp = DateTime.Now;
        SkillData = skillData;
    }
}