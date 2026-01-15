using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SpinWheel : MonoBehaviour
{
    [SerializeField] private Transform spinWheelTransform;
    [SerializeField] private Button spinButton;
    private float _rewardAngle = 45f;
    private Tween _spinTween;
    
    private void Start()
    {
        spinButton.onClick.AddListener(Spin);
    }
    
    private void Spin()
    {
        var randomSpins = Random.Range(3, 6);
        var rotateAngle = randomSpins * 360f + _rewardAngle;
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
