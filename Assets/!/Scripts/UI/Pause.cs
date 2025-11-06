using System;
using DesignPattern.Observer;
using JetBrains.Annotations;
using UnityEngine;

namespace LongNC.UI
{
    public class Pause : UIBase
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            
            Regis(GameEventUI.Pause);
        }

        private void OnDisable()
        {
            Remove(GameEventUI.Pause);
        }

        protected override void OnActive([CanBeNull] Action callback)
        {
            if (callback == null)
            {
                Time.timeScale = 0;
                return;
            }
            callback?.Invoke();
        }
    }
}