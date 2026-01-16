using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SpinWheel : MonoBehaviour
{
    
    private const int NumberOfRewards = 8;
    private const float RewardRadius = 170;
    private int RewardAngle => 360 / NumberOfRewards;
    
    [SerializeField] private Transform spinWheelTransform;
    [SerializeField] private Button spinButton;
    [SerializeField] private GameObject rewardPrefab;
    private Tween _spinTween;
    
    private void Start()
    {
        CreateRewards();
        spinButton.onClick.AddListener(Spin);
    }

    private void CreateRewards()
    {
        for (var i = 0; i < NumberOfRewards; i++)
        {
            var angle = i * RewardAngle;
            var position = Quaternion.Euler(0, 0, angle) * Vector3.up * RewardRadius;
            var reward = Instantiate(rewardPrefab, spinWheelTransform);
            reward.transform.localPosition = position;
            reward.transform.localRotation = Quaternion.Euler(0, 0, angle);
        }
    }
    
    private void Spin()
    {
        var randomSpins = Random.Range(3, 6);
        var rotateAngle = randomSpins * 360f + RewardAngle;
        _spinTween?.Kill();
        _spinTween =
            spinWheelTransform.DORotate(new Vector3(0, 0, -rotateAngle), 2f, RotateMode.FastBeyond360)
            .SetEase(Ease.InOutCubic);
    }

    private void OnValidate()
    {
        if (spinButton == null)
        {
            spinButton = transform.parent.Find("ui_button_spin").GetComponent<Button>();
        }
    }
}
