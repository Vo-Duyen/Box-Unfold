using DesignPattern.Observer;
using DG.Tweening;
using LongNC.Data;
using LongNC.UI.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LongNC.UI.Panel
{
     public class WinUI : BaseUIPanel
    {
        [Header("Display")]
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _levelText;
        
        [Header("Buttons")]
        [SerializeField] private Button _nextLevelButton;
        [SerializeField] private Button _restartButton;
        
        private ObserverManager<UIEventID> Observer => ObserverManager<UIEventID>.Instance;
        
        protected override void Awake()
        {
            base.Awake();
            SetupButtons();
        }
        
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
        
        private void OnHomeClicked()
        {
            Observer.PostEvent(UIEventID.OnHomeButtonClicked);
        }
        
        protected override void OnShow()
        {
            transform.localScale = Vector3.zero;
            transform.DOScale(Vector3.one, _fadeTime).SetEase(Ease.OutBack);
            
            _titleText.transform.localScale = Vector3.zero;
            _titleText.transform.DOScale(Vector3.one, 0.5f)
                .SetEase(Ease.OutElastic)
                .SetDelay(0.2f);
        }
    }
}