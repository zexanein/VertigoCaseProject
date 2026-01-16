using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpinWheelItemDisplay : MonoBehaviour
{
    [SerializeField] private Image rewardImage;
    [SerializeField] private TMP_Text rewardAmountText;

    public void SetRewardVisual(Sprite icon, int amount)
    {
        rewardImage.sprite = icon;
        rewardAmountText.text = "x" + amount;
    }
    
    public void SetBombVisual(Sprite bombIcon)
    {
        rewardImage.sprite = bombIcon;
        rewardAmountText.text = "";
    }
}
