using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpinWheel : MonoBehaviour
{
    private int _numberOfRewards;
    private int _bombIndex;
    private List<RewardInfo> _rewards;
    private const float RewardRadius = 170;
    private Tween _spinTween;
    
    private int RewardAngle => 360 / _numberOfRewards;

    [SerializeField] private Sprite bombSprite;
    [SerializeField] private Transform spinWheelTransform;
    [SerializeField] private SpinWheelItemDisplay itemDisplayPrefab;
    
    public delegate void SpinCompleteHandler(RewardInfo reward, bool isBomb);
    
    
    public void GenerateRewards(RewardPool rewardPool, int numberOfRewards)
    {
        foreach (Transform child in spinWheelTransform)
        {
            Destroy(child.gameObject);
        }
        
        _numberOfRewards = numberOfRewards;
        
        var bombIndex = Random.Range(0, _numberOfRewards);
        var rewards = rewardPool.GetRandomRewards(_numberOfRewards - 1);

        _rewards = new List<RewardInfo>();
        
        for (var i = 0; i < _numberOfRewards; i++)
        {
            _rewards.Add(i == bombIndex ? null : rewards[0]);
            if (i != bombIndex) rewards.RemoveAt(0);
        }
        
        for (var i = 0; i < _numberOfRewards; i++)
        {
            var createdItem = CreateItem(i);
            
            if (i == bombIndex)
            {
                createdItem.SetBombVisual(bombSprite);
                continue;
            }

            var reward = _rewards[i];
            createdItem.SetRewardVisual(reward.rewardItem.RewardIcon, reward.GetRandomAmount());
        }
    }

    private SpinWheelItemDisplay CreateItem(int index)
    {
        var angle = index * RewardAngle;
        var position = Quaternion.Euler(0, 0, angle) * Vector3.up * RewardRadius;
        
        var createdItem = Instantiate(itemDisplayPrefab, spinWheelTransform);
        createdItem.transform.localPosition = position;
        createdItem.transform.localRotation = Quaternion.Euler(0, 0, angle);
        return createdItem;
    }
    
    public void Spin(SpinCompleteHandler onComplete = null)
    {
        var earnedIndex = Random.Range(0, _numberOfRewards);
        var earnedIsBomb = earnedIndex == _bombIndex;
        var earnedReward = earnedIsBomb ? null : _rewards[earnedIndex];
        
        var randomSpins = Random.Range(4, 7);
        var rotateAngle = earnedIndex * RewardAngle + randomSpins * 360;
        
        _spinTween?.Kill();
        _spinTween = spinWheelTransform
                .DORotate(new Vector3(0, 0, -rotateAngle), 3f, RotateMode.FastBeyond360)
                .SetEase(Ease.InOutCubic)
                .OnComplete(() => onComplete?.Invoke(earnedReward, earnedIsBomb));
    }
}
