using TMPro;
using UnityEngine;

namespace Christmas_Traffic
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text statsText;

        public void UpdateStatsText(int correctCount, int wrongCount)
        {
            correctCount = Mathf.Max(0, correctCount);
            wrongCount = Mathf.Max(0, wrongCount);
            statsText.text = correctCount.ToString("00") + "-" + wrongCount.ToString("00");
        }
    }
}