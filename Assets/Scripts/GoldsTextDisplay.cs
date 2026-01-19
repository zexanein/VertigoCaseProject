using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class GoldsTextDisplay : MonoBehaviour
{
    private TMP_Text _coinsText;
    
    private void Awake()
    {
        _coinsText = GetComponent<TMP_Text>();
    }
    
    private void OnEnable()
    {
        EconomyManager.Instance.OnGoldsChanged += UpdateGoldsText;
        UpdateGoldsText(0);
    }

    private void UpdateGoldsText(int change)
    {
        _coinsText.text = "Golds: " + EconomyManager.Instance.PlayerGolds;
    }
}
