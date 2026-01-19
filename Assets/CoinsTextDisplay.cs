using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class CoinsTextDisplay : MonoBehaviour
{
    private TMP_Text _coinsText;
    
    private void Awake()
    {
        _coinsText = GetComponent<TMP_Text>();
    }
    
    private void OnEnable()
    {
        EconomyManager.Instance.OnCoinsChanged += UpdateCoinsText;
        UpdateCoinsText(0);
    }

    private void UpdateCoinsText(int change)
    {
        _coinsText.text = "Coins: " + EconomyManager.Instance.PlayerCoins;
    }
}
