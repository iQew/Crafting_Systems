using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

[RequireComponent(typeof(CanvasGroup))]
public class UI_LootInfoBox : MonoBehaviour {

    [SerializeField]
    private TextMeshProUGUI _experienceBarLevelText;

    [SerializeField]
    private TextMeshProUGUI _experienceBarExperienceText;

    [SerializeField]
    private float _animationTime = 0.175f;

    [SerializeField]
    private Image _fullCircleBar;
    private Material _fullCircleBarMaterial;

    private CanvasGroup _canvasGroup;

    private bool _isFadingIn = false;
    private bool _isAnimatingFadeInOut = false;
    private float _animationStepFadeInOut = 0f;

    private int _startLevel;
    private int _startXPRequirement;
    private int _gainedXP;
    private int _currentlyDisplayedXP;
    private int _currentlyDisplayedLevel;
    private float _fillingAmount;
    private bool _isAnimatingExperienceProgress;
    private float _animationStepExperienceProgress;

    private bool _initialized;

    private void Awake() {
        if (!_initialized) {
            gameObject.SetActive(false);
        }
    }

    private void Initialize() {
        if (!_initialized) {
            _fullCircleBarMaterial = _fullCircleBar.material;
            _canvasGroup = GetComponent<CanvasGroup>();
            _initialized = true;
        }
    }

    private void Update() {
        if (_isAnimatingFadeInOut) {
            _animationStepFadeInOut += _isFadingIn ? Time.deltaTime / _animationTime : -Time.deltaTime / _animationTime;
            _animationStepFadeInOut = Mathf.Max(_animationStepFadeInOut, 0f);
            _animationStepFadeInOut = Mathf.Min(_animationStepFadeInOut, 1f);
            _fullCircleBarMaterial.SetFloat("_Opacity", _animationStepFadeInOut);
            _canvasGroup.alpha = _animationStepFadeInOut;
            
            if(_animationStepFadeInOut == 1f || _animationStepFadeInOut == 0) {
                _isAnimatingFadeInOut = false;
            }
        }
        if (_isAnimatingExperienceProgress) {
            UpdateXPBar();
        }
    }

    public void Show(PlayerStats.ExperienceType type, int previousXP, int gainedXP) {
        Initialize();
        InitializeFadeIn();
        InitializeProgressAnimation(type, previousXP, gainedXP);
        gameObject.SetActive(true);
    }

    private void UpdateXPBar() {
        if (_gainedXP > 0) {
            _currentlyDisplayedXP++;
            _gainedXP--;
        }
        int requiredXP = PlayerStats.Instance.GetRequiredXPForLevelUp(_startLevel);
        if (_currentlyDisplayedXP > requiredXP) {
            _currentlyDisplayedXP -= requiredXP;
            _startLevel++;
            requiredXP = PlayerStats.Instance.GetRequiredXPForLevelUp(_startLevel);
            _experienceBarLevelText.text = _startLevel.ToString();
        }
        _fillingAmount = _currentlyDisplayedXP / (float)requiredXP;
        float fillingAmountFracted = _fillingAmount == 1f ? 0 : _fillingAmount;
        _fullCircleBarMaterial.SetFloat("_FillingAmount", fillingAmountFracted);
        _isAnimatingExperienceProgress = _gainedXP > 0;
    }

    private void InitializeProgressAnimation(PlayerStats.ExperienceType type, int startXP, int gainedXP) {
        _experienceBarExperienceText.text = "+ " + gainedXP + " " + StringHelper.EXPERIENCE_BAR_TITLE_GATHERING_FORAGING;

        _startLevel = PlayerStats.Instance.GetLevelByTotalXP(startXP);
        _startXPRequirement = PlayerStats.Instance.GetRequiredXPForLevelUp(_startLevel);
        _experienceBarLevelText.text = _startLevel.ToString();

        _gainedXP = gainedXP;
        _currentlyDisplayedXP = startXP - PlayerStats.Instance.GetTotalXPAtLevel(_startLevel);
        _currentlyDisplayedLevel = _startLevel;

        _fillingAmount = _currentlyDisplayedXP / (float)(PlayerStats.Instance.GetRequiredXPForLevelUp(_startLevel));
        _isAnimatingExperienceProgress = true;
    }

    private void InitializeFadeIn() {
        if (!_isFadingIn) {
            _isFadingIn = true;
            _animationStepFadeInOut = 0f;
            _isAnimatingFadeInOut = true;
        }
    }

    private void Hide() {

    }
}
