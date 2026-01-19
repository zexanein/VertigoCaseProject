using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "RewardPool", menuName = "Game/Reward Pool", order = 1)]
public class RewardPool : ScriptableObject
{
    [SerializeField] private RewardConfig[] rewardConfig;
    
    public List<RewardInfo> GetRandomRewards(int count)
    {
        var rewards = new List<RewardInfo>();
        var availableRewards = new List<RewardConfig>(rewardConfig);
        
        for (var i = 0; i < count; i++)
        {
            if (availableRewards.Count == 0) break;
            
            var randomIndex = Random.Range(0, availableRewards.Count);
            var config = availableRewards[randomIndex];
            var rewardInfo = new RewardInfo(config.rewardItem, Random.Range(config.minAmount, config.maxAmount));
            availableRewards.RemoveAt(randomIndex);
            rewards.Add(rewardInfo);
        }
        
        return rewards;
    }
}

[Serializable]
public class RewardConfig
{
    public RewardItem rewardItem;
    public int minAmount;
    public int maxAmount;
    public float probabilityWeight = 1f;
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
