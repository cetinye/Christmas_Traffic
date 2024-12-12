using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Christmas_Traffic
{
    public class UIManager : MonoBehaviour
    {
        [Header("Images")]
        [SerializeField] private Image timeSlider;

        [Header("Texts")]
        [SerializeField] private TMP_Text statsText;

        public void UpdateStatsText(int correctCount, int wrongCount)
        {
            correctCount = Mathf.Max(0, correctCount);
            wrongCount = Mathf.Max(0, wrongCount);
            statsText.text = correctCount.ToString("00") + "-" + wrongCount.ToString("00");
        }

        public void UpdateTimeSlider(float timer, float totalTime)
        {
            timeSlider.fillAmount = timer / totalTime;
        }
    }
}