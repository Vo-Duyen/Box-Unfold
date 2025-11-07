using DG.Tweening;
using UnityEngine;

namespace LongNC.UI.Panel
{
     public abstract class BaseUIPanel : MonoBehaviour
    {
        [SerializeField] protected CanvasGroup _canvasGroup;
        [SerializeField] protected float _fadeTime = 0.3f;
        [SerializeField] protected bool _hideOnStart = true;
        
        protected virtual void Awake()
        {
            if (_canvasGroup == null)
            {
                _canvasGroup = GetComponent<CanvasGroup>();
                if (_canvasGroup == null)
                {
                    _canvasGroup = gameObject.AddComponent<CanvasGroup>();
                }
            }
            
            if (_hideOnStart)
            {
                Hide(true);
            }
        }
        
        public virtual void Show(bool immediate = false)
        {
            gameObject.SetActive(true);
            
            if (immediate)
            {
                _canvasGroup.alpha = 1f;
                _canvasGroup.interactable = true;
                _canvasGroup.blocksRaycasts = true;
                OnShowComplete();
            }
            else
            {
                _canvasGroup.DOKill();
                _canvasGroup.DOFade(1f, _fadeTime).OnComplete(() =>
                {
                    _canvasGroup.interactable = true;
                    _canvasGroup.blocksRaycasts = true;
                    OnShowComplete();
                });
            }
            
            OnShow();
        }
        
        public virtual void Hide(bool immediate = false)
        {
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            
            if (immediate)
            {
                _canvasGroup.alpha = 0f;
                gameObject.SetActive(false);
                OnHideComplete();
            }
            else
            {
                _canvasGroup.DOKill();
                _canvasGroup.DOFade(0f, _fadeTime).OnComplete(() =>
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
    }
}