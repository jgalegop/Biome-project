using System;

public class OnLivingBeingCreatedDisabler : OnConditionDisabler
{
    private void Awake()
    {
        StatisticsManager.OnFirstLivingBeingCreated += ConditionMet;
    }

    private void OnDestroy()
    {
        StatisticsManager.OnFirstLivingBeingCreated -= ConditionMet;
    }

    public override void ConditionMet()
    {
        base.ConditionMet();
    }
}
