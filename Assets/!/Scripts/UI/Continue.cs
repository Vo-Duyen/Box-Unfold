using System;
using JetBrains.Annotations;
using UnityEngine;

namespace LongNC.UI
{
    public class Continue : UIBase
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            
            Regis(GameEventUI.Continue);
        }

        private void OnDisable()
        {
            Remove(GameEventUI.Continue);
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