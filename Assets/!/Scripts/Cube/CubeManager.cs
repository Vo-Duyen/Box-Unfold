using System;
using System.Collections;
using System.Collections.Generic;
using DesignPattern.ObjectPool;
using DG.Tweening;
using LongNC.Manager;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LongNC.Cube
{
    public class CubeManager : MonoBehaviour
    {
        [SerializeField] private float _timeMove = 0.5f;
        [SerializeField] private float _distance = 1f;

        [SerializeField] private Collider _collider;

        private IMovement _movement = new Movement();
        private Vector3 _posMouseDown;
        private Vector3 _posMouseUp;
        private float _timeMouseDown;
        private float _timeMouseUp;
        private Coroutine _coroutine;
        private Dictionary<Transform, bool> _dictionary = new Dictionary<Transform, bool>();
        
        private void Awake()
        {
            for (var i = 0; i < transform.childCount; ++i)
            {
                _dictionary[transform.GetChild(i)] = true;
            }
        }

        private void Start()
        {
            transform.DOMove(transform.position + Vector3.back * 2f, 0.5f).From().SetEase(Ease.OutBounce);
        }

        public void OnClickDown()
        {
            _posMouseDown = Input.mousePosition;
            _timeMouseDown = Time.time;
        }

        public void OnClickUp()
        {
            _posMouseUp = Input.mousePosition;
            _timeMouseUp = Time.time;
        }
        
        public void CheckMove()
        {
            if (_coroutine == null)
            {
                var isDrag = _timeMouseUp - _timeMouseDown > 0.08f;
                
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
                    if (transform == null)
                    {
                        Debug.Log("Error");
                        return;
                    }
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
                            break;
                        }
                    }

                    // if (!isCheckCubeChild) Debug.LogWarning("Check cube child failed");
                    if (!isCheckCubeChild) return;

                    if (!LevelManager.Instance.CheckMove(transform, direction))
                    {
                        // Debug.LogWarning("Check Move failed");
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
                    
                    _coroutine = StartCoroutine(IEDelay(_timeMove, () =>
                    {
                        _movement.Move(transform, direction, _distance, _timeMove);
                        if (transform.childCount == 1)
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
                        // Debug.LogWarning("Get direction failed");
                        return;
                    }
                    
                    if (!LevelManager.Instance.CheckMove(transform, arrDirection))
                    {
                        // Debug.LogWarning("Check Move failed");
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
                    
                    _coroutine = StartCoroutine(IEDelay(_timeMove, () =>
                    {
                        _collider.enabled = false;
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

                            if (direct == Vector3.back)
                            {
                                continue;
                            }
                            
                            transform.GetChild(i).gameObject.SetActive(false);
                            
                            var childRes = direct switch
                            {
                                var d when d == Vector3.left => Direction.Left,
                                var d when d == Vector3.right => Direction.Right,
                                var d when d == Vector3.up => Direction.Up,
                                var d when d == Vector3.down => Direction.Down,
                                _ => Direction.None
                            };

                            var newObj = new GameObject(name: "Cube Clone")
                            {
                                transform =
                                {
                                    parent = transform.parent,
                                    position = transform.position,
                                    rotation = transform.rotation,
                                    localScale = transform.localScale,
                                }
                            };
                            for (var j = 0; j < transform.childCount; ++j)
                            {
                                var transChild = transform.GetChild(j);
                                var child = PoolingManager.Spawn(transChild.gameObject, transChild.position, transChild.rotation, newObj.transform); 
                                child.SetActive(i == j);
                                child.transform.localScale = Vector3.one;
                            }
                            
                            _movement.Move(newObj.transform, childRes, _distance, _timeMove);
                        }
                    }));
                }
            }
        }
        
        private IEnumerator IEDelay(float time, Action action)
        {
            action?.Invoke();
            
            yield return WaitForSecondCache.Get(time);

            _coroutine = null;
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