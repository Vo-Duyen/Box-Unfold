using DG.Tweening;
using UnityEngine;

namespace LongNC.Cube
{
    public class CubeReverse : MonoBehaviour
    {
        [SerializeField] private Collider _collider;
        [SerializeField] private CubeManager _cubeManager;

        private Tween _tween;
        
        public void ReadyReverse(float time)
        {
            _tween ??= DOVirtual.DelayedCall(time * 2, () =>
            {
                _collider.enabled = true;
            }).OnComplete(() =>
            {
                _tween = null;
            });
        }
        public void StartReverse()
        {
            _collider.enabled = false;
            _cubeManager.Reverse();
        }
    }
}