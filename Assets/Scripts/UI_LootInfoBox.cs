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
    private float _fadeInOutTime = 0.5f;

    [SerializeField]
    private float _progressUpdateTime = 3f;

    [SerializeField]
    private float _fadeOutDelay = 0.5f;
    private float _updateFinishedTime;


    [SerializeField]
    private Image _fullCircleBar;
    private Material _fullCircleBarMaterial;

    private CanvasGroup _canvasGroup;

    private bool _isFadingIn = false;
    private bool _isFadingOut = false;
    private bool _isAnimatingFadeInOut = false;
    private float _animationStepFadeInOut = 0f;

    private int _startLevel;
    private float _totalGainedXP;
    private bool _isAnimatingExperienceProgress;

    private bool _initialized;
    private float _runningXP;
    private float _runningXPTarget;  

    private float _currentTotalXP;
    private int _runningLevel = 1;
    private float _currentXP;
    private float _currentStartXP;

    private void Awake() {
        if (!_initialized) {
            gameObject.SetActive(false);
        }
    }

    private void Update() {
        if (_isAnimatingFadeInOut) {
            if (_isFadingIn) {
                _animationStepFadeInOut += Time.deltaTime / _fadeInOutTime;
            }
            if (_isFadingOut) {
                if (Time.time - _updateFinishedTime >= _fadeOutDelay) {
                    _animationStepFadeInOut -= Time.deltaTime / _fadeInOutTime;
                }
            }
            _animationStepFadeInOut = Mathf.Clamp01(_animationStepFadeInOut);
            float opacity = (float)Tweening.EaseOutCubic(_animationStepFadeInOut);
            _fullCircleBarMaterial.SetFloat("_Opacity", opacity);
            _canvasGroup.alpha = opacity;

            if ((_isFadingIn && _animationStepFadeInOut == 1f)) {
                _isFadingIn = false;                
            }
            if ((_isFadingOut && _animationStepFadeInOut == 0)) {
                _isFadingOut = false;
            }
            _isAnimatingFadeInOut = _isFadingIn || _isFadingOut;
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

    private void Initialize() {
        if (!_initialized) {
            _fullCircleBarMaterial = _fullCircleBar.material;
            _fullCircleBarMaterial.SetFloat("_FillingAmount", 0f);
            _fullCircleBarMaterial.SetFloat("_Opacity", 0f);
            _canvasGroup = GetComponent<CanvasGroup>();
            _initialized = true;
        }
    }

    private void InitializeFadeIn() {
        if (!_isAnimatingFadeInOut && !_isAnimatingExperienceProgress) {
            _isFadingIn = true;
            _animationStepFadeInOut = 0f;
            _isAnimatingFadeInOut = true;
        } else if(_isFadingOut) {
            _isFadingIn = true;
            _isFadingOut = false;            
        }
    }

    private void InitializeFadeOut() {
        _isFadingOut = true;
        _animationStepFadeInOut = 1f;
        _isAnimatingFadeInOut = true;
    }

    private void InitializeProgressAnimation(PlayerStats.ExperienceType type, int startXP, int gainedXP) {
        _totalGainedXP += gainedXP;
        _runningXPTarget = _totalGainedXP;
        _experienceBarExperienceText.text = "+ " + _totalGainedXP + " " + StringHelper.EXPERIENCE_BAR_TITLE_GATHERING_FORAGING;
        if (!_isAnimatingExperienceProgress) { // animation has been stopped and needs to be initialized again/for the first time
            _currentStartXP = startXP;
            _startLevel = PlayerStats.Instance.GetLevelByTotalXP(startXP);
            _experienceBarLevelText.text = _startLevel.ToString();
        } else {
            // animation was interrupted by another item pickup
            // example: animated 50 out of 100 totalGainedXP and
            // then the player picks up another item for 100 gainedXP
            // new target needs to take the previously animated 50
            // points of XP into consideration for the updated totalGainedXP
            _runningXPTarget = _totalGainedXP - _runningXP;
        }

        _isAnimatingExperienceProgress = true;
    }

    private void UpdateXPBar() {
        // xp bar needs to be updated until all of the gained xp has been animated
        if(_runningXP < _totalGainedXP) {
            _runningXP += Time.deltaTime / _progressUpdateTime * _runningXPTarget;
        }
        _runningXP = Mathf.Min(_runningXP, _totalGainedXP);

        // currentTotalXP is the total xp the player currently has at this point in time
        // currentXP is the xp that has been animated so far
        _currentTotalXP = _currentStartXP + _runningXP;
        _currentXP = _currentTotalXP - PlayerStats.Instance.GetTotalXPAtLevel(_runningLevel);  
        
        // compute currently displayed level and xp requirements for said level
        _runningLevel = PlayerStats.Instance.GetLevelByTotalXP((int)_currentTotalXP);
        float currentTarget = PlayerStats.Instance.GetRequiredXPForLevelUp(_runningLevel);

        _currentXP = Mathf.Min(_currentXP, currentTarget);

        float progress = _currentXP / (float)PlayerStats.Instance.GetRequiredXPForLevelUp(_runningLevel);
        
        // update level number text when a level up has been detected
        if(_runningLevel > _startLevel) {
            _startLevel++;
            _experienceBarLevelText.text = _startLevel.ToString();
        }

        _fullCircleBarMaterial.SetFloat("_FillingAmount", progress);
        _isAnimatingExperienceProgress = _runningXP < _totalGainedXP;

        // finishing up to prepare fade out animation and prepare values to animate again
        if (!_isAnimatingExperienceProgress) {
            _updateFinishedTime = Time.time;
            _totalGainedXP = 0;
            _runningXP = 0;
            InitializeFadeOut();
        }
    }
}
