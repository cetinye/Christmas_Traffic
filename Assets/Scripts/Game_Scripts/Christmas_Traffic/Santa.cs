using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Lean.Touch;
using UnityEngine;

namespace Christmas_Traffic
{
    public class Santa : MonoBehaviour
    {
        [Header("Santa State")]
        [SerializeField] private SantaStates santaState = SantaStates.Idle;
        public SantaStates SantaState
        {
            set
            {
                santaState = value;
                Debug.Log("Santa State: " + santaState);
            }
            get { return santaState; }
        }
        public enum SantaStates
        {
            Idle,
            FollowingPath,
            Landing,
            Dead
        }

        [Header("Speed Variables")]
        [SerializeField] private float moveSpeed;
        [SerializeField] private float idleSpeed;
        [SerializeField] private float speed;
        [SerializeField] private float rotationSpeed;

        [Header("Collision Detection Variables")]
        [SerializeField] private List<Collider2D> collidersNearbyList = new List<Collider2D>();
        [SerializeField] private float collisionDetectRadius;
        [SerializeField] private SpriteRenderer warningRenderer;
        [SerializeField] private Vector3 endScale = new Vector3(0.6f, 0.6f, 0.6f);
        private Sequence startWarningSeq;
        private Sequence endWarningSeq;

        [Header("Santa Type")]
        public SantaTypes SantaType;
        public enum SantaTypes
        {
            Moose,
            Balloon,
            Rocket
        }

        [Header("Components")]
        [SerializeField] private SpriteRenderer colorRenderer;
        private Collider2D santaCollider;
        private LineRenderer lineRenderer;
        private SpriteRenderer santaRenderer;
        private LevelManager levelManager;

        [Header("Particles")]
        [SerializeField] private ParticleSystem confettiBlast;

        public List<Vector3> points = new List<Vector3>();
        private bool isLandable;
        private bool isInteracted;
        private int moveIndex = 0;
        public bool pathDrawable = false;
        private bool isSoundPlayed;

        void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
            santaCollider = GetComponent<Collider2D>();
            santaRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        }

        public void Initialize()
        {
            levelManager = LevelManager.Instance;

            levelManager.AddActiveSanta(this);

            ColorSanta();
        }

        void Update()
        {
            if (levelManager.State != LevelManager.GameState.Playing) return;

            CreateFollowPath();
            CheckNearbyCollision();

            AlignWarning();
        }

        void OnCollisionEnter2D(Collision2D other)
        {
            if (other.collider.TryGetComponent(out Santa santa) &&
            santa.SantaState != SantaStates.Landing && santa.SantaState != SantaStates.Dead &&
             SantaState != SantaStates.Landing && SantaState != SantaStates.Dead)
            {
                CollidedWSanta();
                santa.CollidedWSanta();
            }

            if (other.collider.CompareTag("OOB"))
            {
                levelManager.IncrementWrong();
                Die();
            }
        }

        public void CollidedWSanta()
        {
            if (SantaState == SantaStates.Dead) return;

            SantaState = SantaStates.Dead;
            AudioManager.Instance.PlayOneShot(AudioManager.SoundType.Collision);
            levelManager.IncrementWrong();
            transform.DOScale(0.5f, 1.5f).OnComplete(() => Die());
        }

        private void CreateFollowPath()
        {
            if (SantaState == SantaStates.FollowingPath)
            {
                if (pathDrawable && LeanTouch.Fingers.Count > 0 && LeanTouch.Fingers[0].SwipeScaledDelta.magnitude > 0)
                {
                    Ray ray = levelManager.MainCamera.ScreenPointToRay(LeanTouch.Fingers[0].ScreenPosition);
                    if (Physics.Raycast(ray, out RaycastHit hitInfo))
                    {
                        if (hitInfo.collider.CompareTag("Plane"))
                        {
                            if (DistanceToLastPoint(hitInfo.point) > 1)
                            {
                                isInteracted = true;

                                points.Add(new Vector3(hitInfo.point.x, hitInfo.point.y, 0));

                                lineRenderer.positionCount = points.Count;
                                lineRenderer.SetPositions(points.ToArray());
                            }
                        }
                    }

                    RaycastHit2D hitInfo2d = Physics2D.Raycast(levelManager.MainCamera.ScreenToWorldPoint(LeanTouch.Fingers[0].ScreenPosition), Vector2.zero);
                    if (hitInfo2d && hitInfo2d.collider.CompareTag("Lane"))
                    {
                        Debug.Log("Lane");
                        if (!isSoundPlayed)
                        {
                            Debug.Log("Lane");
                            isSoundPlayed = true;
                            AudioManager.Instance.PlayOneShot(AudioManager.SoundType.Road);
                            Taptic.Success();
                        }
                    }
                }

                if (LeanTouch.Fingers.Count == 0)
                {
                    pathDrawable = false;
                    isSoundPlayed = false;
                }

                if (points != null && points.Count != 0)
                {
                    Vector2 currentPos = points[moveIndex];
                    ChangeSpeed(moveSpeed);
                    transform.position = Vector2.MoveTowards(transform.position, currentPos, speed * Time.deltaTime);

                    LookAt(points[moveIndex]);

                    float distance = Vector3.Distance(currentPos, transform.position);

                    if (distance <= 0.05f)
                    {
                        points.RemoveAt(0);
                        lineRenderer.SetPositions(points.ToArray());
                    }

                    if (moveIndex > points.Count - 1 || points.Count == 0)
                    {
                        points.Clear();
                        lineRenderer.positionCount = 0;
                        moveIndex = 0;
                        pathDrawable = false;

                        SantaState = SantaStates.Idle;
                    }
                }
            }

            if (points.Count == 0 && LeanTouch.Fingers.Count == 0 && SantaState == SantaStates.FollowingPath)
            {
                SantaState = SantaStates.Idle;
            }

            if (SantaState == SantaStates.Idle)
            {
                pathDrawable = false;
                IdleMove();
            }
        }

