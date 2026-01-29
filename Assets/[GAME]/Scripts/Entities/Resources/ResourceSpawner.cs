using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ResourceSpawner
{
    private CancellationTokenSource _cancellationTokenSource;
    private ResourcesGenerator _rewardGenerator;
    private Transform _spawnPoint;
    private float _radius;
    private float _delaySpawn;

    public ResourceSpawner(
        ResourcesGenerator rewardGenerator, 
        Transform spawnPoint,
        float radius,
        float delay)
    {
        _rewardGenerator = rewardGenerator;
        _spawnPoint = spawnPoint;
        _radius = radius;
        _delaySpawn = delay;
    }

    public void SpawnResources()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = new CancellationTokenSource();
        SpawnLootCoroutine(_cancellationTokenSource.Token).Forget();
    }

    private async UniTask SpawnLootCoroutine(CancellationToken token)
    {
        List<ResourceData> datas = _rewardGenerator.GetResourceDatas();

        foreach (ResourceData data in datas)
        {
            SpawnSingleLoot(data);
            await UniTask.Delay((int)(_delaySpawn * 1000), cancellationToken: token);
        }
    }

    private void SpawnSingleLoot(ResourceData data)
    {
        if (data == null) return;

        ResourceItem loot = PoolManager.GetPool(data.Prefab, _spawnPoint.position, Quaternion.identity);

        Vector2 randomCircle = Random.insideUnitCircle * _radius;
        randomCircle.y = randomCircle.y < 1f ? 2f : randomCircle.y;

        Vector3 targetPos = _spawnPoint.transform.position + new Vector3(randomCircle.x, randomCircle.y, _spawnPoint.transform.position.z);

        loot.Init(data);
        loot.Spawn(targetPos);
    }

    public void OnDestroy()
    {
        _cancellationTokenSource?.Cancel();
    }
}