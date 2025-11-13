using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using DesignPattern;
using DesignPattern.ObjectPool;
using DesignPattern.Observer;
using DG.Tweening;
using LongNC.Cube;
using LongNC.Data;
using LongNC.UI.Data;
using LongNC.UI.Manager;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace LongNC.Manager
{
    public interface ILevelLoader
    {
        LevelData Load(int level);
    }
    
    public class LevelLoader : ILevelLoader
    {
        public LevelData Load(int level)
        {
            return Resources.Load<LevelData>("Data/LevelData" + level);
        }
    }
    
    public class LevelManager : Singleton<LevelManager>
    {
        private const string CurLevelString = "Current Level";
        
        private Transform TF => transform;
        
        [FoldoutGroup(CurLevelString)]
        [SerializeField]
        private int _currentLevel = -1;
        
        [FoldoutGroup(CurLevelString)]
        [SerializeField]
        private LevelData _curLevelData = null;
        
        [FoldoutGroup(CurLevelString)]
        [SerializeField]
        private GameObject _cubePrefab;
        
        [FoldoutGroup(CurLevelString)]
        [SerializeField]
        private GameObject _wallPrefab;
        
        [FoldoutGroup(CurLevelString)]
        [SerializeField]
        private GameObject _backgroundPrefab;
        
        [FoldoutGroup(CurLevelString)]
        [SerializeField]
        private GameObject _maskBackgroundPrefab;

        private bool _isShowTip;
        
        private readonly ILevelLoader _levelLoader = new LevelLoader();
        private CellType[,] _gridClone;
        private Vector3[] _arrCheckDirection =
        {
            new Vector3(0, -1),
            new Vector3(0, 1),
            new Vector3(-1, 0),
            new Vector3(1, 0),
        };

        private float _timeMove = 0.2f;

        private Dictionary<Transform, Stack<List<(int, int)>>> _dataReverse = new Dictionary<Transform, Stack<List<(int, int)>>>();

        private Dictionary<Transform, (int x, int y)> _dictIndexCube = new Dictionary<Transform, (int x, int y)>();
        private int _cntCheckWinLevel;
        
        [Button]
        public void LoadLevel(int level)
        {
            _currentLevel = level;
            _curLevelData = _levelLoader.Load(_currentLevel);
            // Debug.Log($"[Complete] Loaded level: {_curLevelData.name}");
        }

        [Button]
        public void LoadNextLevel()
        {
            ++_currentLevel;
            _curLevelData = _levelLoader.Load(_currentLevel);
            // Debug.Log($"[Complete] Loaded next level: {_currentLevel}");
        }

        [Button]
        public void ClearCurrentLevel()
        {
            var queue = new Queue<Transform>();
            queue.Enqueue(TF.GetChild(0));
            while (queue.Count > 0)
            {
                var target = queue.Dequeue();
                
                for (var i = 0; i < target.childCount; ++i)
                {
                    queue.Enqueue(target.GetChild(0));
                }
                
                DOTween.Kill(target);
                PoolingManager.Despawn(target.gameObject);
            }
        }

        [Button]
        public void LoadAllObjInLevel()
        {
            _dictIndexCube.Clear();
            var curLevelObj = new GameObject(name: $"{_curLevelData.name}")
            {
                transform =
                {
                    parent = TF
                }
            };

            var cubeObj = new GameObject(name: "Cubes")
            {
                transform =
                {
                    parent = curLevelObj.transform,
                }
            };

            var wallObj = new GameObject(name: "Walls")
            {
                transform =
                {
                    parent = curLevelObj.transform
                }
            };

            var maskBgObj = new GameObject(name: "MaskBg")
            {
                transform =
                {
                    parent = curLevelObj.transform,
                }
            };

            _cntCheckWinLevel = _curLevelData.cntCheckWinLevel;
            _isShowTip = false;
            switch (_curLevelData.isShowText)
            {
                case -1:
                    UIManager.Instance.ShowTip(false, _curLevelData.textShow);
                    break;
                case 0:
                    UIManager.Instance.ShowTip(true, _curLevelData.textShow);
                    break;
                case 1:
                    UIManager.Instance.ShowTip(false, _curLevelData.textShow);
                    _isShowTip = true;
                    break;
            }
            
            var grid = _curLevelData.gridCells;

            var pivot = Vector3.zero;
            pivot.x += 0.5f;
            pivot.y -= 1f;
            
            _gridClone = (CellType[, ]) grid.Clone();

            for (var i = 0; i < grid.GetLength(0); ++i)
            {
                for (var j = 0; j < grid.GetLength(1); ++j)
                {
                    var posCell = new Vector3(j - grid.GetLength(1) / 2f, grid.GetLength(0) / 2f - i);
                    posCell += pivot;
                    switch (grid[i, j])
                    {
                        case CellType.Ban:
                            break;
                        case CellType.Active:
                            break;
                        case CellType.Inactive:
                            posCell.z = -0.165f;
                            PoolingManager.Spawn(_wallPrefab, posCell, Quaternion.Euler(-90f, 0f, 0f),
                                wallObj.transform);

                            posCell.z = 0.29f;
                            PoolingManager.Spawn(_maskBackgroundPrefab, posCell, Quaternion.identity,
                                maskBgObj.transform);
                            break;
                        case CellType.Cube:
                            posCell.z = -0.165f;
                            PoolingManager.Spawn(_wallPrefab, posCell, Quaternion.Euler(-90f, 0f, 0f),
                                wallObj.transform);

                            posCell.z = 0.29f;
                            PoolingManager.Spawn(_maskBackgroundPrefab, posCell, Quaternion.identity,
                                maskBgObj.transform);
                            
                            posCell.z = -0.7f;
                            var trans = PoolingManager.Spawn(_cubePrefab, posCell, Quaternion.identity, cubeObj.transform);
                            _dictIndexCube[trans.transform] = (i, j);
                            _dataReverse[trans.transform] = new Stack<List<(int, int)>>();
                            _dataReverse[trans.transform].Push(new List<(int, int)>()
                            {
                                (i, j)
                            });
                            break;
                    }
                }
            }

            var bg = PoolingManager.Spawn(_backgroundPrefab, Vector3.back * 0.2f, Quaternion.Euler(Vector3.left * 90f), curLevelObj.transform);
            bg.transform.localScale *= 1.75f;

            
            // UI
            UIManager.Instance.UpdateLevel(_currentLevel);
            UIManager.Instance.OnRestartTimer(_curLevelData.timeCounter);
        }

        public bool CheckMove(Transform trans, Direction direction)
        {
            // Debug.Log("Check move 1");
            var id = direction switch
            {
                Direction.Left => 0,
                Direction.Right => 1,
                Direction.Up => 2,
                Direction.Down => 3,
                _ => -1
            };

            if (_dictIndexCube.ContainsKey(trans) == false)
            {
                // Debug.LogWarning("This is null");
                return false;
            }
            var newIndex = (_dictIndexCube[trans].x + (int) _arrCheckDirection[id].x, _dictIndexCube[trans].y + (int) _arrCheckDirection[id].y);

            // Debug.LogWarning($"{newIndex} + {_gridClone[newIndex.Item1, newIndex.Item2]}");
            if (_gridClone[newIndex.Item1, newIndex.Item2] == CellType.Inactive)
            {
                _gridClone[_dictIndexCube[trans].x, _dictIndexCube[trans].y] = CellType.Active;
                _gridClone[newIndex.Item1, newIndex.Item2] = CellType.Cube;
                _dictIndexCube[trans] = newIndex;

                CheckLevel(1);
                
                _dataReverse[trans].Push(new List<(int, int)>()
                {
                    {
                        (newIndex.Item1, newIndex.Item2)
                    }
                });

                if (_isShowTip && _cntCheckWinLevel == 2)
                {
                    _isShowTip = false;
                    UIManager.Instance.ShowTip(true, _curLevelData.textShow);
                }
                
                return true;
            }
            
            return false;
        }

        public bool CheckMove(Transform target, Direction[] directions)
        {
            var arrReverse = new List<(int, int)>();
            // Debug.Log($"Check Move 2 + {directions.Length}");
            foreach (var direction in directions)
            {
                var id = direction switch
                {
                    Direction.Left => 0,
                    Direction.Right => 1,
                    Direction.Up => 2,
                    Direction.Down => 3,
                    _ => -1
                };
                
                var newIndex = (_dictIndexCube[target].x + (int) _arrCheckDirection[id].x, _dictIndexCube[target].y + (int) _arrCheckDirection[id].y);
                
                if (_gridClone[newIndex.Item1, newIndex.Item2] != CellType.Inactive)
                {
                    return false;
                }
            }
            
            foreach (var direction in directions)
            {
                var id = direction switch
                {
                    Direction.Left => 0,
                    Direction.Right => 1,
                    Direction.Up => 2,
                    Direction.Down => 3,
                    _ => -1
                };
                
                var newIndex = (_dictIndexCube[target].x + (int) _arrCheckDirection[id].x, _dictIndexCube[target].y + (int) _arrCheckDirection[id].y);
                
                _gridClone[_dictIndexCube[target].x, _dictIndexCube[target].y] = CellType.Active;
                _gridClone[newIndex.Item1, newIndex.Item2] = CellType.Active;
                // _dictIndexCube[target] = newIndex;
                
                arrReverse.Add((newIndex.Item1, newIndex.Item2));

            }
            CheckLevel(directions.Length);
            
            _dataReverse[target].Push(arrReverse);
            
            return true;
        }

        public void CheckLevel(int amount = 0)
        {
            _cntCheckWinLevel -= amount;
            if (_cntCheckWinLevel == 0)
            {
                ObserverManager<UIEventID>.Instance.PostEvent(UIEventID.OnWinGame, 0.5f);
                SoundManager.Instance.PlayFX(SoundId.Win);
            }
            else if (_cntCheckWinLevel < 0)
            {
                Debug.LogWarning("Error");
            }
        }

        public void ReverseCube(Transform cubeTrans)
        {
            var sequence = DOTween.Sequence();
            var totalTime = 0f;
            while (_dataReverse[cubeTrans].Count > 1)
            {
                var data = _dataReverse[cubeTrans].Pop();
                var timeMove = _timeMove;
                totalTime += timeMove;
                sequence.AppendInterval(totalTime);
                foreach (var (i, j) in data)
                {
                    sequence.JoinCallback(() =>
                    {
                        _gridClone[i, j] = CellType.Inactive;
                        CheckLevel(-1);
                    });
                }
            }
            
            sequence?.OnComplete(() =>
            {
                _dictIndexCube[cubeTrans] = _dataReverse[cubeTrans].Peek()[0];
                _gridClone[_dictIndexCube[cubeTrans].x, _dictIndexCube[cubeTrans].y] = CellType.Cube;
            });

            sequence?.Play();
        }
        
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (_gridClone == null) return;
            var pivot = Vector3.zero;
            pivot.x += 0.5f + 10f;
            for (var i = 0; i < _gridClone.GetLength(0); ++i)
            {
                for (var j = 0; j < _gridClone.GetLength(1); ++j)
                {
                    var posCell = new Vector3(j - _gridClone.GetLength(1) / 2f, _gridClone.GetLength(0) / 2f - i);
                    posCell += pivot;
                    switch (_gridClone[i, j])
                    {
                        case CellType.Ban:
                            break;
                        case CellType.Active:
                            Gizmos.color = Color.red;
                            Gizmos.DrawCube(posCell, Vector3.one);
                            break;
                        case CellType.Inactive:
                            posCell.z = -0.165f;
                            Gizmos.color = Color.green;
                            Gizmos.DrawCube(posCell, Vector3.one);
                            break;
                        case CellType.Cube:
                            posCell.z = -0.165f;
                            Gizmos.color = Color.yellow;
                            Gizmos.DrawCube(posCell, Vector3.one);
                            break;
                    }
                }
            }
        }
#endif
    }
}