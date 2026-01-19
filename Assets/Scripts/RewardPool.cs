using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "RewardPool", menuName = "Game/Reward Pool", order = 1)]
public class RewardPool : ScriptableObject
{
    [SerializeField] private RewardConfig[] rewardConfig;
    
    public List<RewardInfo> GetRandomRewards(int count, float amountMultiplier = 1f)
    {
        var rewards = new List<RewardInfo>();
        
        for (var i = 0; i < count; i++)
        {
            var config = GetWeightedRandomReward();
            if (rewards.Any(r => r.rewardItem == config.rewardItem))
            {
                i--;
                continue;
            }
            
            if (config == null) break;
            
            var baseAmount = Random.Range(config.minAmount, config.maxAmount);
            var scaledAmount = Mathf.RoundToInt(baseAmount * amountMultiplier);
            scaledAmount = Mathf.Max(1, scaledAmount);
            scaledAmount = Mathf.Clamp(scaledAmount, config.minAmount, config.maxAmount);
            
            var rewardInfo = new RewardInfo(config.rewardItem, scaledAmount);
            rewards.Add(rewardInfo);
        }
        
        return rewards;
    }
    
    private RewardConfig GetWeightedRandomReward()
    {
        if (rewardConfig.Length == 0) return null;
        
        var validConfigs = new List<RewardConfig>();
        foreach (var config in rewardConfig)
        {
            if (config.probabilityWeight > 0)
            {
                validConfigs.Add(config);
            }
        }
        
        switch (validConfigs.Count)
        {
            case 0: return rewardConfig[0];
            case 1: return validConfigs[0];
        }

        var totalWeight = 0f;
        foreach (var config in validConfigs)
        {
            totalWeight += config.probabilityWeight;
        }
        
        var randomValue = Random.Range(0f, totalWeight);
        var currentWeight = 0f;
        
        foreach (var config in validConfigs)
        {
            currentWeight += config.probabilityWeight;
            if (randomValue < currentWeight)
            {
                return config;
            }
        }
        
        return validConfigs[^1];
    }
}

[Serializable]
public class RewardConfig
{
    public RewardItem rewardItem;
    public int minAmount;
    public int maxAmount;
    
    // This value is not normalized, just relative to other weights. 100 is not guaranteed but high probability.
    [Range(0, 100)] public float probabilityWeight = 50f;
}

[Serializable]
public class RewardInfo
{
    public RewardItem rewardItem;
    public int amount;
    
    public RewardInfo(RewardItem item, int amt)
    {
        rewardItem = item;
        amount = amt;
    }
}