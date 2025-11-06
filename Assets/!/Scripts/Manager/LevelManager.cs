using DesignPattern;
using DesignPattern.ObjectPool;
using LongNC.Data;
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
        
        private readonly ILevelLoader _levelLoader = new LevelLoader();
        
        [Button]
        public void LoadLevel(int level)
        {
            _currentLevel = level;
            _curLevelData = _levelLoader.Load(_currentLevel);
            Debug.Log($"Loaded Level {_curLevelData}");
        }

        [Button]
        public void LoadNextLevel()
        {
            ++_currentLevel;
            _curLevelData = _levelLoader.Load(_currentLevel);
            Debug.Log($"Loaded next level {_currentLevel}");
        }

        [Button]
        public void LoadAllObjInLevel()
        {
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
                    parent = curLevelObj.transform
                }
            };

            var wallObj = new GameObject(name: "Walls")
            {
                transform =
                {
                    parent = curLevelObj.transform
                }
            };

            var backgroundObj = new GameObject(name: "Background")
            {
                transform =
                {
                    parent = curLevelObj.transform
                }
            };

            var grid = _curLevelData.gridCells;

            for (var i = 0; i < grid.GetLength(0); ++i)
            {
                for (var j = 0; j < grid.GetLength(1); ++j)
                {
                    var posCell = new Vector3((i - grid.GetLength(0) / 2f) * 1f, (grid.GetLength(1) / 2f - j) * 1f);
                    switch (grid[i, j])
                    {
                        case CellType.Ban:
                            PoolingManager.Spawn(_backgroundPrefab, posCell, Quaternion.Euler(-90f, 0f, 0f), backgroundObj.transform);
                            break;
                        case CellType.Active:
                            break;
                        case CellType.Inactive:
                            posCell.z = 0.1f;
                            PoolingManager.Spawn(_wallPrefab, posCell, Quaternion.Euler(-90f, 0f, 0f),
                                wallObj.transform);
                            break;
                        case CellType.Cube:
                            posCell.z = -0.5f;
                            PoolingManager.Spawn(_cubePrefab, posCell, Quaternion.identity, cubeObj.transform);
                            break;
                    }
                }
            }
        }
    }
}