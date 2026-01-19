using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpinWheelVisualData", menuName = "Game/SpinWheelVisualData")]
public class SpinWheelVisualData : ScriptableObject
{
    [SerializeField] private Sprite indicatorSprite;
    [SerializeField] private Sprite wheelBaseSprite;
    
    public Sprite IndicatorSprite => indicatorSprite;
    public Sprite WheelBaseSprite => wheelBaseSprite;
}
