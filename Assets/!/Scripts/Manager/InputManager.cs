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
        
        private bool _isDragging;
        private CubeManager _cubeManager;
        
        public Transform GetCube()
        {
            if (Camera.main == null)
            {
                Debug.LogWarning("No Main Camera");
                return null;
            }
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var hits = new RaycastHit[10];
            var size = Physics.RaycastNonAlloc(ray, hits, Mathf.Infinity);
            
            for (var i = 0; i < Mathf.Min(size, hits.Length); ++ i)
            {
                var hit = hits[i];
                if (hit.transform.TryGetComponent<CubeManager>(out var cubeManager))
                {
                    return hit.transform;
                }
            }
            return null;
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
                if (Input.GetMouseButtonDown(0) && !_isDragging)
                {
                    var cube = GetCube();
                    if (cube == null)
                    {
                        return;
                    }

                    _isDragging = true;
                    
                    _cubeManager = cube.GetComponent<CubeManager>();
                    _cubeManager.OnClickDown();
                }
                else if (Input.GetMouseButtonUp(0) && _isDragging)
                {
                    _isDragging = false;
                    _cubeManager.OnClickUp();
                    _cubeManager.CheckMove();
                }
            }
        }
    }
}