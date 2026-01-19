using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class EndlessNumberLayout : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private int visibleNumberTextCount = 13;
    [SerializeField] private RectTransform numbersTextsContent;
    [SerializeField] private TMP_Text numberTextPrefab;
    [SerializeField] private Image normalZoneBackground;
    [SerializeField] private Image safeZoneBackground;
    [SerializeField] private Image superZoneBackground;
    
    private int _safeZoneFactor;
    private int _superZoneFactor;
    private int _lastNumber = 1;
    private float _numberTextWidth;
    private readonly Queue<TMP_Text> _numberTexts = new();
    private Sequence _shiftSequence;
    private Sequence _backgroundSequence;

    public int Value { get; private set; } = 1;

    public void Initialize(int safeZoneFactor, int superZoneFactor)
    {
        _safeZoneFactor = safeZoneFactor;
        _superZoneFactor = superZoneFactor;
        _numberTextWidth = numbersTextsContent.sizeDelta.x / visibleNumberTextCount;
        
        CreateNumberTexts();
        UpdateBackgroundColor(true);
    }

    private void CreateNumberTexts()
    {
        var initialOffset = numbersTextsContent.sizeDelta.x / 2 - _numberTextWidth / 2;
        
        for (var i = 0; i < visibleNumberTextCount + 1; i++)
        {
            var numberText = Instantiate(numberTextPrefab, numbersTextsContent);
            numberText.rectTransform.anchoredPosition = new Vector2(i * _numberTextWidth + initialOffset, 0);
            numberText.rectTransform.sizeDelta = new Vector2(_numberTextWidth, numbersTextsContent.sizeDelta.y);
            
            _lastNumber = i + 1;
            numberText.text = _lastNumber.ToString();
            numberText.color = GetNumberColor(_lastNumber);
            
            _numberTexts.Enqueue(numberText);
        }
    }

    private Color GetNumberColor(int number)
    {
        if (number % _superZoneFactor == 0) return Color.yellow;
        if (number % _safeZoneFactor == 0) return Color.green;
        return Color.white;
    }

    public void NextValue()
    {
        var halfVisibleCount = visibleNumberTextCount / 2 + visibleNumberTextCount % 2;
        if (Value > halfVisibleCount) RecycleLeftmostNumber();
        ShiftAllNumbersLeft();
        Value++;
        UpdateBackgroundColor();
    }
    
    private void UpdateBackgroundColor(bool instant = false)
    {
        var isSafeZone = Value % _safeZoneFactor == 0;
        var isSuperZone = Value % _superZoneFactor == 0;
        
        var safeAlpha = isSafeZone ? 1f : 0f;
        var superAlpha = isSuperZone ? 1f : 0f;
        var normalAlpha = isSafeZone || isSuperZone ? 0f : 1f;
        
        _backgroundSequence?.Complete();
        
        if (instant)
        {
            SetBackgroundAlpha(normalZoneBackground, normalAlpha);
            SetBackgroundAlpha(safeZoneBackground, safeAlpha);
            SetBackgroundAlpha(superZoneBackground, superAlpha);
        }
        else
        {
            _backgroundSequence = DOTween.Sequence();
            _backgroundSequence.Join(normalZoneBackground.DOFade(normalAlpha, 0.2f));
            _backgroundSequence.Join(safeZoneBackground.DOFade(safeAlpha, 0.2f));
            _backgroundSequence.Join(superZoneBackground.DOFade(superAlpha, 0.2f));
        }
    }

    private void SetBackgroundAlpha(Image image, float alpha)
    {
        var color = image.color;
        color.a = alpha;
        image.color = color;
    }
    
    private void RecycleLeftmostNumber()
    {
        var leftmost = _numberTexts.Dequeue();
        var offset = visibleNumberTextCount % 2 == 0 ? _numberTextWidth / 2 : 0;
        var targetX = visibleNumberTextCount * _numberTextWidth + offset;
        
        leftmost.rectTransform.anchoredPosition = Vector2.right * targetX;
        _lastNumber++;
        leftmost.text = _lastNumber.ToString();
        leftmost.color = GetNumberColor(_lastNumber);
        
        _numberTexts.Enqueue(leftmost);
    }
    
    private void ShiftAllNumbersLeft()
    {
        _shiftSequence?.Complete();
        _shiftSequence = DOTween.Sequence();
        
        foreach (var numberText in _numberTexts)
        {
            var targetX = numberText.rectTransform.anchoredPosition.x - _numberTextWidth;
            _shiftSequence.Join(numberText.rectTransform.DOAnchorPosX(targetX, 0.2f).SetEase(Ease.InOutSine));
        }
    }
}