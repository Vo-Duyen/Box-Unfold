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
        private const string TimeString = SetupString + "/Time";
        
#if UNITY_EDITOR
        [BoxGroup(SetupString)]
        [TableMatrix(Transpose = true)]
#endif
        public CellType[, ] gridCells = new CellType[1, 1];
        
        [BoxGroup(SetupString)]
        public int cntCheckWinLevel;
        
#if UNITY_EDITOR
        [FoldoutGroup(GridString)]
        [OdinSerialize]
        private int _rows;
        
        [FoldoutGroup(GridString)]
        [OdinSerialize]
        private int _columns;
        
        [FoldoutGroup(TimeString)]
        [OdinSerialize]
        public float timeCounter = 90f;
        
        [FoldoutGroup(GridString)]
        [Button]
        private void SetupGrid()
        {
            gridCells = new CellType[_rows, _columns];
        }

        [FoldoutGroup(GridString)]
        [Button]
        private void GetCntCheckWinLevel()
        {
            cntCheckWinLevel = 0;
            for (var i = 0; i < gridCells.GetLength(0); ++i)
            {
                for (var j = 0; j < gridCells.GetLength(1); ++j)
                {
                    if (gridCells[i, j] == CellType.Inactive)
                    {
                        ++cntCheckWinLevel;
                    }
                }
            }
        }
#endif
    }
}