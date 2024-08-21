using DG.Tweening;
using System.Collections;
using UnityEngine;

public class ParallaxManager : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float baseSpeed = 1.0f;
    [SerializeField] private float instantiationDistance = 10.0f;
    [SerializeField] private float countdownTime = 3f;

    private Coroutine _coroutineCountdown;

    private bool isPlaying = false;
    private bool _isCountingDown = false;
    public bool IsCountingDown { get { return _isCountingDown; } set { _isCountingDown = value; } }

    [System.Serializable]
    public class ParallaxCategory
    {
        public Transform[] prefabs;
        public float speedMultiplier = 1.0f;
        public float zOffset = 0;
        public float xRespawnDistance = -7f;
        public float minPosSpawnX = 8.5f;
        public float maxPosSpawnX = 17f;
        public float minGap = 2.0f;
        [HideInInspector] public float lastPrefabXPosition = 0;
    }

    [SerializeField] private ParallaxCategory foregroundCategory;
    [SerializeField] private ParallaxCategory playerCategory;
    [SerializeField] private ParallaxCategory backgroundCategory;
    [SerializeField] private ParallaxCategory farBackgroundCategory;

    private void Start()
    {
        InstantiateFirst();
    }

    public void Restart()
    {
        ClearAll();
        InstantiateFirst();
    }

    private void InstantiateFirst()
    {
        InstantiatePrefabs(foregroundCategory, "_foregroundPlane");
        InstantiatePrefabs(playerCategory, "_playerPlane");
        InstantiatePrefabs(backgroundCategory, "_backgroundPlane");
        InstantiatePrefabs(farBackgroundCategory, "_skyPlane");
    }

    private void ClearAll()
    {
        Clear(foregroundCategory);
        Clear(playerCategory);
        Clear(backgroundCategory);
        Clear(farBackgroundCategory);
    }

    private void Clear(ParallaxCategory category)
    {
        for (int i = 0; i < category.prefabs.Length; i++)
        {
            Destroy(category.prefabs[i].gameObject);            
        }
    }

    private void Update()
    {
        if (GameManager.Instance.IsPlaying)
        {
            // Check if IsPlaying just changed from false to true
            if (!isPlaying)
                _isCountingDown = isPlaying = true;

            if (_isCountingDown)
            {
                if (_coroutineCountdown == null)
                    _coroutineCountdown = StartCoroutine(StartRaceWithDelay(countdownTime));
            }
            else
            {
                MoveParallaxCategory(foregroundCategory);
                MoveParallaxCategory(playerCategory);
                MoveParallaxCategory(backgroundCategory);
                MoveParallaxCategory(farBackgroundCategory);
            }
        }
        else
        {
            // Reset countdown when IsPlaying is false
            _isCountingDown = isPlaying = false;
        }
    }

    private void InstantiatePrefabs(ParallaxCategory category, string planeName)
    {
        Vector3 spawnPosition = playerTransform.position + new Vector3(0.0f, 0.0f, category.zOffset);

        for (int i = 0; i < category.prefabs.Length; i++)
        {
            // Calculate the new prefab's position with a minimum gap
            float randomXOffset = Random.Range(category.minPosSpawnX, category.maxPosSpawnX);
            float newXPosition = category.lastPrefabXPosition + category.minGap + randomXOffset;
            Vector3 newPosition = spawnPosition + new Vector3(newXPosition, 0f, 0f);

            Transform tr = Instantiate(category.prefabs[i], newPosition, Quaternion.identity);
            tr.gameObject.name = tr.gameObject.name + planeName;
            category.prefabs[i] = tr;

            // Update the last prefab's X position
            category.lastPrefabXPosition = newXPosition;
        }
    }

    private void MoveParallaxCategory(ParallaxCategory category)
    {
        foreach (Transform prefab in category.prefabs)
        {
            Vector3 newPosition = prefab.position;
            float speed = baseSpeed * category.speedMultiplier;

            newPosition.x -= speed * Time.deltaTime;
            newPosition.y = 0.5f;

            prefab.position = newPosition;

            if (category.xRespawnDistance > prefab.position.x)
            {
                instantiationDistance += Random.Range(category.minGap, 4);
                Vector3 respawnPosition = (playerTransform.position + Vector3.right * instantiationDistance) + new Vector3(0.0f, 0.0f, category.zOffset);
                respawnPosition.y = 0.5f;
                prefab.position = respawnPosition;
            }
        }
    }

    private IEnumerator StartRaceWithDelay(float delay)
    {
        GameManager.Instance.CountdownText.gameObject.SetActive(true);
        GameManager.Instance.CountdownText.transform.localScale = Vector3.one * .5f;
        GameManager.Instance.DisplayCountdown("Get Ready in...");
        GameManager.Instance.CountdownText.transform.DOScale(Vector3.one, .25f);
        yield return new WaitForSeconds(1f);
        GameManager.Instance.CountdownText.transform.localScale = Vector3.one * .5f;
        float countdown = delay;

        while (countdown > 0f)
        {
            GameManager.Instance.DisplayCountdown(countdown.ToString("0"));
            // Scale up animation using DOTween
            GameManager.Instance.CountdownText.transform.DOScale(Vector3.one, .25f);
            yield return new WaitForSeconds(1f);
            GameManager.Instance.CountdownText.transform.localScale = Vector3.one * .5f;
            countdown -= 1f;
        }

        GameManager.Instance.DisplayCountdown("Go!");
        GameManager.Instance.CountdownText.transform.localScale = Vector3.one * .5f;
        GameManager.Instance.CountdownText.transform.DOScale(Vector3.one, .25f);
        yield return new WaitForSeconds(1f);
        GameManager.Instance.CountdownText.gameObject.SetActive(false);

        //start game
        _isCountingDown = false;
        _coroutineCountdown = null;
    }
}
