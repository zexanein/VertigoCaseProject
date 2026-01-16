using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpinWheelMenuController : MonoBehaviour
{
    [SerializeField] private SpinWheel spinWheel;
    [SerializeField] private Button spinButton;
    [SerializeField] private RewardPool[] rewardPoolsByTier;
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
        {
            spinWheel = transform.Find("ui_spinwheel").GetComponent<SpinWheel>();
            spinButton = transform.Find("ui_button_spinwheel_spin").GetComponent<Button>();
        }
    }

    private void OnSpinClicked()
    {
        spinWheel.Spin(OnSpinComplete);
    }

    private void OnSpinComplete(RewardInfo reward, bool isBomb)
    {
        
    }
}
