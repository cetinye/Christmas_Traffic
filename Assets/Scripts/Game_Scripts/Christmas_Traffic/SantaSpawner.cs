using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Christmas_Traffic
{
    public class SantaSpawner : MonoBehaviour
    {
        [SerializeField] private List<Santa> santaPrefabs;
        [SerializeField] private List<Transform> spawnPoints;
        [SerializeField] private List<Santa> santasToSpawn;

        private LevelManager levelManager;

        [SerializeField] private List<Vector3> usedPoints = new List<Vector3>();
        [SerializeField] private List<Vector3> tmpUsedPoints = new List<Vector3>();

        void Start()
        {
            levelManager = LevelManager.Instance;
        }

        void OnDestroy()
        {
            CancelInvoke(nameof(SpawnRandomSanta));
            StopAllCoroutines();
        }

        public void Initialize()
        {
            for (int i = 0; i < levelManager.LevelSO.MooseSantaAmount; i++)
            {
                Santa s = Instantiate(santaPrefabs[0]);
                s.gameObject.SetActive(false);
                santasToSpawn.Add(s);
            }

            for (int i = 0; i < levelManager.LevelSO.RocketSantaAmount; i++)
            {
                Santa s = Instantiate(santaPrefabs[1]);
                s.gameObject.SetActive(false);
                santasToSpawn.Add(s);
            }

            for (int i = 0; i < levelManager.LevelSO.BalloonSantaAmount; i++)
            {
                Santa s = Instantiate(santaPrefabs[2]);
                s.gameObject.SetActive(false);
                santasToSpawn.Add(s);
            }

            santasToSpawn.Shuffle();
        }

        public void StartSpawning()
        {
            InvokeRepeating(nameof(SpawnRandomSanta), 0f, 5f);
        }

        public void SpawnRandomSanta()
        {
            if (santasToSpawn.Count == 0) return;
            if (levelManager.State != LevelManager.GameState.Playing) return;

            Santa santaToSpawn = santasToSpawn[0];
            Vector3 posToSpawn = GetAvailableSpawnPoint();

            santasToSpawn.RemoveAt(0);
            santaToSpawn.Initialize();

            santaToSpawn.transform.position = posToSpawn;
            santaToSpawn.transform.right = Vector3.zero - santaToSpawn.transform.position;

            santaToSpawn.gameObject.SetActive(true);
        }

        private Vector3 GetAvailableSpawnPoint()
        {
            Vector3 positionToSpawn;

            if (usedPoints.Count > 1)
                RemoveUsedPoints(tmpUsedPoints[0]);

            do
            {
                positionToSpawn = spawnPoints[Random.Range(0, spawnPoints.Count)].position;
            } while (usedPoints.Contains(positionToSpawn));

            AddUsedPoints(positionToSpawn);
            return positionToSpawn;
        }

        private void AddUsedPoints(Vector3 position)
        {
            usedPoints.Add(position);
            tmpUsedPoints.Add(position);
        }

        private void RemoveUsedPoints(Vector3 position)
        {
            tmpUsedPoints.Remove(position);
            StartCoroutine(RemoveRoutine(position));
        }

        IEnumerator RemoveRoutine(Vector3 position)
        {
            yield return new WaitForSeconds(10);
            usedPoints.Remove(position);
        }
    }
}