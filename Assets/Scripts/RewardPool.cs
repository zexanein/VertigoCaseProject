using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "RewardPool", menuName = "Game/Reward Pool", order = 1)]
public class RewardPool : ScriptableObject
{
    [SerializeField] private RewardInfo[] rewardInfo;
    
    public List<RewardInfo> GetRandomRewards(int count)
    {
        var rewards = new List<RewardInfo>();
        var availableRewards = new List<RewardInfo>(rewardInfo);
        
        for (var i = 0; i < count; i++)
        {
            if (availableRewards.Count == 0) break;
            
            var randomIndex = Random.Range(0, availableRewards.Count);
            rewards.Add(availableRewards[randomIndex]);
            availableRewards.RemoveAt(randomIndex);
        }
        
        return rewards;
    }
}

[Serializable]
public class RewardInfo
{
    public RewardItem rewardItem;
    public int minAmount;
    public int maxAmount;
    private int _amount;
    public int Amount => _amount;
    
    public void RecalculateAmount()
    {
        _amount = Random.Range(minAmount, maxAmount);
    }
}
