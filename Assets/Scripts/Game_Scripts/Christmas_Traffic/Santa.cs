using UnityEngine;

namespace Christmas_Traffic
{
    public class Santa : MonoBehaviour
    {
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
        private FollowLine followLine;
        private PathCreator pathCreator;
        private LevelManager levelManager;

        void Awake()
        {
            followLine = GetComponent<FollowLine>();
            pathCreator = GetComponent<PathCreator>();
        }

        void Start()
        {
            levelManager = LevelManager.Instance;

            Invoke(nameof(Initialize), 0.5f);
        }

        public void Initialize()
        {
            ColorSanta();
        }

        private void ColorSanta()
        {
            if (colorRenderer == null) return;

            colorRenderer.color = levelManager.GetRandomLaneColor();
            pathCreator.ColorLine(colorRenderer.color);
        }
    }
}