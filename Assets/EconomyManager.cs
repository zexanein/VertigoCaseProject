using System;
using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    private static EconomyManager _instance;
    public static EconomyManager Instance
    {
        get
        {
            if (_instance != null) return _instance;
            
            _instance = FindObjectOfType<EconomyManager>();
            if (_instance != null) return _instance;
            
            var newInstance = new GameObject("EconomyManager");
            _instance = newInstance.AddComponent<EconomyManager>();
            DontDestroyOnLoad(newInstance);
            return _instance;
        }
    }
    
    public event Action<int> OnCoinsChanged;
    
    public int PlayerCoins { get; private set; }
    
    public void AddCoins(int amount)
    {
        if (amount == 0) return;
        
        if (amount < 0)
        {
            TrySpendCoins(amount);
            return;
        }
        
        OnCoinsChanged?.Invoke(amount);
        PlayerCoins += amount;
    }
    
    public bool TrySpendCoins(int amount)
    {
        if (PlayerCoins < amount) return false;
        
        PlayerCoins -= amount;
        OnCoinsChanged?.Invoke(-amount);
        return true;
    }
}
