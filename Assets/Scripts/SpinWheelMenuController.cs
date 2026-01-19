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
    [SerializeField] private Button leaveButton;
    
    // Reward Inventory
    [SerializeField] private SpinWheelItemDisplay rewardDisplayPrefab;
    [SerializeField] private Transform inventoryContentParent;
    
    // Endless Zone Number Display
    [SerializeField] private EndlessNumberLayout endlessNumberTextLayout;
    [SerializeField] private ConsentMenu bombExplosionMenu;
    [SerializeField] private ConsentMenu collectAndLeaveMenu;
    
    [SerializeField] private SpinWheelVisualData normalSpinWheelVisualData;
    [SerializeField] private SpinWheelVisualData safeSpinWheelVisualData;
    [SerializeField] private SpinWheelVisualData superSpinWheelVisualData;
    
    [Header("Settings")]
    public int safeZoneFactor = 5;
    public int superZoneFactor = 30;
    public int reviveCost = 50;
    [SerializeField] private RewardPool[] rewardPoolsByTier;
    
    [Header("Progression Settings")]
    [SerializeField] private int spinsPerTier = 10;
    [SerializeField] private float baseMultiplierPerTier = 0.5f;
    [SerializeField] private float progressMultiplierPerTier = 0.3f;
    [SerializeField] private float superZoneMultiplierBonus = 0.5f;
    
    // Private Fields
    private readonly Dictionary<RewardItem, (SpinWheelItemDisplay display, int amount)> _rewardInventory = new();
    private int _currentTier;
    private int _totalSpins;
    private ConsentButtonData _bombContinueButtonData;
    private ConsentButtonData _bombGiveUpButtonData;

    private void Start()
    {
        _bombContinueButtonData = new ConsentButtonData(
            buttonAction: OnBombContinue,
            buttonText: "COST: " + reviveCost + "\nREVIVE",
            interactableCondition: () => EconomyManager.Instance.PlayerGolds >= reviveCost);

        _bombGiveUpButtonData = new ConsentButtonData(buttonAction: OnBombGiveUp);
        
        spinButton.onClick.AddListener(OnSpinClicked);
        leaveButton.onClick.AddListener(OnLeaveClicked);
        
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
        
        if (leaveButton == null)
            leaveButton = transform.Find("ui_container_collected_rewards/ui_button_spinwheel_leave").GetComponent<Button>();
    }

    private void OnSpinClicked()
    {
        ToggleButtons(false);
        spinWheel.Spin(OnSpinComplete);
    }
    
    private void OnLeaveClicked()
    {
        collectAndLeaveMenu.Show(
            new ConsentButtonData(buttonAction: OnLeaveConfirmed),
            new ConsentButtonData(buttonAction: collectAndLeaveMenu.Hide));
    }
    
    private void OnLeaveConfirmed()
    {
        collectAndLeaveMenu.Hide();
        CollectRewards();
        ToggleButtons(false);
        Reset();
    }

    private void OnSpinComplete(RewardInfo reward, bool isBomb)
    {
        if (isBomb)
        {
            TriggerBomb();
            return;
        }
        
        AddReward(reward);
        spinWheel.TryGetRewardDisplay(reward.rewardItem, out var wheelRewardDisplay);
    
        if (wheelRewardDisplay != null)
            StartCoroutine(MoveIconToInventory(wheelRewardDisplay, _rewardInventory[reward.rewardItem].display));
    }

    private void AddReward(RewardInfo reward)
    {
        if (_rewardInventory.ContainsKey(reward.rewardItem))
        {
            var existingReward = _rewardInventory[reward.rewardItem];
            existingReward.amount += reward.amount;
            existingReward.display.SetRewardVisual(reward.rewardItem.RewardIcon, existingReward.amount);
            _rewardInventory[reward.rewardItem] = existingReward;
        }
        
        else
        {
            var newDisplay = Instantiate(rewardDisplayPrefab, inventoryContentParent);
            newDisplay.SetRewardVisual(reward.rewardItem.RewardIcon, reward.amount);
            _rewardInventory.Add(reward.rewardItem, (newDisplay, reward.amount));
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
        _totalSpins++;
        
        UpdateTier();
        
        var multiplier = CalculateRewardMultiplier();
        
        var includeBomb = endlessNumberTextLayout.Value % safeZoneFactor != 0;
        var regenerateTween = spinWheel.RegenerateRewards(rewardPoolsByTier[_currentTier], 8, includeBomb, multiplier);
        regenerateTween.onComplete += () => { ToggleButtons(true); };
        
        UpdateWheelVisual();
    }
    
    private void ToggleButtons(bool state)
    {
        spinButton.interactable = state;
        leaveButton.interactable = state;
    }
    
    private void UpdateTier()
    {
        _currentTier = Mathf.Min(_totalSpins / spinsPerTier, rewardPoolsByTier.Length - 1);
    }
    
    private float CalculateRewardMultiplier()
    {
        var tierMultiplier = 1f + _currentTier * baseMultiplierPerTier;
        
        var tierProgress = _totalSpins % spinsPerTier / (float)spinsPerTier;
        var progressMultiplier = tierProgress * progressMultiplierPerTier;
        
        var isSuperZone = endlessNumberTextLayout.Value % superZoneFactor == 0;
        var superZoneBonus = isSuperZone ? superZoneMultiplierBonus : 0f;
        
        var finalMultiplier = tierMultiplier + progressMultiplier + superZoneBonus;
        
        return finalMultiplier;
    }
    
    private void UpdateWheelVisual()
    {
        if (endlessNumberTextLayout.Value % superZoneFactor == 0)
            spinWheel.SetVisual(superSpinWheelVisualData);
        else if (endlessNumberTextLayout.Value % safeZoneFactor == 0)
            spinWheel.SetVisual(safeSpinWheelVisualData);
        else
            spinWheel.SetVisual(normalSpinWheelVisualData);
    }
    
    private void CollectRewards()
    {
        foreach (var rewardEntry in _rewardInventory)
        {
            Debug.Log("Collecting Reward: " + rewardEntry.Key.RewardId + " x" + rewardEntry.Value.amount);
            CollectReward(rewardEntry.Key, rewardEntry.Value.amount);
        }
    }

    private void CollectReward(RewardItem reward, int amount)
    {
        switch (reward.RewardId)
        {
            case "gold":
                EconomyManager.Instance.AddGolds(amount);
                break;
        }
    }
    
    private void TriggerBomb()
    {
        bombExplosionMenu.Show(_bombContinueButtonData, _bombGiveUpButtonData);
    }

    private void OnBombContinue()
    {
        if (!EconomyManager.Instance.TrySpendGolds(reviveCost))
        {
            Debug.Log("Not enough gold to revive!");
            OnBombGiveUp();
            return;
        }
        
        bombExplosionMenu.Hide();
        
        var multiplier = CalculateRewardMultiplier();
        var regenerateTween = spinWheel.RegenerateRewards(rewardPoolsByTier[_currentTier], 8, includeBomb: true, multiplier);
        regenerateTween.onComplete += () => { ToggleButtons(true); };
    }
    
    private void OnBombGiveUp()
    {
        bombExplosionMenu.Hide();
        Reset();
    }

    private void Reset()
    {
        foreach (var rewardInventoryValue in _rewardInventory.Values)
        {
            Destroy(rewardInventoryValue.display.gameObject);
        }
        
        
        spinWheel.SetVisual(normalSpinWheelVisualData);
        _rewardInventory.Clear();
        endlessNumberTextLayout.ResetValue();
        _currentTier = 0;
        _totalSpins = 0;
        var regenerateTween = spinWheel.RegenerateRewards(rewardPoolsByTier[_currentTier], 8, includeBomb: true);
        regenerateTween.onComplete += () => { ToggleButtons(true); };
    }
}