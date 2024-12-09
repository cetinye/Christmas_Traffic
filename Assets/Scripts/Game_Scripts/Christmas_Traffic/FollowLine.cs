using UnityEngine;
using DG.Tweening;

namespace Christmas_Traffic
{
    public class FollowLine : MonoBehaviour
    {
        public float moveSpeed;
        public float idleSpeed;
        public float speed;
        public SantaType santaType;
        public bool startMovement;
        public int moveIndex = 0;
        public PathCreator pathCreator;
        public bool idle = false;

        [SerializeField] private float turnSpeed;

        private void Start()
        {
            pathCreator = GetComponent<PathCreator>();

            if (santaType == SantaType.Balloon)
                BalloonUp();
        }

        void Update()
        {
            if (Input.touchCount > 0)
            {
                pathCreator.DrawLine();
            }

            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && pathCreator.drawable)
            {
                startMovement = true;
            }

            if (startMovement)
            {
                if (pathCreator.points != null && pathCreator.points.Count != 0)
                {
                    Vector2 currentPos = pathCreator.points[moveIndex];
                    ChangeSpeed(moveSpeed);
                    transform.position = Vector2.MoveTowards(transform.position, currentPos, speed * Time.deltaTime);

                    // transform.DOLookAt(pathCreator.points[moveIndex], turnSpeed, AxisConstraint.Z, transform.up);

                    LookAt();

                    float distance = Vector3.Distance(currentPos, transform.position);

                    if (distance <= 0.05f)
                    {
                        pathCreator.points.RemoveAt(0);
                        pathCreator.lineRenderer.SetPositions(pathCreator.points.ToArray());
                        //moveIndex++;
                        transform.DOKill();
                    }

                    if (moveIndex > pathCreator.points.Count - 1)
                    {
                        pathCreator.points.Clear();
                        pathCreator.lineRenderer.positionCount = 0;
                        moveIndex = 0;
                        startMovement = false;
                        idle = true;
                    }
                }
            }

            if (pathCreator.points.Count == 0)
                idle = true;
            else
                idle = false;

            if (idle)
                IdleMove();
        }

        private void LookAt()
        {
            // Vector2 direction = pathCreator.points[moveIndex] - transform.position;
            // transform.rotation = Quaternion.FromToRotation(Vector3.up, direction);

            transform.right = pathCreator.points[moveIndex] - transform.position;
        }

        public void IdleMove()
        {
            ChangeSpeed(idleSpeed);
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

        public void ChangeSpeed(float value)
        {
            speed = Mathf.Lerp(speed, value, Time.deltaTime * 3f);
        }

        void BalloonUp()
        {
            transform.GetChild(0).DOLocalMoveY(transform.GetChild(0).localPosition.y + 1f, 2f).SetEase(Ease.InOutQuad).OnComplete(() => BalloonDown());
        }

        void BalloonDown()
        {
            transform.GetChild(0).DOLocalMoveY(transform.GetChild(0).localPosition.y - 1f, 2f).SetEase(Ease.InOutQuad).OnComplete(() => BalloonUp());
        }

        public enum SantaType
        {
            Reindeer,
            Rocket,
            Balloon
        }
    }
}