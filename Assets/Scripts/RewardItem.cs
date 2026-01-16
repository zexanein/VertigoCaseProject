using UnityEngine;

[CreateAssetMenu(fileName = "Reward Item", menuName = "Game/Reward Item")]
public class RewardItem : ScriptableObject
{
    [SerializeField] private string rewardId;
    [SerializeField] private Sprite rewardIcon;

    public string RewardId => rewardId;
    public Sprite RewardIcon => rewardIcon;
}