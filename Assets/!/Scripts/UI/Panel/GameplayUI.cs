using System;
using DesignPattern.Observer;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using LongNC.UI.Data;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace LongNC.UI.Panel
{
    public class GameplayUI : BaseUIPanel
    {
        [Header("Info Display")]
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private TextMeshProUGUI moveCountText;
        [SerializeField] private Image timeBarFill;
        
        [Title("Buttons")]
        [OdinSerialize]
        private Button _restartButton;
        [OdinSerialize]
        private Button _helpButton;
        [OdinSerialize]
        private Button _settingButton;
        
        
        [OdinSerialize]
        private Button _pauseButton;
        [OdinSerialize] 
        private Button _homeButton;
        
        [Header("Time Warning")]
        [SerializeField] private Color normalTimeColor = Color.white;
        [SerializeField] private Color warningTimeColor = Color.yellow;
        [SerializeField] private Color dangerTimeColor = Color.red;
        [SerializeField] private float warningThreshold = 30f;
        [SerializeField] private float dangerThreshold = 10f;
        
        private Tween timeWarningTween;
        
        private void Awake()
        {
            SetupButtons();
        }

        private void OnEnable()
        {
            RegisterEvents();
        }
        
        private void OnDisable()
        {
            UnregisterEvents();
            timeWarningTween?.Kill();
        }
        
        private void SetupButtons()
        {
            _restartButton?.onClick.AddListener(OnRestartClicked);
            _helpButton?.onClick.AddListener(OnHelpClicked);
            _settingButton?.onClick.AddListener(OnSettingClicked);
            
            _pauseButton?.onClick.AddListener(OnPauseClicked);
        }
        
        private void RegisterEvents()
        {
            Observer.RegisterEvent(UIEventID.OnTimeChanged, OnTimeChanged);
            Observer.RegisterEvent(UIEventID.OnLevelUp, OnLevelUp);
        }
        
        private void UnregisterEvents()
        {
            Observer.RemoveEvent(UIEventID.OnTimeChanged, OnTimeChanged);
            Observer.RemoveEvent(UIEventID.OnLevelUp, OnLevelUp);
        }
        
        #region Button Handlers

        private void OnRestartClicked()
        {
            Observer.PostEvent(UIEventID.OnRestartClicked);
        }
        
        private void OnHelpClicked()
        {
            Observer.PostEvent(UIEventID.OnHelpClicked);
        }
        
        private void OnSettingClicked()
        {
            Observer.PostEvent(UIEventID.OnSettingClicked);
        }
        
        private void OnPauseClicked()
        {
            Observer.PostEvent(UIEventID.OnPauseButtonClicked);
        }
        
        #endregion
        
        #region Event Handlers
        
        private void OnTimeChanged(object param)
        {
            if (param is TimeData data)
            {
                // Update time text
                timeText.text = data.GetTimeString();
                
                // Update time bar
                if (timeBarFill != null)
                {
                    float fillAmount = data.CurrentTime / data.MaxTime;
                    timeBarFill.fillAmount = fillAmount;
                }
                
                // Change color based on time remaining
                UpdateTimeColor(data.CurrentTime);
                
                // Warning animation
                if (data.CurrentTime <= dangerThreshold)
                {
                    PlayDangerAnimation();
                }
                else if (data.CurrentTime <= warningThreshold)
                {
                    PlayWarningAnimation();
                }
                else
                {
                    StopWarningAnimation();
                }
            }
        }
        
        private void OnLevelUp(object param)
        {
            // if (param is LevelData data)
            // {
            //     levelText.text = $"Level {data.Level}";
            //     levelText.transform.DOPunchScale(Vector3.one * 0.3f, 0.5f);
            // }
        }
        
        #endregion
        
        #region Time Warning
        
        private void UpdateTimeColor(float currentTime)
        {
            Color targetColor;
            
            if (currentTime <= dangerThreshold)
            {
                targetColor = dangerTimeColor;
            }
            else if (currentTime <= warningThreshold)
            {
                targetColor = warningTimeColor;
            }
            else
            {
                targetColor = normalTimeColor;
            }
            
            timeText.color = targetColor;
            if (timeBarFill != null)
            {
                timeBarFill.color = targetColor;
            }
        }
        
        private void PlayWarningAnimation()
        {
            timeWarningTween?.Kill();
            timeWarningTween = timeText.transform
                .DOScale(Vector3.one * 1.1f, 0.5f)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }
        
        private void PlayDangerAnimation()
        {
            timeWarningTween?.Kill();
            timeWarningTween = timeText.transform
                .DOScale(Vector3.one * 1.2f, 0.3f)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }
        
        private void StopWarningAnimation()
        {
            timeWarningTween?.Kill();
            timeText.transform.DOScale(Vector3.one, 0.2f);
        }
        
        #endregion
        
        
        public void UpdateLevel(int level)
        {
            levelText.text = $"Level {level}";
        }
    }
}