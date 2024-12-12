using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Christmas_Traffic
{
    public class UIManager : MonoBehaviour
    {
        private LevelManager levelManager;

        [Header("Images")]
        [SerializeField] private Image timeSlider;

        [Header("Texts")]
        [SerializeField] private TMP_Text statsText;

        [Header("Santa Amount Info Panel")]
        [SerializeField] private InfoPanel santaAmountInfoPanel;

        [Header("Santas Info Panel")]
        [SerializeField] private Transform parentTransform;

        public void Initialize()
        {
            levelManager = LevelManager.Instance;
        }

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

        public void ShowInfoPanels()
        {
            santaAmountInfoPanel.Show();

            if (levelManager.LevelSO.InfoPanel != null)
            {
                GameObject panel = Instantiate(levelManager.LevelSO.InfoPanel, parentTransform);
                panel.SetActive(true);
            }
        }
    }
}