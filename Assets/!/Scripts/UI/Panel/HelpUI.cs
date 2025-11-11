using System;
using LongNC.UI.Data;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.UI;

namespace LongNC.UI.Panel
{
    public class HelpUI : BaseUIPanel
    {
        [Title("Close Settings")]
        [OdinSerialize]
        private Button _closeButton;

        private void Awake()
        {
            SetupButtons();
        }

        private void SetupButtons()
        {
            _closeButton?.onClick.AddListener(OnCloseButtonClicked);
        }

        private void OnCloseButtonClicked()
        {
            Observer.PostEvent(UIEventID.OnCloseHelpClicked, _closeButton);
        }
    }
}