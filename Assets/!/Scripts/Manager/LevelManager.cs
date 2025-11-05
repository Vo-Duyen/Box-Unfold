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
            var curLevelObj = new GameObject(name: $"Level{_curLevelData}")
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
                    parent = TF
                }
            };

            var grid = _curLevelData.gridCells;

            for (var i = 0; i < grid.GetLength(0); ++i)
            {
                for (var j = 0; j < grid.GetLength(1); ++j)
                {
                    switch (grid[i, j])
                    {
                        case CellType.Ban:
                            break;
                        case CellType.Active:
                            break;
                        case CellType.Inactive:
                            PoolingManager.Spawn(_wallPrefab, Vector3.back * 0.1f, Quaternion.identity,
                                wallObj.transform);
                            break;
                        case CellType.Cube:
                            PoolingManager.Spawn(_cubePrefab, Vector3.zero, Quaternion.identity, cubeObj.transform);
                            break;
                    }
                }
            }
        }
    }
}