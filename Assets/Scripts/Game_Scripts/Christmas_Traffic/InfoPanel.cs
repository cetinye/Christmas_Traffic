using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Christmas_Traffic
{
    public class InfoPanel : MonoBehaviour
    {
        private LevelManager levelManager;

        [SerializeField] private Image bg;
        [SerializeField] private Image button;
        [SerializeField] private TMP_Text infoText;
        [SerializeField] private float fadeInOutTime;

        private bool pressed;

        void OnDestroy()
        {
            StopAllCoroutines();
        }

        public void StartGame()
        {
            if (!pressed)
            {
                pressed = true;
                StartCoroutine(AnimOutRoutine());
            }
        }

        private void UpdateSantaAmountPanel()
        {
            string newText = "<color=#FFFF>" + "Holiday rush!\nDeliver " + "<color=#38AA2C>" + levelManager.LevelSO.NumOfSantasRequiredToLand + " gifts " + "<color=#FFFF>" + "in under\n" + levelManager.LevelSO.TotalTime + " seconds with\nup to " + "<color=#D53232>" + (levelManager.LevelSO.TotalSantaAmount - levelManager.LevelSO.NumOfSantasRequiredToLand) + " mistakes" + "<color=#FFFF>" + " allowed.";
            infoText.text = newText;
        }

        public void Show()
        {
            levelManager = LevelManager.Instance;
            UpdateSantaAmountPanel();
            StartCoroutine(AnimInRoutine());
        }

        private IEnumerator AnimInRoutine()
        {
            Sequence s = DOTween.Sequence();

            s.Append(bg.DOFade(1f, fadeInOutTime));
            s.Join(infoText.DOFade(1f, fadeInOutTime));
            s.AppendInterval(1.5f);
            s.Append(button.DOFade(1f, fadeInOutTime));

            yield return s.WaitForCompletion();
        }

        private IEnumerator AnimOutRoutine()
        {
            Sequence s = DOTween.Sequence();

            s.Append(bg.DOFade(0f, fadeInOutTime));
            s.Join(infoText.DOFade(0f, fadeInOutTime));
            s.Join(button.DOFade(0f, fadeInOutTime));

            yield return s.WaitForCompletion();

            levelManager.StartGame();
            transform.gameObject.SetActive(false);
        }
    }
}