        private void CheckNearbyCollision()
        {
            if (SantaState == SantaStates.Landing) return;

            int nearbySantaCount = 0;
            Collider2D[] colliders = Physics2D.OverlapCapsuleAll(transform.position, Vector2.one * collisionDetectRadius, CapsuleDirection2D.Horizontal, 0f);
            collidersNearbyList = new List<Collider2D>(colliders.ToList());

            foreach (var collider in colliders)
            {
                if (collider.TryGetComponent(out Santa nearbySanta) && nearbySanta != this)
                {
                    nearbySantaCount++;

                    if (startWarningSeq != null && !startWarningSeq.IsPlaying())
                    {
                        startWarningSeq?.Complete(true);
                        endWarningSeq?.Complete(true);

                        startWarningSeq = StartWarningSequence();
                    }

                    startWarningSeq ??= StartWarningSequence();
                }
            }

            if (nearbySantaCount == 0)
            {
                if (endWarningSeq != null && endWarningSeq.IsPlaying()) return;

                if (startWarningSeq != null && startWarningSeq.IsPlaying())
                {
                    startWarningSeq?.Complete(true);
                }

                if (endWarningSeq != null && !endWarningSeq.IsPlaying())
                {
                    startWarningSeq?.Complete(true);
                    endWarningSeq?.Complete(true);

                    endWarningSeq = EndWarningSequence();
                }

                endWarningSeq ??= EndWarningSequence();
            }
        }

        private Sequence StartWarningSequence()
        {
            Sequence seq = DOTween.Sequence();
            seq.SetAutoKill(false);

            seq.Append(warningRenderer.transform.DOScale(endScale, 0.5f));
            seq.Append(warningRenderer.transform.DOScale(new Vector3(endScale.x / 2f, endScale.y / 2f), 0.5f));

            return seq;
        }

        private Sequence EndWarningSequence()
        {
            Sequence seq = DOTween.Sequence();
            seq.SetAutoKill(false);

            seq.Append(warningRenderer.transform.DOScale(Vector3.zero, 0.5f));

            return seq;
        }

        public void LookAt(Vector3 targetPos)
        {
            Vector2 direction = (targetPos - transform.position).normalized;
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            float currentAngle = Mathf.LerpAngle(transform.eulerAngles.z, targetAngle, rotationSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0, 0, currentAngle);
        }

        private float DistanceToLastPoint(Vector2 point)
        {
            if (!points.Any())
                return Mathf.Infinity;

            return Vector2.Distance(points.Last(), point);
        }

        public void ColorLine(Color color)
        {
            Material[] mats = new Material[1];
            mats[0] = lineRenderer.materials[0];
            mats[0].color = color;
            lineRenderer.materials = mats;
        }

        public void IdleMove()
        {
            ChangeSpeed(idleSpeed);
            transform.Translate(Vector3.right * speed * Time.deltaTime);
        }

        public void ChangeSpeed(float value)
        {
            speed = Mathf.Lerp(speed, value, Time.deltaTime * 3f);
        }

        private void ColorSanta()
        {
            if (colorRenderer == null) return;

            colorRenderer.color = levelManager.GetRandomLaneColor();
            ColorLine(colorRenderer.color);
        }

        public void SetLandable(bool isLandable)
        {
            this.isLandable = isLandable;
        }

        public bool IsLandable()
        {
            return isLandable && isInteracted;
        }

        public void ClearPoints()
        {
            points.Clear();
            lineRenderer.positionCount = 0;
            moveIndex = 0;
            pathDrawable = false;
        }

        private void AlignWarning()
        {
            Vector3 newRotation = new Vector3(transform.rotation.x, transform.rotation.y, -transform.rotation.z);
            warningRenderer.transform.rotation = Quaternion.Euler(newRotation);
        }

        public void SetCollideable(bool isCollidable)
        {
            santaCollider.enabled = isCollidable;
        }

        public void SetRenderOrder(int val)
        {
            santaRenderer.sortingOrder = val;
            if (colorRenderer != null)
                colorRenderer.sortingOrder = santaRenderer.sortingOrder - 1;
        }

        public void Die()
        {
            SantaState = SantaStates.Dead;

            transform.DOKill();
            warningRenderer.transform.localScale = Vector3.zero;

            Instantiate(confettiBlast, transform.position, Quaternion.identity);
            AudioManager.Instance.PlayOneShot(AudioManager.SoundType.BlowUp);

            levelManager.RemoveActiveSanta(this);
            levelManager.CheckEndGame();

            gameObject.SetActive(false);
        }

        public Color GetSantaColor()
        {
            return colorRenderer.color;
        }
    }
}