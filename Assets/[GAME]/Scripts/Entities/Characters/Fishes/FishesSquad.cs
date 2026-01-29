using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class FishesSquad : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private FishesConfig _fishesConfig;
    [SerializeField] private List<EntityType> _fishTypes = new List<EntityType>();
    [SerializeField] private int _initialFishCount = 3;
    [SerializeField] private int _maxFishCount = 10;
    [SerializeField] private float _minSpawnRadius = 2f;
    [SerializeField] private float _maxSpawnRadius = 8f;
    [SerializeField] private float _respawnDelay = 5f;

    [Header("Squad Area")]
    [SerializeField] private float _squadRadius = 15f;

    private List<AbstractFish> _activeFishes = new List<AbstractFish>();
    private List<EntityType> _availableTypes = new List<EntityType>()
    {
        EntityType.ClownFish,
        EntityType.GoldenFish,
        EntityType.TroutFish
    };
    private Coroutine _respawnCoroutine;
    private bool _isRespawning = false;
    private float _waterEdgeY = float.MinValue;

    private void Awake()
    {
        foreach (var type in _fishTypes)
            if (_availableTypes.Contains(type) == false)
                throw new ArgumentException($"The type <color=yellow>{type}</color> is not a fish type!");
    }

    public void Init()
    {
        StartCoroutine(SpawnInitialFish());
    }

    private IEnumerator SpawnInitialFish()
    {
        yield return new WaitForSeconds(1);

        _waterEdgeY = SL.Get<GeneralComponentsService>().GetGeneralSettingsConfig().WaterLevelY;

        if (_fishTypes.Count == 0)
            throw new ArgumentException("No fish prefabs assigned to FishesSquad!");

        int initialCount = Mathf.Min(_maxFishCount, _initialFishCount);

        for (int i = 0; i < initialCount; i++)
            SpawnFish();
    }

    private void SpawnFish()
    {
        if (_activeFishes.Count >= _maxFishCount)
            return;

        if (_fishTypes.Count == 0)
            return;

        EntityType fishType = _fishTypes[Random.Range(0, _fishTypes.Count)];
        FishData data = _fishesConfig.FishDatas.First(t => t.EntityType == fishType);

        bool isAvailablePoint = false;
        Vector2 randomCircle = Vector2.zero;
        while (isAvailablePoint == false)
        {
            randomCircle = Random.insideUnitCircle * _maxSpawnRadius;
            if (randomCircle.magnitude > _minSpawnRadius)
                isAvailablePoint = true;
        }

        Vector3 spawnPosition = transform.position +
                              new Vector3(randomCircle.x, Mathf.Clamp(randomCircle.y, randomCircle.y, _waterEdgeY), transform.position.z);
        Vector3 spawnRotation = new Vector3(0, Random.Range(0, 1) == 0 ? 90f : -90f, 0);

        var fishGO = PoolManager.GetPool(data.FishPrefab, transform);
        fishGO.transform.SetPositionAndRotation(spawnPosition, Quaternion.Euler(spawnRotation));
        AbstractFish fish = fishGO.GetComponent<AbstractFish>();

        if (fish != null)
        {
            fish.Init(data, transform.position, _squadRadius);
            _activeFishes.Add(fish);
            fish.FishDied += OnFishDeath;
        }

        StartCoroutine(StartRespawnMonitoring());
    }

    private void OnFishDeath(AbstractFish deadFish)
    {
        deadFish.FishDied -= OnFishDeath;
        _activeFishes.Remove(deadFish);

        if (!_isRespawning)
            StartRespawnCoroutine();
    }

    private IEnumerator StartRespawnMonitoring()
    {
        yield return new WaitForSeconds(1);
        CheckRespawnCondition();
    }

    private void CheckRespawnCondition()
    {
        if (_activeFishes.Count < _maxFishCount && !_isRespawning)
            StartRespawnCoroutine();

        if (_activeFishes.Count >= _maxFishCount && _isRespawning)
            StopRespawnCoroutine();
    }

    private void StartRespawnCoroutine()
    {
        if (_respawnCoroutine == null)
            _respawnCoroutine = StartCoroutine(RespawnFishRoutine());
    }

    private void StopRespawnCoroutine()
    {
        if (_respawnCoroutine != null)
        {
            StopCoroutine(_respawnCoroutine);
            _respawnCoroutine = null;
            _isRespawning = false;
        }
    }

    private IEnumerator RespawnFishRoutine()
    {
        _isRespawning = true;
        float delay = _activeFishes.Count == 0 ? 0f : _respawnDelay;

        yield return new WaitForSeconds(delay);

        if (_activeFishes.Count < _maxFishCount)
            SpawnFish();

        _isRespawning = false;
        _respawnCoroutine = null;
    }

    private void OnDrawGizmosSelected()
    {

        Gizmos.color = new Color(1f, 0.5f, 0f, 0.1f);
        Gizmos.DrawSphere(transform.position, _squadRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _minSpawnRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _maxSpawnRadius);
    }
}