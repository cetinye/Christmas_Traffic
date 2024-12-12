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
        [SerializeField] private bool isOthersLandable;

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
                if (santa.SantaType != Santa.SantaTypes.Balloon && !isOthersLandable) return;

                if (santa.SantaType == Santa.SantaTypes.Balloon || santa.GetSantaColor() == spriteRenderer.color)
                {
                    santa.SantaState = Santa.SantaStates.Landing;
                    santa.SetCollideable(false);
                    santa.SetRenderOrder(9);
                    levelManager.IncrementCorrect();
                    santa.ClearPoints();
                    StartCoroutine(LandRoutine(santa));
                }
            }
        }

        IEnumerator LandRoutine(Santa santa)
        {
            Sequence sequence = DOTween.Sequence();

            santa.transform.right = transform.position - santa.transform.position;

            sequence.Append(santa.transform.DOMove(transform.position, 0.5f).SetEase(Ease.Linear).OnComplete(() => santa.transform.right = moveToOnLand.transform.position - santa.transform.position));
            sequence.Append(santa.transform.DOScale(landingScale, landingScaleTime).SetEase(Ease.Linear));
            sequence.Join(santa.transform.DOMove(moveToOnLand.transform.position, timeToMoveToTarget).SetEase(Ease.Linear));

            yield return sequence.WaitForCompletion();

            santa.Die();
        }
    }
}