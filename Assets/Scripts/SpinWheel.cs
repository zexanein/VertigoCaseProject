using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SpinWheel : MonoBehaviour
{
    [SerializeField] private Sprite bombSprite;
    [SerializeField] private Transform spinWheelTransform;
    [SerializeField] private Image wheelBaseImage;
    [SerializeField] private Image wheelIndicatorImage;
    [SerializeField] private SpinWheelItemDisplay itemDisplayPrefab;
    
    private int _numberOfRewards;
    private int _bombIndex;
    private Dictionary<SpinWheelItemDisplay, RewardSlotData> _rewardSlots = new();
    private const float RewardRadius = 170;
    private Tween _spinTween;
    private Tween _regenerateTween;
    
    private int RewardAngle => 360 / _numberOfRewards;
    
    public delegate void SpinCompleteHandler(RewardInfo reward, bool isBomb);
    
    public void GenerateRewards(RewardPool rewardPool, int numberOfRewards, bool includeBomb = true)
    {
        ClearExistingRewards();
        
        _numberOfRewards = numberOfRewards;
        _bombIndex = includeBomb ? Random.Range(0, _numberOfRewards) : -1;
        var rewardCount = includeBomb ? _numberOfRewards - 1 : _numberOfRewards;
        
        var availableRewards = rewardPool.GetRandomRewards(rewardCount);
        _rewardSlots.Clear();
        
        for (var i = 0; i < _numberOfRewards; i++)
        {
            var itemDisplay = CreateItemAtIndex(i);
            
            if (i != _bombIndex)
            {
                SetupRewardSlot(itemDisplay, availableRewards[0], i);
                availableRewards.RemoveAt(0);
            }
            
            else SetupBombSlot(itemDisplay, i);
        }
    }

    public void RegenerateRewards(RewardPool rewardPool, int numberOfRewards, bool includeBomb = true)
    {
        _regenerateTween?.Complete();
        
        var isHalfwayPassed = false;
        _regenerateTween = spinWheelTransform
            .DORotate(new Vector3(0, 0, 2 * 360), 1f, RotateMode.FastBeyond360)
            .SetEase(Ease.InOutBack);

        _regenerateTween.OnUpdate(() =>
        {
            if (_regenerateTween.ElapsedPercentage() <= 0.5f || isHalfwayPassed) return;
            isHalfwayPassed = true;
            GenerateRewards(rewardPool, numberOfRewards, includeBomb);
        });
        
        _regenerateTween.OnComplete(() =>
        {
            foreach (var display in _rewardSlots.Keys)
            {
                display.transform.DOPunchScale(Vector3.one * 0.5f, 0.4f, vibrato: 0).SetEase(Ease.OutBack);
            }
        });
    }

    private void ClearExistingRewards()
    {
        foreach (Transform child in spinWheelTransform)
        {
            Destroy(child.gameObject);
        }
    }

    public bool TryGetRewardDisplay(RewardItem reward, out SpinWheelItemDisplay display)
    {
        var slotData = _rewardSlots.FirstOrDefault(x => x.Value.RewardInfo != null && x.Value.RewardInfo.rewardItem == reward);
        
        if (slotData.Key != null)
        {
            display = slotData.Key;
            return true;
        }

        display = null;
        return false;
    }

    private SpinWheelItemDisplay CreateItemAtIndex(int index)
    {
        var angle = index * RewardAngle;
        var position = Quaternion.Euler(0, 0, angle) * Vector3.up * RewardRadius;
        
        var itemDisplay = Instantiate(itemDisplayPrefab, spinWheelTransform);
        itemDisplay.transform.localPosition = position;
        itemDisplay.transform.localRotation = Quaternion.Euler(0, 0, angle);
        
        return itemDisplay;
    }
    
    private void SetupBombSlot(SpinWheelItemDisplay itemDisplay, int index)
    {
        itemDisplay.SetBombVisual(bombSprite);
        _rewardSlots.Add(itemDisplay, new RewardSlotData(null, index));
    }
    
    private void SetupRewardSlot(SpinWheelItemDisplay itemDisplay, RewardInfo rewardInfo, int index)
    {
        rewardInfo.RecalculateAmount();
        itemDisplay.SetRewardVisual(rewardInfo.rewardItem.RewardIcon, rewardInfo.Amount);
        _rewardSlots.Add(itemDisplay, new RewardSlotData(rewardInfo, index));
    }

    public void Spin(SpinCompleteHandler onComplete = null)
    {
        var earnedSlotIndex = Random.Range(0, _numberOfRewards);
        var earnedSlotData = GetRewardDataAtIndex(earnedSlotIndex);
        
        var isBomb = earnedSlotIndex == _bombIndex;
        var earnedReward = isBomb ? null : earnedSlotData.RewardInfo;
        
        RotateToSlot(earnedSlotIndex, () => onComplete?.Invoke(earnedReward, isBomb));
    }
    
    private RewardSlotData GetRewardDataAtIndex(int index)
    {
        return _rewardSlots.FirstOrDefault(x => x.Value.Order == index).Value;
    }
    
    private void RotateToSlot(int slotIndex, System.Action onComplete, int overrideFullSpins = -1)
    {
        var randomFullSpins = Random.Range(4, 7);
        var targetAngle = slotIndex * RewardAngle + randomFullSpins * 360;
        
        _spinTween?.Kill();
        _regenerateTween?.Complete();
        _spinTween = spinWheelTransform
            .DORotate(new Vector3(0, 0, -targetAngle), 3f, RotateMode.FastBeyond360)
            .SetEase(Ease.InOutCubic)
            .OnComplete(() => onComplete?.Invoke());
    }

    public void SetVisual(SpinWheelVisualData visualData)
    {
        wheelBaseImage.sprite = visualData.WheelBaseSprite;
        wheelIndicatorImage.sprite = visualData.IndicatorSprite;
    }
    
    private struct RewardSlotData
    {
        public readonly RewardInfo RewardInfo;
        public readonly int Order;

        public RewardSlotData(RewardInfo rewardInfo, int order)
        {
            RewardInfo = rewardInfo;
            Order = order;
        }
    }
}