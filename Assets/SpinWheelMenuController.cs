using UnityEngine;
using UnityEngine.UI;

public class SpinWheelMenuController : MonoBehaviour
{
    [SerializeField] private SpinWheel spinWheel;
    [SerializeField] private Button spinButton;

    private void Start()
    {
        spinButton.onClick.AddListener(spinWheel.Spin);
    }


    private void OnValidate()
    {
        if (spinButton == null)
        {
            spinWheel = transform.Find("ui_spinwheel").GetComponent<SpinWheel>();
            spinButton = transform.Find("ui_button_spin").GetComponent<Button>();
        }
    }
}
