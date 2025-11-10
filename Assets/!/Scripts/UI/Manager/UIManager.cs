using System;
using DesignPattern;
using DesignPattern.Observer;
using LongNC.UI.Data;
using LongNC.UI.Panel;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace LongNC.UI.Manager
{
    public class UIManager : Singleton<UIManager>
    {
        [Title("UI Panels")]
        [SerializeField] private GameplayUI _gameplayUI;
        [FormerlySerializedAs("_pauseUI")] [SerializeField] private RestartUI restartUI;
        [SerializeField] private WinUI _winUI;
        [SerializeField] private LoseUI _loseUI;
        
        [SerializeField] private HelpUI _helpUI;
        [SerializeField] private SettingUI _settingUI;

        private float _currentTimeScale;
        
        private ObserverManager<UIEventID> Observer => ObserverManager<UIEventID>.Instance;
        
        private void OnEnable()
        {
            RegisterEvents();
            // ShowStartScreen();
        }

        private void OnDisable()
        {
            UnregisterEvents();
        }
        
        private void RegisterEvents()
        {
            Observer.RegisterEvent(UIEventID.OnRestartClicked, OnRestartClicked);
            Observer.RegisterEvent(UIEventID.OnCloseRestartClicked, OnCloseRestartClicked);
            Observer.RegisterEvent(UIEventID.OnRestartButtonClicked, OnRestartButtonClicked);
            
            Observer.RegisterEvent(UIEventID.OnHelpClicked, OnHelpClicked);
            Observer.RegisterEvent(UIEventID.OnCloseHelpClicked, OnCloseHelpClicked);
            
            Observer.RegisterEvent(UIEventID.OnSettingClicked, OnSettingClicked);
            Observer.RegisterEvent(UIEventID.OnCloseSettingClicked, OnCloseSettingClicked);
            
            Observer.RegisterEvent(UIEventID.OnStartGame, OnStartGame);
            Observer.RegisterEvent(UIEventID.OnPauseGame, OnPauseGame);
            Observer.RegisterEvent(UIEventID.OnResumeGame, OnResumeGame);
            Observer.RegisterEvent(UIEventID.OnWinGame, OnWinGame);
            Observer.RegisterEvent(UIEventID.OnLoseGame, OnLoseGame);
            Observer.RegisterEvent(UIEventID.OnCloseButtonClicked, OnCloseClicked);
        }
        
        private void UnregisterEvents()
        {
            Observer.RemoveEvent(UIEventID.OnRestartClicked, OnRestartClicked);
            Observer.RemoveEvent(UIEventID.OnCloseRestartClicked, OnCloseRestartClicked);
            Observer.RemoveEvent(UIEventID.OnRestartButtonClicked, OnRestartButtonClicked);
            
            Observer.RemoveEvent(UIEventID.OnHelpClicked, OnHelpClicked);
            Observer.RemoveEvent(UIEventID.OnCloseHelpClicked, OnCloseHelpClicked);
            
            Observer.RemoveEvent(UIEventID.OnSettingClicked, OnSettingClicked);
            Observer.RemoveEvent(UIEventID.OnCloseSettingClicked, OnCloseSettingClicked);
            
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
            restartUI?.Hide(true);
            _winUI?.Hide(true);
            _loseUI?.Hide(true);
        }
        
        #endregion
        
        #region Event Handlers

        private void OnRestartClicked(object param)
        {
            // TODO:
        }

        private void OnCloseRestartClicked(object param)
        {
            // TODO:
        }

        private void OnRestartButtonClicked(object param)
        {
            // TODO:
        }
        
        private void OnHelpClicked(object param)
        {
            SetContinueGame(false);
            _gameplayUI?.SetControl(false);
            _helpUI?.Show();
        }

        private void OnCloseHelpClicked(object param)
        {
            SetContinueGame();
            _gameplayUI?.SetControl();
            _helpUI?.Hide();
        }
        
        private void OnSettingClicked(object param)
        {
            SetContinueGame(false);
            _gameplayUI?.SetControl(false);
            _settingUI?.Show();
        }

        private void OnCloseSettingClicked(object param)
        {
            SetContinueGame();
            _gameplayUI?.SetControl();
            _settingUI?.Hide();
        }
        
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
            restartUI?.Show();
        }
        
        private void OnResumeGame(object param)
        {
            restartUI?.Hide();
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
            restartUI?.Hide();
        }
        
        #endregion

        #region Core Buttons

        public void OnHelp()
        {
            
        }

        #endregion

        private void SetContinueGame(bool value = true)
        {
            if (value)
            {
                Time.timeScale = _currentTimeScale;
            }
            else
            {
                _currentTimeScale = Time.timeScale;
                Time.timeScale = 0;
            }
        }
    }
}