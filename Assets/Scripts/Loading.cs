using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using LongNC.Manager;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
    public Slider _slider;
    public float _timeLoading;
    public Button _play;

    private void Start()
    {
        SoundManager.Instance.PlayFX(SoundId.Background, true);
        
        _slider.gameObject.SetActive(false);
        _play.gameObject.SetActive(true);
        _play.onClick.AddListener(() =>
        {
            _play.gameObject.SetActive(false);
            _slider.gameObject.SetActive(true);
            _slider.DOValue(1f, _timeLoading).OnComplete(delegate
            {
                _slider.gameObject.SetActive(false);
                SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
            });
        });
    }
}
