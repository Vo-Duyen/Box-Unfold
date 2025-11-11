using System;
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
        [ShowInInspector]
        [ReadOnly]
        private bool _isCanControl;
        
        private CubeManager _cubeManager; 
        private RaycastHit[] _hits = new RaycastHit[10];
        
        public Transform GetCube()
        {
            var ans = transform;
            ans = null;
            if (Camera.main == null)
            {
                Debug.LogWarning("No Main Camera");
                return null;
            }
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var size = Physics.RaycastNonAlloc(ray, _hits, Mathf.Infinity);
            
            for (var i = 0; i < Mathf.Min(size, _hits.Length); ++ i)
            {
                var hit = _hits[i];
                if (hit.transform.TryGetComponent<CubeManager>(out var cubeManager))
                {
                    ans = hit.transform;
                    break;
                }
            }

            _hits = new RaycastHit[10];
            
            return ans;
        }

        [Button]
        public void SetIsCanControl(bool isCanControl = true)
        {
            _isCanControl = isCanControl;
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
                    _cubeManager.OnClickDown();
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    if (_cubeManager != null)
                    {
                        _cubeManager.OnClickUp();
                        _cubeManager.CheckMove();
                    }
                }
            }
        }
    }
}