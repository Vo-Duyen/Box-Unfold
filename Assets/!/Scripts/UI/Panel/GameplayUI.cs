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
        
        private void Awake()
        {
            SetupButtons();
        }
        
        private void SetupButtons()
        {
            _restartButton?.onClick.AddListener(OnRestartClicked);
            _helpButton?.onClick.AddListener(OnHelpClicked);
            _settingButton?.onClick.AddListener(OnSettingClicked);
        }
        
        #region Button Handlers

        private void OnRestartClicked()
        {
            Observer.PostEvent(UIEventID.OnRestartClicked, _restartButton.transform);
        }
        
        private void OnHelpClicked()
        {
            Observer.PostEvent(UIEventID.OnHelpClicked, _helpButton.transform);
        }
        
        private void OnSettingClicked()
        {
            Observer.PostEvent(UIEventID.OnSettingClicked, _settingButton.transform);
        }
        
        #endregion
        
        public void UpdateLevel(int level)
        {
            levelText.text = $"Level {level}";
            levelText.transform.DOPunchScale(Vector3.one * 0.5f, 0.5f);
        }
    }
}