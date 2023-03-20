using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

[RequireComponent(typeof(CanvasGroup))]
public class UI_LootInfoBox : MonoBehaviour {

    [SerializeField]
    private Image _experienceBarFilling;

    [SerializeField]
    private TextMeshProUGUI _experienceBarLevelText;

    [SerializeField]
    private TextMeshProUGUI _experienceBarExperienceText;

    [SerializeField]
    private List<Image> _experienceBarSymbolElements;

    [SerializeField]
    private float _animationTime = 0.175f;

    private CanvasGroup _canvasGroup;
    private List<Material> _materialsToFade;

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
            _materialsToFade = new List<Material>();
            _canvasGroup = GetComponent<CanvasGroup>();
            foreach (Image image in _experienceBarSymbolElements) {
                _materialsToFade.Add(image.material);
            }
            _initialized = true;
        }
        gameObject.SetActive(true);
    }

    private void Update() {
        if (_isAnimatingFadeInOut) {
            foreach (Material mat in _materialsToFade) {
                _animationStepFadeInOut += _isFadingIn ? Time.deltaTime / _animationTime : -Time.deltaTime / _animationTime;
                if (_animationStepFadeInOut <= 1f && _animationStepFadeInOut >= 0f) {
                    mat.SetFloat("_Opacity", _animationStepFadeInOut);
                    _canvasGroup.alpha = _animationStepFadeInOut;
                } else {
                    _isAnimatingFadeInOut = false;
                }
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
        _experienceBarFilling.fillAmount = _fillingAmount == 1f ? 0 : _fillingAmount;
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
        _isFadingIn = true;
        _animationStepFadeInOut = 0f;
        _isAnimatingFadeInOut = true;
    }

    private void Hide() {

    }
}
