using System;
using JetBrains.Annotations;
using UnityEngine;

namespace LongNC.UI
{
    public class StartGame : UIBase
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            
            Regis(GameEventUI.StartGame);
        }

        private void OnDisable()
        {
            Remove(GameEventUI.StartGame);
        }

        protected override void OnActive([CanBeNull] Action callback)
        {
            
        }
    }
}