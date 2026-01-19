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
    
    public event Action<int> OnGoldsChanged;
    
    public int PlayerGolds { get; private set; }
    
    public void AddGolds(int amount)
    {
        if (amount == 0) return;
        
        if (amount < 0)
        {
            TrySpendGolds(amount);
            return;
        }
        
        PlayerGolds += amount;
        OnGoldsChanged?.Invoke(amount);
    }
    
    public bool TrySpendGolds(int amount)
    {
        if (PlayerGolds < amount) return false;
        
        PlayerGolds -= amount;
        OnGoldsChanged?.Invoke(-amount);
        return true;
    }
}
