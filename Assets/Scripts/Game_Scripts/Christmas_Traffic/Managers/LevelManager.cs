using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Christmas_Traffic
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance;

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

        [Header("Lanes")]
        [SerializeField] private List<SpriteRenderer> laneRenderers = new List<SpriteRenderer>();
        [SerializeField] private List<Color> laneColors = new List<Color>();

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
            StartGame();
        }

        void Update()
        {

        }

        private void StartGame()
        {
            ColorLanes();

            State = GameState.Playing;
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