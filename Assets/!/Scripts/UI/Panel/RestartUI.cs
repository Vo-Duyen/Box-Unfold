using DesignPattern.Observer;
using DG.Tweening;
using LongNC.UI.Data;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.UI;

namespace LongNC.UI.Panel
{
    public class RestartUI : BaseUIPanel
    {
        [Title("Buttons")]
        [OdinSerialize]
        private Button _closeButton;
        [OdinSerialize]
        private Button _restartButton;

        private void Awake()
        {
            SetupButtons();
        }

        private void SetupButtons()
        {
            _closeButton?.onClick.AddListener(OnCloseButtonClicked);
            _restartButton?.onClick.AddListener(OnRestartButtonClicked);
        }

        private void OnCloseButtonClicked()
        {
            Observer.PostEvent(UIEventID.OnCloseRestartClicked, _closeButton);
        }

        private void OnRestartButtonClicked()
        {
            Observer.PostEvent(UIEventID.OnRestartButtonClicked, _restartButton);
        }
    }
}