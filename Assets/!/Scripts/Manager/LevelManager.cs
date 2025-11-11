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
        
        private readonly ILevelLoader _levelLoader = new LevelLoader();
        private CellType[,] _gridClone;
        private Vector3[] _arrCheckDirection =
        {
            new Vector3(0, -1),
            new Vector3(0, 1),
            new Vector3(-1, 0),
            new Vector3(1, 0),
        };

        private Dictionary<Transform, (int x, int y)> _dictIndexCube = new Dictionary<Transform, (int x, int y)>();
        private int _cntCheckWinLevel;
        
        [Button]
        public void LoadLevel(int level)
        {
            _currentLevel = level;
            _curLevelData = _levelLoader.Load(_currentLevel);
            Debug.Log($"[Complete] Loaded level: {_curLevelData.name}");
        }

        [Button]
        public void LoadNextLevel()
        {
            ++_currentLevel;
            _curLevelData = _levelLoader.Load(_currentLevel);
            Debug.Log($"[Complete] Loaded next level: {_currentLevel}");
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
            
            var grid = _curLevelData.gridCells;

            var pivot = Vector3.zero;
            pivot.x += 0.5f;
            
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
                            break;
                    }
                }
            }

            var isResize = grid.GetLength(0) > 7 || grid.GetLength(1) > 7;

            var bg = PoolingManager.Spawn(_backgroundPrefab, Vector3.back * 0.2f, Quaternion.Euler(Vector3.left * 90f), curLevelObj.transform);
            var cam = Camera.main.transform;
            
            if (isResize)
            {
                bg.transform.localScale *= 1.2f;
                var pos = cam.position;
                pos.z = - 11.6f;
                cam.position = pos;
            }
            else
            {
                var pos = cam.position;
                pos.z = - 10f;
                cam.position = pos;
            }
            
            // UI
            UIManager.Instance.UpdateLevel(_currentLevel);
            UIManager.Instance.OnRestartTimer(_curLevelData.timeCounter);
        }

        public bool CheckMove(Transform trans, Direction direction)
        {
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
                return false;
            }
            var newIndex = (_dictIndexCube[trans].x + (int) _arrCheckDirection[id].x, _dictIndexCube[trans].y + (int) _arrCheckDirection[id].y);

            if (_gridClone[newIndex.Item1, newIndex.Item2] == CellType.Inactive)
            {
                _gridClone[_dictIndexCube[trans].x, _dictIndexCube[trans].y] = CellType.Active;
                _gridClone[newIndex.Item1, newIndex.Item2] = CellType.Cube;
                _dictIndexCube[trans] = newIndex;

                CheckLevel(1);
                
                return true;
            }
            
            return false;
        }

        public bool CheckMove(Transform target, Direction[] directions)
        {
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
                _gridClone[newIndex.Item1, newIndex.Item2] = CellType.Cube;
                _dictIndexCube[target] = newIndex;

            }
            CheckLevel(directions.Length);
            
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
    }
}