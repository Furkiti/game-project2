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
    [SerializeField]private RectTransform gameCompletedCanvas;
    [SerializeField]private RectTransform gameFailedCanvas;
    
    [Header("Buttons")]
    [SerializeField] private Button tapToStartButton;
    [SerializeField] private Button continueButton;
    
    [Header("Others")]
    [SerializeField] private TextMeshProUGUI levelTMP;
    
    private List<UIElement> _mainMenuUIElements = new List<UIElement>();
    private List<UIElement> _inGameUIElements = new List<UIElement>();
    private List<UIElement> _gameCompletedUIElements = new List<UIElement>();
    private List<UIElement> _gameFailedUIElements = new List<UIElement>();
    
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
        foreach (UIElement uiElement in gameCompletedCanvas.gameObject.GetComponentsInChildren<UIElement>())
        {
            _gameCompletedUIElements.Add(uiElement);
        }
        foreach (UIElement uiElement in gameFailedCanvas.gameObject.GetComponentsInChildren<UIElement>())
        {
            _gameFailedUIElements.Add(uiElement);
        }
    }

    private void OnEnable()
    {
        EventManager.OnGameLoaded += OnGameLoaded;
        EventManager.OnGameStarted += OnGameStarted;
        EventManager.OnGameFailed += OnGameFailed;
        EventManager.OnGameCompleted += OnGameCompleted;
        EventManager.OnGameContinue += OnGameContinue;
        EventManager.OnGameReset += OnGameReset;
        tapToStartButton.onClick.AddListener(OnTapToStartButtonClicked);
        continueButton.onClick.AddListener(OnContinueButtonClicked);
    }
    
    private void OnDisable()
    {
        EventManager.OnGameLoaded -= OnGameLoaded;
        EventManager.OnGameStarted -= OnGameStarted;
        EventManager.OnGameFailed -= OnGameFailed;
        EventManager.OnGameCompleted -= OnGameCompleted;
        EventManager.OnGameContinue -= OnGameContinue;
        EventManager.OnGameReset -= OnGameReset;
        tapToStartButton.onClick.RemoveListener(OnTapToStartButtonClicked);
        continueButton.onClick.RemoveListener(OnContinueButtonClicked);
    }
    
    
    private void OnGameLoaded()
    {
        ActivateUIElements(_mainMenuUIElements);
    }
    
    private void OnGameStarted()
    {
        DeactivateUIElements(_mainMenuUIElements);
        ActivateUIElements(_inGameUIElements);
    }

    private void OnGameCompleted()
    {
        DeactivateUIElements(_inGameUIElements);
        ActivateUIElements(_gameCompletedUIElements);
    }

    private void OnGameContinue()
    {
        DeactivateUIElements(_gameCompletedUIElements);
        ActivateUIElements(_mainMenuUIElements);
    }

    private void OnGameFailed()
    {
        DeactivateUIElements(_inGameUIElements);
        ActivateUIElements(_gameFailedUIElements);
    }

    private void OnGameReset()
    {
        DeactivateUIElements(_gameFailedUIElements);
        ActivateUIElements(_mainMenuUIElements);
    }

    private void OnTapToStartButtonClicked()
    {
        GameManager.Instance.ChangeState(GameManager.Instance.gmStartState);
    }
    
    private void OnContinueButtonClicked()
    {
        GameManager.Instance.ChangeState(GameManager.Instance.gmContinue);
    }

    public void SetLevelText(int level)
    {
        levelTMP.text = "Level " + level;
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
