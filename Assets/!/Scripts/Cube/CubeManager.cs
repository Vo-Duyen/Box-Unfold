using System;
using System.Collections;
using System.Collections.Generic;
using DesignPattern.ObjectPool;
using DG.Tweening;
using LongNC.Manager;
using NUnit.Framework;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LongNC.Cube
{
    public class CubeManager : MonoBehaviour
    {
        [SerializeField] private float _timeMove = 0.5f;
        [SerializeField] private float _distance = 1f;

        [SerializeField] private Collider _collider;
        [SerializeField] private CubeReverse _cubeReverse;

        private IMovement _movement = new Movement();
        private Vector3 _posMouseDown;
        private Vector3 _posMouseUp;
        
        private Dictionary<Transform, bool> _dictionary = new Dictionary<Transform, bool>();
        
        private Coroutine _coroutine;
        
        private void Awake()
        {
            for (var i = 0; i < transform.childCount; ++i)
            {
                _dictionary[transform.GetChild(i)] = true;
            }
        }

        private void Start()
        {
            transform.DOMove(transform.position + Vector3.back * 2f, 0.5f).From().SetEase(Ease.OutBounce).OnComplete(() =>
            {
                _collider.enabled = true;
            });
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
            if (_coroutine == null)
            {
                _cubeReverse?.ReadyReverse(_timeMove);
                var isDrag = Vector3.Distance(_posMouseDown, _posMouseUp) > 0.1f;
                if (isDrag)
                {
                    var direction = _movement.GetDirection(_posMouseDown, _posMouseUp);
                    var realDirection = direction switch
                    {
                        Direction.Left => Vector3.left,
                        Direction.Right => Vector3.right,
                        Direction.Up => Vector3.up,
                        Direction.Down => Vector3.down,
                        _ => Vector3.zero
                    };
                    var isCheckCubeChild = false;
                    var directs = new List<Vector3>();
                    for (var i = 0; i < transform.childCount; ++i)
                    {
                        var direct = transform.GetChild(i).position - transform.position;
                        direct = direct.normalized;
                        if (Mathf.Abs(direct.x) > 0.7f) direct.x = direct.x > 0 ? 1 : -1;
                        else direct.x = 0;
                        if (Mathf.Abs(direct.y) > 0.7f) direct.y = direct.y > 0 ? 1 : -1;
                        else direct.y = 0;
                        if (Mathf.Abs(direct.z) > 0.7f) direct.z = direct.z > 0 ? 1 : -1;
                        else direct.z = 0;
                        if (direct == realDirection)
                        {
                            isCheckCubeChild = true;
                        }
                        directs.Add(direct);
                    }
                    if (!isCheckCubeChild) return;
                    if (transform.childCount == 3)
                    {
                        foreach (var vector3 in directs)
                        {
                            if (vector3 == realDirection * -1)
                            {
                                return;
                            }
                        }
                    }
                    if (!LevelManager.Instance.CheckMove(transform, direction))
                    {
                        return;
                    }
                    var bestSquare = transform.GetChild(0);
                    for (var i = 1; i < transform.childCount; ++i)
                    {
                        var trans = transform.GetChild(i);
                        if (trans.position.z > bestSquare.position.z)
                        {
                            bestSquare = trans;
                        }
                    }
                    bestSquare.SetParent(transform.parent);
                    
                    _coroutine = StartCoroutine(IEDelayCall(_timeMove, () =>
                    {
                        _movement.MoveParent(transform, direction, _distance, _timeMove);
                        if (transform.childCount <= 1)
                        {
                            _collider.enabled = false;
                        }
                    }));
                }
                else
                {
                    var arrDirection = _movement.GetDirections(transform);
                    if (arrDirection == null)
                    {
                        return;
                    }
                    if (!LevelManager.Instance.CheckMove(transform, arrDirection))
                    {
                        return;
                    }
                    var bestSquare = transform.GetChild(0);
                    for (var i = 1; i < transform.childCount; ++i)
                    {
                        var trans = transform.GetChild(i);
                        if (trans.position.z > bestSquare.position.z)
                        {
                            bestSquare = trans;
                        }
                    }
                    bestSquare.SetParent(transform.parent);
                    _coroutine = StartCoroutine(IEDelayCall(_timeMove, () =>
                    {
                        _collider.enabled = false;
                        _movement.MoveAll(transform, _distance, _timeMove);
                    }));
                }
            }
        }
        private IEnumerator IEDelayCall(float time, Action callback)
        {
            callback?.Invoke();
            yield return WaitForSecondCache.Get(time);
            _coroutine = null;
        }
        
        public void Reverse()
        {
            if (_coroutine == null)
            {
                _movement.MoveReverseParent(transform, _cubeReverse.transform, _collider);
                LevelManager.Instance.ReverseCube(transform);
            }
        }
        
#if UNITY_EDITOR
        [Button]
        private void GetCollider()
        {
            if (transform.TryGetComponent<Collider>(out var col))
            {
                _collider = col;
            }
        }
#endif
    }
}