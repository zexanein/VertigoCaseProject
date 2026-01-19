using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConsentMenu : MonoBehaviour
{
    [SerializeField] private Button acceptButton;
    [SerializeField] private Button declineButton;
    
    public event Action OnAccepted;
    public event Action OnDeclined;
    
    public void Show(ConsentButtonData acceptButtonData, ConsentButtonData declineButtonData)
    {
        OnAccepted = acceptButtonData.ButtonAction;
        OnDeclined = declineButtonData.ButtonAction;
        
        if (acceptButtonData.ButtonText != null)
            acceptButton.GetComponentInChildren<TMP_Text>().text = acceptButtonData.ButtonText;
        
        if (declineButtonData.ButtonText != null)
            declineButton.GetComponentInChildren<TMP_Text>().text = declineButtonData.ButtonText;
        
        if (acceptButtonData.InteractableCondition != null)
            acceptButton.interactable = acceptButtonData.InteractableCondition();
        
        if (declineButtonData.InteractableCondition != null)
            declineButton.interactable = declineButtonData.InteractableCondition();
        
        gameObject.SetActive(true);
    }
    
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        acceptButton.onClick.AddListener(HandleAccept);
        declineButton.onClick.AddListener(HandleDecline);
    }
    
    private void OnDisable()
    {
        acceptButton.onClick.RemoveListener(HandleAccept);
        declineButton.onClick.RemoveListener(HandleDecline);
    }
    
    private void HandleAccept()
    {
        OnAccepted?.Invoke();
    }

    private void HandleDecline()
    {
        OnDeclined?.Invoke();   
    }

    private void OnValidate()
    {
        if (acceptButton == null)
            acceptButton = transform.Find("ui_button_accept").GetComponent<Button>();
        
        if (declineButton == null)
            declineButton = transform.Find("ui_button_decline").GetComponent<Button>();
    }
}

public class ConsentButtonData
{
    public Action ButtonAction;
    public string ButtonText;
    public Func<bool> InteractableCondition;

    public ConsentButtonData(Action buttonAction, string buttonText = null, Func<bool> interactableCondition = null)
    {
        ButtonAction = buttonAction;
        ButtonText = buttonText;
        InteractableCondition = interactableCondition;
    }
}
