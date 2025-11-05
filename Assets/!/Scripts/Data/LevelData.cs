using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace LongNC.Data
{
    [Serializable]
    public enum CellType
    {
        Ban,
        Inactive,
        Active,
        Cube,
    }
    
    [CreateAssetMenu(fileName = "LevelData0", menuName = "Data/New Level Data", order = 1)]
    public class LevelData : SerializedScriptableObject
    {
        private const string SetupString = "Setup";
        private const string GridString = SetupString + "/Grid Setup";
        
#if UNITY_EDITOR
        [BoxGroup(SetupString)]
        [TableMatrix]
#endif
        public CellType[, ] gridCells = new CellType[1, 1];
        
#if UNITY_EDITOR
        [FoldoutGroup(GridString)]
        [OdinSerialize]
        private int _rows = 6;
        
        [FoldoutGroup(GridString)]
        [OdinSerialize]
        private int _columns = 5;
        
        [FoldoutGroup(GridString)]
        [Button]
        private void SetupGrid()
        {
            gridCells = new CellType[_columns, _rows];
        }
#endif
    }
}