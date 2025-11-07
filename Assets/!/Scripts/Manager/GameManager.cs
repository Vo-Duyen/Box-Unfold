using System;
using System.Collections;
using DesignPattern;
using LongNC.Cube;
using LongNC.UI;
using UnityEngine;

namespace LongNC.Manager
{
    public class GameManager : Singleton<GameManager>
    {
        private void Start()
        {
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