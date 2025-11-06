using System;
using System.Net.NetworkInformation;
using DesignPattern.Observer;
using JetBrains.Annotations;
using UnityEngine;

namespace LongNC.UI
{
    public class UIBase : MonoBehaviour
    {
        private Action<object> _eventActive;
        
        protected virtual void OnEnable()
        {
            _eventActive = param =>
            {
                OnActive((Action) param);
            };
        }

        protected virtual void OnActive([CanBeNull] Action callback) {}

        protected virtual void Regis(GameEventUI state)
        {
            ObserverManager<GameEventUI>.Instance.RegisterEvent(state, _eventActive);
        }
        
        protected virtual void Remove(GameEventUI state)
        {
            ObserverManager<GameEventUI>.Instance.RemoveEvent(state, _eventActive);
        }
    }
}