using System;
using DesignPattern.Observer;
using DG.Tweening;
using LongNC.UI.Data;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.UI;

namespace LongNC.UI.Panel
{
    public class SettingUI : BaseUIPanel
    {
        [Title("Sound")]
        [OdinSerialize]
        private Slider _sliderSound;
        
        [Title("Close Settings")]
        [OdinSerialize]
        private Button _closeButton;

        private Tween _tween;

        private void Awake()
        {
            Setup();
        }

        #region Setup

        private void Setup()
        {
            SetupButtons();
            SetupSlider();
        }

        private void SetupButtons()
        {
            _closeButton?.onClick.AddListener(OnCloseButtonClicked);
        }

        private void SetupSlider()
        {
            _sliderSound?.onValueChanged.AddListener(OnSliderValueChanged);
        }

        #endregion

        #region Handle

        private void OnCloseButtonClicked()
        {
            Observer.PostEvent(UIEventID.OnCloseSettingClicked, _closeButton.transform);
        }

        private void OnSliderValueChanged(float value)
        {
            Observer.PostEvent(UIEventID.OnSoundChanged, (value, _sliderSound.maxValue));
        }

        #endregion
    }
}