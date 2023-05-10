using System.Collections.Generic;
using DG.Tweening;
using Singleton;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class UIManager : Singleton<UIManager>
{
    [Header("Canvas Scaler")]
    [SerializeField]private CanvasScaler mainCanvasScaler;
    
    [Header("Canvases")]
    [SerializeField]private RectTransform mainMenuCanvas;
    [SerializeField]private RectTransform inGameCanvas;
    
    [SerializeField] private Button tapToStartButton;
    
    
    private List<UIElement> _mainMenuUIElements = new List<UIElement>();
    private List<UIElement> _inGameUIElements = new List<UIElement>();
    private List<UIElement> _gameCompletedUIElements = new List<UIElement>();
    
    private float _canvasX;
    private float _canvasY;
    
    private void Awake()
    {
        SetCanvasScale();
        GetUIElements();
    }
    private void SetCanvasScale()
    {
        var mainRes = mainCanvasScaler.referenceResolution;
        _canvasX = mainRes.x;
        _canvasY = mainRes.y;
    }
    
    private void GetUIElements()
    {
        foreach (UIElement uiElement in mainMenuCanvas.gameObject.GetComponentsInChildren<UIElement>())
        {
            _mainMenuUIElements.Add(uiElement);
        }
        foreach (UIElement uiElement in inGameCanvas.gameObject.GetComponentsInChildren<UIElement>())
        {
            _inGameUIElements.Add(uiElement);
        }
    }

    private void OnEnable()
    {
        EventManager.OnGameLoaded += LevelLoaded;
        EventManager.OnGameStarted += LevelStarted;
        tapToStartButton.onClick.AddListener(OnTapToStartButtonClicked);
    }
    
    private void OnDisable()
    {
        EventManager.OnGameLoaded -= LevelLoaded;
        EventManager.OnGameStarted -= LevelStarted;
        tapToStartButton.onClick.RemoveListener(OnTapToStartButtonClicked);
    }
    
    
    private void LevelLoaded()
    {
        ActivateUIElements(_mainMenuUIElements);
    }
    
    private void LevelStarted()
    {
        DeactivateUIElements(_mainMenuUIElements);
        ActivateUIElements(_inGameUIElements);
    }

    private void OnTapToStartButtonClicked()
    {
        EventManager.OnGameStarted?.Invoke();
    }

    private void LevelCompleted(int level)
    {
        DeactivateUIElements(_inGameUIElements);
        ActivateUIElements(_gameCompletedUIElements);
    }
    
    private void ActivateUIElements(List<UIElement> uiElements)
    {
        for (int i = 0; i < uiElements.Count; i++)
        {
            uiElements[i].ActivateUI();
        }
    }
    
    private void DeactivateUIElements(List<UIElement> uiElements)
    {
        for (int i = 0; i < uiElements.Count; i++)
        {
            uiElements[i].DeactivateUI();
        }
    }
    
    public void ActivateUI(RectTransform activatedUI,float duration,Ease easeType)
    {
        activatedUI.DOAnchorPos(Vector2.zero, duration).SetEase(easeType).OnComplete((() =>
        {
            activatedUI.GetComponent<UIElement>().isUIActive = true;
        }));
    }
    
    public void DeactivateUI(RectTransform deactivatedUI,Vector2 direction,float duration,Ease easeType,Vector2 reverseDirection)
    {
        deactivatedUI.DOAnchorPos(direction * (_canvasX * 2), duration).SetEase(easeType).OnComplete((() =>
        {
            ResetUIPosition(deactivatedUI,reverseDirection);
            deactivatedUI.GetComponent<UIElement>().isUIActive = false;
        }));
    }
    
    public void ResetUIPosition(RectTransform resettedUI,Vector2 reverseDirection)
    {
        resettedUI.DOAnchorPos(reverseDirection * (_canvasX * 2), 0);
    }
}
}
