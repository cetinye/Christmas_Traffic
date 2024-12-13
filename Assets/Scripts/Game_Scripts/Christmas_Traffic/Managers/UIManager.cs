using DG.Tweening;
using HUDIndicator;
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

        [Header("Countdown Variables")]
        [SerializeField] private Image countdownBg;
        [SerializeField] private RectTransform countdownRect;
        [SerializeField] private TMP_Text countdownText;
        private float flashInterval = 0.5f;
        public bool IsFlashable = true;

        [Header("Times Up Variables")]
        [SerializeField] private Image timesUpBg;
        [SerializeField] private TMP_Text timesUpText;

        [Header("HUD Indicator Variables")]
        [SerializeField] private HUDIndicator.IndicatorRenderer indicatorRenderer;

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

        public void Last5SecWaring()
        {
            countdownBg.DOFade(1f, 0.5f);
            countdownRect.DOAnchorPosY(-128f, 1.2f).OnComplete(() =>
            {
                // GameManager.instance.PlayFx("Countdown", 0.7f, 1f);
                FlashRed();
            });
        }

        public void SetCountdownText(float val)
        {
            countdownText.text = val.ToString("F0");
        }

        private void FlashRed()
        {
            Sequence redFlash = DOTween.Sequence();

            redFlash.Append(countdownText.DOColor(new Color(1f, 0.538739f, 0f, 1f), flashInterval))
                    .SetEase(Ease.Linear)
                    .Append(countdownText.DOColor(Color.white, flashInterval))
                    .SetEase(Ease.Linear)
                    .SetLoops(6);

            redFlash.Play();
        }

        public void TimesUp()
        {
            timesUpBg.DOFade(1f, 2f);
            timesUpText.DOFade(1f, 2f);
        }

        public HUDIndicator.IndicatorRenderer GetIndicatorRenderer()
        {
            return indicatorRenderer;
        }
    }
}