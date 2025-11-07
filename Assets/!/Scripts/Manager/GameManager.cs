using System;
using System.Collections;
using DesignPattern;
using DesignPattern.Observer;
using DG.Tweening;
using LongNC.Cube;
using LongNC.UI;
using LongNC.UI.Data;
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

        #region Observers

        private void RegisterObserver()
        {
            Observer.RegisterEvent(UIEventID.OnNextLevelButtonClicked, OnNextLevelButtonClicked);
            Observer.RegisterEvent(UIEventID.OnRestartButtonClicked, OnRestartButtonClicked);
        }

        private void UnregisterObserver()
        {
            Observer.RemoveEvent(UIEventID.OnNextLevelButtonClicked, OnNextLevelButtonClicked);
            Observer.RemoveEvent(UIEventID.OnRestartButtonClicked, OnRestartButtonClicked);
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


        private Coroutine _coroutine0, _coroutine1;
        public void OnNextLevelButtonClicked()
        {
            if (_coroutine0 == null)
            {
                LevelManager.Instance.LoadNextLevel();
                LevelManager.Instance.ClearCurrentLevel();
                DOVirtual.DelayedCall(.1f, () =>
                {

                    LevelManager.Instance.LoadAllObjInLevel();
                });
                _coroutine0 = StartCoroutine(IEDelay(0.7f, () =>
                {
                    _coroutine0 = null;
                }));
            }
        }
        
        public void OnRestartButtonClicked()
        {
            if (_coroutine1 == null)
            {
                LevelManager.Instance.ClearCurrentLevel();
                DOVirtual.DelayedCall(.1f, () =>
                {

                    LevelManager.Instance.LoadAllObjInLevel();
                });
                _coroutine1 = StartCoroutine(IEDelay(0.7f, () =>
                {
                    _coroutine1 = null;
                }));
            }
        }


        #endregion

        private void Start()
        {
            Application.targetFrameRate = 60;
            var curLevel = PlayerPrefs.GetInt("CurrentLevel");
            if (curLevel < 1) curLevel = 1;
            LevelManager.Instance.LoadLevel(curLevel);
            LevelManager.Instance.LoadAllObjInLevel();
        }
        
        public void WinGame(float timeDelay = 0f)
        {
            InputManager.Instance.SetIsCanControl(false);
            if (timeDelay == 0f)
            {
                // UIManager.Instance.PanelWin();
            }
            else
            {
                StartCoroutine(IEDelay(timeDelay, () =>
                {
                    Debug.LogWarning("Win level!");
                    // UIManager.Instance.PanelWin();
                }));
            }
        }

        public void LoseGame(float timeDelay = 0f)
        {
            InputManager.Instance.SetIsCanControl(false);
            if (timeDelay == 0f)
            {
                // UIManager.Instance.PanelLose();
            }
            else
            {
                StartCoroutine(IEDelay(timeDelay, () =>
                {
                    Debug.LogWarning("Lose level!");
                    // UIManager.Instance.PanelLose();
                }));
            }
        }

        private IEnumerator IEDelay(float timeDelay, Action action)
        {
            yield return WaitForSecondCache.Get(timeDelay);

            action?.Invoke();
        }
    }
}