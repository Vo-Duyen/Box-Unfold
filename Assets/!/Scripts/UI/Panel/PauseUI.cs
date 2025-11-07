using DesignPattern.Observer;
using DG.Tweening;
using LongNC.UI.Data;
using UnityEngine;
using UnityEngine.UI;

namespace LongNC.UI.Panel
{
    public class PauseUI : BaseUIPanel
    {
        [Header("Buttons")]
        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _homeButton;
        [SerializeField] private Button _closeButton;
        
        private ObserverManager<UIEventID> Observer => ObserverManager<UIEventID>.Instance;
        
        protected override void Awake()
        {
            base.Awake();
            SetupButtons();
        }
        
        private void SetupButtons()
        {
            _resumeButton?.onClick.AddListener(OnResumeClicked);
            _restartButton?.onClick.AddListener(OnRestartClicked);
            _closeButton?.onClick.AddListener(OnCloseClicked);
        }
        
        private void OnResumeClicked()
        {
            Observer.PostEvent(UIEventID.OnResumeButtonClicked);
        }
        
        private void OnRestartClicked()
        {
            Observer.PostEvent(UIEventID.OnRestartButtonClicked);
        }
        
        private void OnCloseClicked()
        {
            Observer.PostEvent(UIEventID.OnCloseButtonClicked);
        }
        
        protected override void OnShow()
        {
            transform.localScale = Vector3.zero;
            transform.DOScale(Vector3.one, fadeTime).SetEase(Ease.OutBack);
        }
    }
}