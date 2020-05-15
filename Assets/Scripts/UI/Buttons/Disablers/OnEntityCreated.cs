using System;
using UnityEngine;

public class OnEntityCreated : OnConditionDisabler
{
    [SerializeField]
    private SpawnButtons _spawnButtons = null;

    private bool _conditionAlreadyMet = false;

    private void Awake()
    {
        _spawnButtons.OnFirstTreeCreated += ConditionMet;
        StatisticsManager.OnFirstLivingBeingCreated += ConditionMet;
    }

    private void OnDestroy()
    {
        _spawnButtons.OnFirstTreeCreated -= ConditionMet;
        StatisticsManager.OnFirstLivingBeingCreated -= ConditionMet;
    }

    public override void ConditionMet()
    {
        if (!_conditionAlreadyMet)
        {
            _conditionAlreadyMet = true;
            base.ConditionMet();
        }   
    }
}
