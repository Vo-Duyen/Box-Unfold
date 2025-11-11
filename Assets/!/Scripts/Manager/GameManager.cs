using System;
using System.Collections;
using DesignPattern;
using DesignPattern.Observer;
using DG.Tweening;
using LongNC.Cube;
using LongNC.UI;
using LongNC.UI.Data;
using LongNC.UI.Manager;
using UnityEngine;

namespace LongNC.Manager
{
    public class GameManager : Singleton<GameManager>
    {
        private ObserverManager<UIEventID> Observer => ObserverManager<UIEventID>.Instance;

        private int _curLevel;

        private void OnEnable()
        {
            RegisterObserver();
        }

        private void OnDisable()
        {
            UnregisterObserver();
        }
        
        private void Start()
        {
            Application.targetFrameRate = 60;
            _curLevel = PlayerPrefs.GetInt("CurrentLevel");
            if (_curLevel < 1) _curLevel = 1;
            LevelManager.Instance.LoadLevel(_curLevel);
            LevelManager.Instance.LoadAllObjInLevel();
            InputManager.Instance.SetIsCanControl();

            if (SoundManager.Instance.IsPlaying(SoundId.Background) == false)
            {
                SoundManager.Instance.PlayFX(SoundId.Background, true);
            }
        }

        #region Observers

        private void RegisterObserver()
        {
            Observer.RegisterEvent(UIEventID.OnNextLevelButtonClicked, OnNextLevelButtonClicked);
            Observer.RegisterEvent(UIEventID.OnRestartButtonClicked, OnRestartButtonClicked);
            Observer.RegisterEvent(UIEventID.OnTryAgainButtonClicked, OnTryAgainButtonClicked);
        }

        private void UnregisterObserver()
        {
            Observer.RemoveEvent(UIEventID.OnNextLevelButtonClicked, OnNextLevelButtonClicked);
            Observer.RemoveEvent(UIEventID.OnRestartButtonClicked, OnRestartButtonClicked);
            Observer.RemoveEvent(UIEventID.OnTryAgainButtonClicked, OnTryAgainButtonClicked);
        }

        #endregion

        #region Button Handlers

        private void OnNextLevelButtonClicked(object param)
        {
            LevelManager.Instance.LoadNextLevel();
            LevelManager.Instance.ClearCurrentLevel();
            LevelManager.Instance.LoadAllObjInLevel();
            ++_curLevel;
            PlayerPrefs.SetInt("CurrentLevel", _curLevel);
        }
        
        private void OnRestartButtonClicked(object param)
        {
            LevelManager.Instance.ClearCurrentLevel();
            LevelManager.Instance.LoadAllObjInLevel();
        }

        private void OnTryAgainButtonClicked(object param)
        {
            LevelManager.Instance.ClearCurrentLevel();
            LevelManager.Instance.LoadAllObjInLevel();
        }
        
        #endregion

        public void TestNextLevel()
        {
            OnNextLevelButtonClicked(null);
        }
    }
}