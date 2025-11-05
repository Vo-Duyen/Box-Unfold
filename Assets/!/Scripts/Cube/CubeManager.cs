using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LongNC.Cube
{
    public class CubeManager : MonoBehaviour
    {
        [SerializeField] private float _timeMove = 0.5f;
        [SerializeField] private float _distance = 1f;

        private IMovement _movement = new Movement();
        private Vector3 _posMouseDown;
        private Vector3 _posMouseUp;
        private Coroutine _coroutine;
        private Dictionary<Transform, bool> _dictionary = new Dictionary<Transform, bool>();

        private Vector3 _rotate;
        
        private void Awake()
        {
            for (var i = 0; i < transform.childCount; ++i)
            {
                _dictionary[transform.GetChild(i)] = true;
            }
        }

        public void OnClickDown()
        {
            _posMouseDown = Input.mousePosition;
        }

        public void OnClickUp()
        {
            _posMouseUp = Input.mousePosition;
        }
        
        public void CheckMove()
        {
            // TODO: Fix late
            
            // z min la day va noi khong di chuyen

            if (_coroutine == null)
            {
                _coroutine = StartCoroutine(IEDelay(_timeMove, () =>
                {
                    var direction = _movement.GetDirection(_posMouseDown, _posMouseUp);
                    _movement.Move(transform, ref _rotate, direction, _distance, _timeMove);
                }));
            }
        }

        private IEnumerator IEDelay(float time, Action action)
        {
            action?.Invoke();
            
            yield return WaitForSecondCache.Get(time);

            _coroutine = null;
        }
    }
}