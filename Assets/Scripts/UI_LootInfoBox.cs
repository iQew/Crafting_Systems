using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;

[RequireComponent(typeof(CanvasGroup))]
public class UI_LootInfoBox : MonoBehaviour {

    [SerializeField]
    private TextMeshProUGUI _experienceBarLevelText;

    [SerializeField]
    private TextMeshProUGUI _experienceBarExperienceText;

    [SerializeField]
    private float _animationTime = 0.5f;

    [SerializeField]
    private float _fadeOutDelay = 0.5f;
    private float _updateFinishedTime;


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
            _fullCircleBarMaterial.SetFloat("_FillingAmount", 0f);
            _fullCircleBarMaterial.SetFloat("_Opacity", 0f);
            _canvasGroup = GetComponent<CanvasGroup>();
            _initialized = true;
        }
    }

    private void Update() {        
        if (_isAnimatingFadeInOut) {
            if(_isFadingIn) {
                _animationStepFadeInOut += Time.deltaTime / _animationTime;
            } else {
                if(Time.time - _updateFinishedTime >= _fadeOutDelay) {
                    _animationStepFadeInOut -= Time.deltaTime / _animationTime;
                }
            }
            
            _animationStepFadeInOut = Mathf.Clamp01(_animationStepFadeInOut);
            float opacity = (float)Tweening.EaseOutCubic(_animationStepFadeInOut);
            _fullCircleBarMaterial.SetFloat("_Opacity", opacity);
            _canvasGroup.alpha = opacity;

            if ((_isFadingIn && _animationStepFadeInOut == 1f)
                || (!_isFadingIn && _animationStepFadeInOut == 0)) {
                _isAnimatingFadeInOut = false;
            }
        }
        if (!_isAnimatingFadeInOut && _isAnimatingExperienceProgress) {
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
        if (!_isAnimatingExperienceProgress) {
            _updateFinishedTime = Time.time;
            Debug.Log("KB: time saved");
            Hide();
        }
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
        _isFadingIn = false;
        _animationStepFadeInOut = 1f;
        _isAnimatingFadeInOut = true;
    }
}
