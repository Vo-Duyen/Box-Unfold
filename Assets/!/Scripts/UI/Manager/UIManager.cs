using System;
using DesignPattern;
using DesignPattern.Observer;
using LongNC.UI.Data;
using LongNC.UI.Panel;
using UnityEngine;

namespace LongNC.UI.Manager
{
    public class UIManager : Singleton<UIManager>
    {
        public static UIManager Instance { get; private set; }
        
        [Header("UI Panels")]
        [SerializeField] private GameplayUI _gameplayUI;
        [SerializeField] private PauseUI _pauseUI;
        [SerializeField] private WinUI _winUI;
        [SerializeField] private LoseUI _loseUI;
        
        private ObserverManager<UIEventID> Observer => ObserverManager<UIEventID>.Instance;
        
        private void OnEnable()
        {
            RegisterEvents();
            ShowStartScreen();
        }

        private void OnDisable()
        {
            UnregisterEvents();
        }
        
        private void RegisterEvents()
        {
            Observer.RegisterEvent(UIEventID.OnStartGame, OnStartGame);
            Observer.RegisterEvent(UIEventID.OnPauseGame, OnPauseGame);
            Observer.RegisterEvent(UIEventID.OnResumeGame, OnResumeGame);
            Observer.RegisterEvent(UIEventID.OnWinGame, OnWinGame);
            Observer.RegisterEvent(UIEventID.OnLoseGame, OnLoseGame);
            Observer.RegisterEvent(UIEventID.OnCloseButtonClicked, OnCloseClicked);
        }
        
        private void UnregisterEvents()
        {
            Observer.RemoveEvent(UIEventID.OnStartGame, OnStartGame);
            Observer.RemoveEvent(UIEventID.OnPauseGame, OnPauseGame);
            Observer.RemoveEvent(UIEventID.OnResumeGame, OnResumeGame);
            Observer.RemoveEvent(UIEventID.OnWinGame, OnWinGame);
            Observer.RemoveEvent(UIEventID.OnLoseGame, OnLoseGame);
            Observer.RemoveEvent(UIEventID.OnCloseButtonClicked, OnCloseClicked);
        }
        
        #region Show Screens
        
        private void ShowStartScreen()
        {
            HideAllPanels();
        }
        
        private void ShowGameplayScreen()
        {
            HideAllPanels();
            _gameplayUI?.Show();
        }
        
        private void HideAllPanels()
        {
            _gameplayUI?.Hide(true);
            _pauseUI?.Hide(true);
            _winUI?.Hide(true);
            _loseUI?.Hide(true);
        }
        
        #endregion
        
        #region Event Handlers
        
        private void OnStartGame(object param)
        {
            ShowGameplayScreen();
            
            // if (param is LevelData data)
            // {
            //     gameplayUI?.UpdateLevel(data.Level);
            // }
        }
        
        private void OnPauseGame(object param)
        {
            _pauseUI?.Show();
        }
        
        private void OnResumeGame(object param)
        {
            _pauseUI?.Hide();
        }
        
        private void OnWinGame(object param)
        {
            // if (param is LevelData data)
            // {
            //     winUI?.Setup(data);
            //     winUI?.Show();
            // }
        }
        
        private void OnLoseGame(object param)
        {
            int score = 0;
            string message = "Time's Up!";
            
            // loseUI?.Setup(score, message);
            _loseUI?.Show();
        }
        
        private void OnCloseClicked(object param)
        {
            _pauseUI?.Hide();
        }
        
        #endregion
    }
}