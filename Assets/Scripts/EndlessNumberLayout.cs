using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class EndlessNumberLayout : MonoBehaviour
{
    public int visibleNumberTextCount = 13;
    public RectTransform numbersTextsContent;
    public TMP_Text numberTextPrefab;
    private int _lastNumber = 1;
    private float _numberTextWidth;
    private readonly Queue<TMP_Text> _queue = new();
    private Sequence _sequence;

    private int Value { get; set; } = 1;

    public void Initialize()
    {
        _numberTextWidth = numbersTextsContent.sizeDelta.x / visibleNumberTextCount;
        CreateNumberTexts();
    }

    private void CreateNumberTexts()
    {
        var initialOffset = numbersTextsContent.sizeDelta.x / 2 - (_numberTextWidth / 2);
        
        for (var i = 0; i < visibleNumberTextCount + 1; i++)
        {
            var createdNumberText = Instantiate(numberTextPrefab, numbersTextsContent.transform);
            createdNumberText.rectTransform.anchoredPosition = new Vector2(i * _numberTextWidth + initialOffset, 0);
            createdNumberText.rectTransform.sizeDelta = new Vector2(_numberTextWidth, numbersTextsContent.sizeDelta.y);
            _lastNumber = i + 1;
            createdNumberText.text = _lastNumber.ToString();
            _queue.Enqueue(createdNumberText);
        }
    }
    
    public void NextValue()
    {
        if (Value > 7) MoveLeftmostNumberTextToEnd();
        ShiftAllNumbersTextsToLeft();
        Value++;
    }
    
    private void MoveLeftmostNumberTextToEnd()
    {
        var leftmostNumberText = _queue.Dequeue();
        (leftmostNumberText.transform as RectTransform)!.anchoredPosition = Vector2.right * (visibleNumberTextCount * _numberTextWidth);
        _lastNumber++;
        leftmostNumberText.text = _lastNumber.ToString();
        _queue.Enqueue(leftmostNumberText);
    }
    
    private void ShiftAllNumbersTextsToLeft()
    {
        _sequence?.Kill();
        _sequence = DOTween.Sequence();
        
        foreach (var numberText in _queue)
        {
            var targetX = numberText.rectTransform.anchoredPosition.x - _numberTextWidth;
            _sequence.Join(numberText.rectTransform.DOAnchorPosX(targetX, 0.2f).SetEase(Ease.InOutSine));   
        }
    }
}
