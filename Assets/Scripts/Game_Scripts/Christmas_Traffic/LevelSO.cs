using UnityEngine;

namespace Christmas_Traffic
{
    [CreateAssetMenu(fileName = "LevelSO", menuName = "ScriptableObjects/LevelSO", order = 1)]
    public class LevelSO : ScriptableObject
    {
        public int LevelId;
        public int TotalSantaAmount;
        public int NumOfSantasRequiredToLand;
        public int MooseSantaAmount;
        public int RocketSantaAmount;
        public int BalloonSantaAmount;
        public int ActiveLaneCount;
        public int TotalTime;
        public int LevelUpCriteria;
        public int LevelDownCriteria;
        public int MaxInGame;
        public int MinScore;
        public int PenaltyPoints;
    }
}