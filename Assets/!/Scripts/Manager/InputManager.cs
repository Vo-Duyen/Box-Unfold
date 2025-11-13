using System;
using System.Collections;
using DesignPattern;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LongNC.Cube
{
    public interface IInputManager
    {
        Transform GetCube();
    }
    
    public class InputManager : Singleton<InputManager>, IInputManager
    {
        private bool _isCanControl;
        
        private CubeManager _cubeManager; 
        private RaycastHit[] _hits = new RaycastHit[10];

        private Coroutine _coroutine;
        
        public Transform GetCube()
        {
            Transform ans = null;
            if (Camera.main == null)
            {
                Debug.LogWarning("No Main Camera");
                return null;
            }
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var size = Physics.RaycastNonAlloc(ray, _hits, Mathf.Infinity);
            
            for (var i = 0; i < size; ++ i)
            {
                var hit = _hits[i];
                if (hit.transform.TryGetComponent<CubeManager>(out var cubeManager) || hit.transform.TryGetComponent<CubeReverse>(out var reverse))
                {
                    ans = hit.transform;
                    break;
                }
            }
            return ans;
        }

        [Button]
        public void SetIsCanControl(bool isCanControl = true, float timeDelay = 0f)
        {
            if (timeDelay == 0f)
            {
                _isCanControl = isCanControl;
            }
            else
            {
                _coroutine ??= StartCoroutine(IEDelay(timeDelay, () =>
                {
                    _isCanControl = isCanControl;
                    _coroutine = null;
                }));
            }
        }

        private void Update()
        {
            if (_isCanControl)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    var cube = GetCube();
                    if (cube == null)
                    {
                        return;
                    }
                    _cubeManager = cube.GetComponent<CubeManager>();
                    if (_cubeManager == null)
                    {
                        var cubeReverse = cube.GetComponent<CubeReverse>();
                        cubeReverse.StartReverse();
                    }
                    else
                    {
                        _cubeManager.OnClickDown();
                    }
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    _cubeManager?.OnClickUp();
                    _cubeManager?.CheckMove();
                    _cubeManager = null;
                }
            }
        }

        private IEnumerator IEDelay(float time, Action callback)
        {
            yield return WaitForSecondCache.Get(time);

            callback?.Invoke();
        }
    }
}