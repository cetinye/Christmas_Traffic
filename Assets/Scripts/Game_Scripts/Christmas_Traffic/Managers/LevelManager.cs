using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Christmas_Traffic
{
    public class LevelManager : MonoBehaviour
    {
        public float width;

        public static LevelManager Instance;
        public Camera MainCamera;
        [SerializeField] private UIManager uiManager;

        [Header("Level Variables")]
        public int LevelId;
        [SerializeField] private List<LevelSO> levels = new List<LevelSO>();
        public LevelSO LevelSO;

        [Header("Game States")]
        private GameState gameState = GameState.Idle;

        public enum GameState
        {
            Idle,
            Playing,
            Success,
            Fail
        }

        public GameState State
        {
            get { return gameState; }
            set
            {
                gameState = value;
                Debug.Log("Game State: " + gameState);
            }
        }

        [Header("Spawner Variables")]
        [SerializeField] private SantaSpawner santaSpawner;
        [SerializeField] private List<Santa> activeSantas = new List<Santa>();

        [Header("Lanes")]
        [SerializeField] private List<SpriteRenderer> laneRenderers = new List<SpriteRenderer>();
        [SerializeField] private SpriteRenderer helipad;
        [SerializeField] private List<Color> laneColors = new List<Color>();

        [Space()]
        private int correctCount;
        private int wrongCount;

        [Space()]
        private float gameTimer;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }

        void Start()
        {
            Initialize();
        }

        void Update()
        {
            GameTimer();
        }

        private void ScaleCameraSize()
        {
            MainCamera.orthographicSize = width / Screen.width * Screen.height / 2.0f;
        }

        private void Initialize()
        {
            AudioManager.Instance.Play(AudioManager.SoundType.Background);
            ScaleCameraSize();
            AssignLevel();
            SetLanesVisibility();
            ColorLanes();
            santaSpawner.Initialize();
            uiManager.Initialize();
            uiManager.ShowInfoPanels();
        }

        public void StartGame()
        {
            santaSpawner.StartSpawning();
            State = GameState.Playing;
        }

        private void AssignLevel()
        {
            LevelId = Mathf.Clamp(LevelId, 1, levels.Count);
            LevelSO = levels[LevelId - 1];

            gameTimer = LevelSO.TotalTime;
        }

        private void GameTimer()
        {
            if (State != GameState.Playing) return;

            gameTimer -= Time.deltaTime;

            if (gameTimer <= 6.2f && uiManager.IsFlashable)
            {
                uiManager.IsFlashable = false;
                uiManager.Last5SecWaring();
            }

            if (gameTimer <= 5 && !uiManager.IsFlashable)
                uiManager.SetCountdownText(gameTimer);

            if (gameTimer < 0)
            {
                gameTimer = 0;
                AudioManager.Instance.Stop(AudioManager.SoundType.Background);
                AudioManager.Instance.PlayOneShot(AudioManager.SoundType.TimesUp);
                uiManager.UpdateTimeSlider(gameTimer, LevelSO.TotalTime);
                uiManager.TimesUp();
                State = GameState.Fail;
            }

            uiManager.UpdateTimeSlider(gameTimer, LevelSO.TotalTime);
        }

        public void CheckEndGame()
        {
            if (activeSantas.Count != 0) return;

            DecideLevel();
        }

        private void DecideLevel()
        {
            var levelUpCounter = PlayerPrefs.GetInt("ChristmasTraffic_LevelUpCounter", 0);
            var levelDownCounter = PlayerPrefs.GetInt("ChristmasTraffic_LevelDownCounter", 0);

            if (correctCount >= LevelSO.NumOfSantasRequiredToLand)
            {
                State = GameState.Success;
                if (++levelUpCounter >= LevelSO.LevelUpCriteria)
                {
                    Debug.Log("Level UP");
                    LevelId++;
                    levelDownCounter = 0;
                    levelUpCounter = 0;
                }
            }
            else
            {
                State = GameState.Fail;
                if (++levelDownCounter >= LevelSO.LevelDownCriteria)
                {
                    Debug.Log("Level DOWN");
                    LevelId--;
                    levelDownCounter = 0;
                    levelUpCounter = 0;
                }
            }

            PlayerPrefs.SetInt("ChristmasTraffic_Level", LevelId);

            PlayerPrefs.SetInt("ChristmasTraffic_LevelUpCounter", levelUpCounter);
            PlayerPrefs.SetInt("ChristmasTraffic_LevelDownCounter", levelDownCounter);
        }

        private void SetLanesVisibility()
        {
            for (int i = 0; i < laneRenderers.Count; i++)
            {
                laneRenderers[i].gameObject.SetActive(false);
                helipad.gameObject.SetActive(false);
            }

            if (LevelSO.BalloonSantaAmount == 0)
            {
                List<SpriteRenderer> renderers = new List<SpriteRenderer>(laneRenderers);
                renderers.Shuffle();
                for (int i = 0; i < LevelSO.ActiveLaneCount; i++)
                {
                    renderers[i].gameObject.SetActive(true);
                }
            }
            else
            {
                List<SpriteRenderer> renderers = new List<SpriteRenderer>(laneRenderers);
                renderers.Shuffle();
                for (int i = 0; i < LevelSO.ActiveLaneCount - 1; i++)
                {
                    renderers[i].gameObject.SetActive(true);
                }
                helipad.gameObject.SetActive(true);
            }

        }

        private void ColorLanes()
        {
            laneColors.Shuffle();

            for (int i = 0; i < laneRenderers.Count; i++)
            {
                laneRenderers[i].color = laneColors[i];
            }
        }

        public Color GetRandomLaneColor()
        {
            SpriteRenderer sr;
            do
            {
                sr = laneRenderers[Random.Range(0, laneRenderers.Count)];
            } while (!sr.gameObject.activeSelf);
            return sr.color;
        }

        public void IncrementCorrect()
        {
            correctCount++;
            uiManager.UpdateStatsText(correctCount, wrongCount);
        }

        public void IncrementWrong()
        {
            wrongCount++;
            uiManager.UpdateStatsText(correctCount, wrongCount);
        }

        public void AddActiveSanta(Santa s)
        {
            if (s != null && !activeSantas.Contains(s))
                activeSantas.Add(s);
        }

        public void RemoveActiveSanta(Santa s)
        {
            if (s != null && activeSantas.Contains(s))
                activeSantas.Remove(s);
        }
    }

    public static class IListExtensions
    {
        /// <summary>
        /// Shuffles the element order of the specified list.
        /// </summary>
        public static void Shuffle<T>(this IList<T> ts)
        {
            var count = ts.Count;
            var last = count - 1;
            for (var i = 0; i < last; ++i)
            {
                var r = UnityEngine.Random.Range(i, count);
                var tmp = ts[i];
                ts[i] = ts[r];
                ts[r] = tmp;
            }
        }
    }
}