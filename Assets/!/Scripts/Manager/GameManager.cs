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
            var curLevel = PlayerPrefs.GetInt("CurrentLevel");
            if (curLevel < 1) curLevel = 1;
            LevelManager.Instance.LoadLevel(curLevel);
            LevelManager.Instance.LoadAllObjInLevel();
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