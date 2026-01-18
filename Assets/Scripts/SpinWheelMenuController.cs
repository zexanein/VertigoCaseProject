using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SpinWheelMenuController : MonoBehaviour
{
    [SerializeField] private SpinWheel spinWheel;
    [SerializeField] private Button spinButton;
    [SerializeField] private SpinWheelItemDisplay rewardDisplayPrefab;
    [SerializeField] private Transform inventoryContentParent;
    [SerializeField] private RewardPool[] rewardPoolsByTier;
    private readonly Dictionary<RewardItem, (SpinWheelItemDisplay display, int amount)> _rewardInventory = new();
    private int _currentZone = 1;
    private int _currentTier = 0;

    private void Start()
    {
        spinButton.onClick.AddListener(OnSpinClicked);
        spinWheel.GenerateRewards(rewardPoolsByTier[_currentTier], 8);
    }


    private void OnValidate()
    {
        if (spinButton == null)
            spinButton = transform.Find("ui_button_spinwheel_spin").GetComponent<Button>();
        
        if (spinWheel == null)
            spinWheel = transform.Find("ui_spinwheel").GetComponent<SpinWheel>();
        
        if (inventoryContentParent == null)
            inventoryContentParent = transform.Find("ui_collected_rewards/ui_layout_collected_rewards");
        
    }

    private void OnSpinClicked()
    {
        spinWheel.Spin(OnSpinComplete);
    }

    private void OnSpinComplete(RewardInfo reward, bool isBomb)
    {
        if (!isBomb) AddReward(reward);
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
}
