using DesignPattern.Observer;
using DG.Tweening;
using LongNC.Data;
using LongNC.UI.Data;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LongNC.UI.Panel
{
     public class WinUI : BaseUIPanel
    {
        [Title("Display")]
        [OdinSerialize] 
        private TextMeshProUGUI _titleText;
        [OdinSerialize] 
        private TextMeshProUGUI _levelText;
        
        [Title("Buttons")]
        [OdinSerialize] 
        private Button _nextLevelButton;
        [OdinSerialize] 
        private Button _restartButton;
        
        
        private void Awake()
        {
            SetupButtons();
        }

        #region SetupButtons

        private void SetupButtons()
        {
            _nextLevelButton?.onClick.AddListener(OnNextLevelClicked);
            _restartButton?.onClick.AddListener(OnRestartClicked);
        }
        
        private void OnNextLevelClicked()
        {
            Observer.PostEvent(UIEventID.OnNextLevelButtonClicked);
        }
        
        private void OnRestartClicked()
        {
            Observer.PostEvent(UIEventID.OnRestartButtonClicked);
        }
        
        #endregion
        
        protected override void OnShow()
        {
            transform.localScale = Vector3.zero;
            transform.DOScale(Vector3.one, fadeTime).SetEase(Ease.OutBack);
            
            _titleText.transform.localScale = Vector3.zero;
            _titleText.transform.DOScale(Vector3.one, 0.5f)
                .SetEase(Ease.OutElastic)
                .SetDelay(0.2f);
        }
    }
}