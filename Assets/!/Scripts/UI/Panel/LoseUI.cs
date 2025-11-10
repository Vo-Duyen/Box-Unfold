using DesignPattern.Observer;
using DG.Tweening;
using LongNC.UI.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LongNC.UI.Panel
{
    public class LoseUI : BaseUIPanel
    {
        [Header("Display")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI messageText;
        
        [Header("Buttons")]
        [SerializeField] private Button retryButton;
        [SerializeField] private Button homeButton;
        
        
        private void Awake()
        {
            SetupButtons();
        }
        
        private void SetupButtons()
        {
            retryButton?.onClick.AddListener(OnRetryClicked);
        }
        
        private void OnRetryClicked()
        {
            Observer.PostEvent(UIEventID.OnRestartButtonClicked);
        }
        
        protected override void OnShow()
        {
            transform.localScale = Vector3.one;
            transform.DOShakeScale(0.5f, 0.3f, 10, 90f);
            
            titleText.color = new Color(1, 1, 1, 0);
            titleText.DOFade(1f, 0.5f).SetDelay(0.3f);
        }
    }
}