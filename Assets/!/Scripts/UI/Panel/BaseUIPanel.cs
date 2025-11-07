using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace LongNC.UI.Panel
{
    [RequireComponent(typeof(CanvasGroup))]
     public abstract class BaseUIPanel : SerializedMonoBehaviour
    {
        [Title("Core")]
        [OdinSerialize] 
        protected CanvasGroup canvasGroup;
        [OdinSerialize] 
        protected float fadeTime = 0.3f;
        [OdinSerialize] 
        protected bool hideOnStart = true;

        protected virtual void Awake()
        {
            if (hideOnStart)
            {
                Hide(true);
            }
        }
        
        public virtual void Show(bool immediate = false)
        {
            gameObject.SetActive(true);
            
            if (immediate)
            {
                canvasGroup.alpha = 1f;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
                OnShowComplete();
            }
            else
            {
                canvasGroup.DOKill();
                canvasGroup.DOFade(1f, fadeTime).OnComplete(() =>
                {
                    canvasGroup.interactable = true;
                    canvasGroup.blocksRaycasts = true;
                    OnShowComplete();
                });
            }
            
            OnShow();
        }
        
        public virtual void Hide(bool immediate = false)
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            
            if (immediate)
            {
                canvasGroup.alpha = 0f;
                gameObject.SetActive(false);
                OnHideComplete();
            }
            else
            {
                canvasGroup.DOKill();
                canvasGroup.DOFade(0f, fadeTime).OnComplete(() =>
                {
                    gameObject.SetActive(false);
                    OnHideComplete();
                });
            }
            
            OnHide();
        }
        
        protected virtual void OnShow() { }
        protected virtual void OnShowComplete() { }
        protected virtual void OnHide() { }
        protected virtual void OnHideComplete() { }

#if UNITY_EDITOR
        [Button]
        protected virtual void Setup()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }
#endif        
    }
}