using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(CanvasGroup))]
public class UI_LootInfoBox : MonoBehaviour {

    [SerializeField]
    private TextMeshProUGUI _experienceBarLevelText;

    [SerializeField]
    private TextMeshProUGUI _experienceBarExperienceText;

    [SerializeField]
    private Image _flashImage;
    private Material _flashMaterial;

    [SerializeField]
    private float _fadeInOutDuration = 0.5f;

    [SerializeField]
    private float _progressUpdateDuration = 3f;

    [SerializeField]
    private float _fadeOutDelay = 0.5f;
    private float _updateFinishedTime;

    [SerializeField]
    private float _XPTypeTextAnimationOffset = -25f;

    [SerializeField]
    private float _experienceBarFlashBrightness = 0.125f;

    [SerializeField]
    private Image _fullCircleBar;
    private Material _fullCircleBarMaterial;

    private CanvasGroup _canvasGroup;

    private bool _isFadingIn;
    private bool _isFadingOut;
    private bool _isAnimatingFadeInOut;
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

    [SerializeField]
    private float _levelUpFlashDuration = 0.25f;
    private float _levelUpFlashAnimationStep;
    private bool _isShowingLevelUpFlash;

    private void Awake() {
        if (!_initialized) {
            gameObject.SetActive(false);
        }
    }

    private void Update() {
        if (_isAnimatingFadeInOut) {
            if (_isFadingIn) {
                FadeIn();
            } else if (_isFadingOut) {
                FadeOut();
            }
            _isAnimatingFadeInOut = _isFadingIn || _isFadingOut;
        }
        if (_isAnimatingExperienceProgress) {
            UpdateXPBar();
        }
    }

    private void FadeIn() {
        _animationStepFadeInOut += Time.deltaTime / _fadeInOutDuration;
        _animationStepFadeInOut = Mathf.Clamp01(_animationStepFadeInOut);
        float fadeProgress = (float)Tweening.EaseOutCubic(_animationStepFadeInOut);
        _fullCircleBarMaterial.SetFloat("_FlashOpacity", (1f - fadeProgress) * _experienceBarFlashBrightness);
        _canvasGroup.alpha = fadeProgress;
        _experienceBarExperienceText.margin = Vector4.Lerp(new Vector4(_XPTypeTextAnimationOffset, 0, 0, 0), Vector4.zero, fadeProgress);
        _isFadingIn = _animationStepFadeInOut != 1f;
        _isAnimatingExperienceProgress = !_isFadingIn;
    }

    private void FadeOut() {
        if (Time.time - _updateFinishedTime >= _fadeOutDelay) {
            _animationStepFadeInOut -= Time.deltaTime / _fadeInOutDuration;
            _animationStepFadeInOut = Mathf.Clamp01(_animationStepFadeInOut);
            float fadeProgress = (float)Tweening.EaseOutCubic(_animationStepFadeInOut);
            _fullCircleBarMaterial.SetFloat("_Opacity", fadeProgress);
            _canvasGroup.alpha = fadeProgress;
            _isFadingOut = _animationStepFadeInOut != 0f;
        }
    }

    public void Show(PlayerStats.ExperienceType type, int previousXP, int gainedXP) {
        InitializeBasics();
        InitializeProgressAnimation(type, previousXP, gainedXP);
        bool isCurrentlyNotShowing = !_isAnimatingFadeInOut && !_isAnimatingExperienceProgress;
        if (isCurrentlyNotShowing || _isFadingOut) {
            InitializeFadeIn();
        }
        gameObject.SetActive(true);
    }

    private void InitializeBasics() {
        if (!_initialized) {
            _canvasGroup = GetComponent<CanvasGroup>();
            _fullCircleBarMaterial = _fullCircleBar.material;
            _fullCircleBarMaterial.SetFloat("_FillingAmount", 0f);
            _fullCircleBarMaterial.SetFloat("_Opacity", 0f);
            _flashMaterial = _flashImage.material;
            _fullCircleBarMaterial.SetFloat("_FlashOpacity", 0f);
            _flashMaterial.SetFloat("_AlphaBoost", 0f);
            _fullCircleBarMaterial.SetFloat("_Opacity", 1f);
            _initialized = true;
        }
    }

    private void InitializeFadeIn() {
        ResetExperienceBarVisuals();
        _isFadingIn = true;
        _isFadingOut = false;
        _animationStepFadeInOut = 0f;
        _isAnimatingFadeInOut = true;
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
        if (!_isAnimatingExperienceProgress) { // animation has been stopped and needs to be initialized again or for the first time
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
    }

    private void UpdateXPBar() {
        // xp bar needs to be updated until all of the gained xp has been animated
        if (_runningXP < _totalGainedXP) {
            _runningXP += Time.deltaTime / _progressUpdateDuration * _runningXPTarget;
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
        if (_runningLevel > _startLevel) {
            _startLevel++;
            _experienceBarLevelText.text = _startLevel.ToString();
            _levelUpFlashAnimationStep = 0;
            _isShowingLevelUpFlash = true;
        }

        if(_isShowingLevelUpFlash) {
            _levelUpFlashAnimationStep += Time.deltaTime / _levelUpFlashDuration;
            float flashProgress = Mathf.Clamp01((float)Tweening.EaseOutCirc(_levelUpFlashAnimationStep));
            _flashMaterial.SetFloat("_AlphaBoost", Mathf.Lerp(1f, 0f, flashProgress));
            _isShowingLevelUpFlash = Mathf.Clamp01(_levelUpFlashAnimationStep) != 1f;
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

    private void ResetExperienceBarVisuals() {
        _fullCircleBarMaterial.SetFloat("_FlashOpacity", 0f);
        _flashMaterial.SetFloat("_AlphaBoost", 0f);
        _fullCircleBarMaterial.SetFloat("_Opacity", 1f);
    }
}
