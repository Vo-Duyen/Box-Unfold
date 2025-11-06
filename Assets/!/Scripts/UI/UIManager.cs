using DesignPattern;
using DesignPattern.Observer;
using DG.Tweening;
using LongNC.Manager;
using LongNC.UI;
using UnityEngine;

namespace LongNC.UI
{
    public enum GameEventUI
    {
        StartGame,
        Pause,
        Continue,
        Win,
        Lose,
    }
    
    public class UIManager : Singleton<UIManager>
    {
        private float _currentTimeScale;
        
        public void StartGame()
        {
            
        }
        
        public void LoadNextLevel()
        {
            LevelManager.Instance.ClearCurrentLevel();
            LevelManager.Instance.LoadNextLevel();
        }

        public void PanelWin()
        {
            Post(GameEventUI.Pause);
            // _panelWin.SetActive(true);
        }

        public void PanelLose()
        {
        }


        private void Post(GameEventUI state)
        {
            ObserverManager<GameEventUI>.Instance.PostEvent(state);
        }
    }
}