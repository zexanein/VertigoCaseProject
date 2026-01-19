using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SpinWheelMenuController : MonoBehaviour
{
    [Header("References")]
    // Spin Wheel
    [SerializeField] private SpinWheel spinWheel;
    [SerializeField] private Button spinButton;
    
    // Reward Inventory
    [SerializeField] private SpinWheelItemDisplay rewardDisplayPrefab;
    [SerializeField] private Transform inventoryContentParent;
    
    // Endless Zone Number Display
    [SerializeField] private EndlessNumberLayout endlessNumberTextLayout;
    
    [SerializeField] private SpinWheelVisualData normalSpinWheelVisualData;
    [SerializeField] private SpinWheelVisualData safeSpinWheelVisualData;
    [SerializeField] private SpinWheelVisualData superSpinWheelVisualData;
    
    [Header("Settings")]
    public int safeZoneFactor = 5;
    public int superZoneFactor = 30;
    [SerializeField] private RewardPool[] rewardPoolsByTier;
    
    // Private Fields
    private readonly Dictionary<RewardItem, (SpinWheelItemDisplay display, int amount)> _rewardInventory = new();
    private int _currentTier = 0;

    private void Start()
    {
        spinButton.onClick.AddListener(OnSpinClicked);
        spinWheel.GenerateRewards(rewardPoolsByTier[_currentTier], 8);
        endlessNumberTextLayout.Initialize(safeZoneFactor, superZoneFactor);
    }

    private void OnValidate()
    {
        if (spinButton == null)
            spinButton = transform.Find("ui_button_spinwheel_spin").GetComponent<Button>();
        
        if (spinWheel == null)
            spinWheel = transform.Find("ui_spinwheel").GetComponent<SpinWheel>();
        
        if (inventoryContentParent == null)
            inventoryContentParent = transform.Find("ui_collected_rewards/ui_layout_collected_rewards");
        
        if (endlessNumberTextLayout == null)
            endlessNumberTextLayout = transform.Find("ui_zone_numbers").GetComponent<EndlessNumberLayout>();
    }

    private void OnSpinClicked()
    {
        spinWheel.Spin(OnSpinComplete);
    }

    private void OnSpinComplete(RewardInfo reward, bool isBomb)
    {
        if (!isBomb)
        {
            AddReward(reward);
            spinWheel.TryGetRewardDisplay(reward.rewardItem, out var wheelRewardDisplay);
        
            if (wheelRewardDisplay != null)
                StartCoroutine(MoveIconToInventory(wheelRewardDisplay, _rewardInventory[reward.rewardItem].display));
        }
    }

    private void AddReward(RewardInfo reward)
    {
        if (_rewardInventory.ContainsKey(reward.rewardItem))
        {
            var existingReward = _rewardInventory[reward.rewardItem];
            existingReward.amount += reward.Amount;
            existingReward.display.SetRewardVisual(reward.rewardItem.RewardIcon, existingReward.amount);
            _rewardInventory[reward.rewardItem] = existingReward;
        }
        
        else
        {
            var newDisplay = Instantiate(rewardDisplayPrefab, inventoryContentParent);
            newDisplay.SetRewardVisual(reward.rewardItem.RewardIcon, reward.Amount);
            _rewardInventory.Add(reward.rewardItem, (newDisplay, reward.Amount));
        }
    }
    
    private IEnumerator MoveIconToInventory(SpinWheelItemDisplay fromDisplay, SpinWheelItemDisplay toDisplay)
    {
        yield return null;
        
        var flyingIcon = Instantiate(toDisplay.GetImageTransform().gameObject, transform);
        flyingIcon.transform.position = fromDisplay.GetImageTransform().position;
        flyingIcon.transform.rotation = Quaternion.identity;
        
        yield return flyingIcon.transform.DOMove(toDisplay.GetImageTransform().position, 0.6f).SetEase(Ease.InOutQuad).WaitForCompletion();
        
        Destroy(flyingIcon.gameObject);
        OnIconMoveComplete();
    }
    
    private void OnIconMoveComplete()
    {
        endlessNumberTextLayout.NextValue();
        var includeBomb = endlessNumberTextLayout.Value % safeZoneFactor != 0;
        spinWheel.RegenerateRewards(rewardPoolsByTier[_currentTier], 8, includeBomb);
        
        if (endlessNumberTextLayout.Value % superZoneFactor == 0) spinWheel.SetVisual(superSpinWheelVisualData);
        else if (endlessNumberTextLayout.Value % safeZoneFactor == 0) spinWheel.SetVisual(safeSpinWheelVisualData);
        else spinWheel.SetVisual(normalSpinWheelVisualData);
    }
}
