using UnityEngine;

namespace Christmas_Traffic
{
    [CreateAssetMenu(fileName = "LevelSO", menuName = "ScriptableObjects/LevelSO", order = 1)]
    public class LevelSO : ScriptableObject
    {
        public int LevelId;
        public int ActiveLaneCount;
        public int SantaAmount;
    }
}