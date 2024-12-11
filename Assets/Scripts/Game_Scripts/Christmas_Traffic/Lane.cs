using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Christmas_Traffic
{
    public class Lane : MonoBehaviour
    {
        [SerializeField] private Vector3 landingScale;
        [SerializeField] private float landingScaleTime;
        [SerializeField] private GameObject moveToOnLand;
        [SerializeField] private float timeToMoveToTarget;
        [SerializeField] private bool isBalloonLandable;

        private SpriteRenderer spriteRenderer;
        private LevelManager levelManager;

        void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            levelManager = LevelManager.Instance;
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out Santa santa) && santa.IsLandable() && santa.SantaState != Santa.SantaStates.Landing)
            {
                if (santa.SantaType == Santa.SantaTypes.Balloon && !isBalloonLandable) return;

                santa.SantaState = Santa.SantaStates.Landing;

                if (santa.GetSantaColor() == spriteRenderer.color)
                    levelManager.IncrementCorrect();
                else
                    levelManager.IncrementWrong();

                santa.ClearPoints();

                StartCoroutine(LandRoutine(santa));
            }
        }

        IEnumerator LandRoutine(Santa santa)
        {
            Sequence sequence = DOTween.Sequence();

            santa.transform.right = moveToOnLand.transform.position - santa.transform.position;

            sequence.Append(santa.transform.DOScale(landingScale, landingScaleTime).SetEase(Ease.Linear));
            sequence.Join(santa.transform.DOMove(moveToOnLand.transform.position, timeToMoveToTarget).SetEase(Ease.Linear));

            yield return sequence.WaitForCompletion();

            santa.Die();
        }
    }
}